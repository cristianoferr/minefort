
using Rimworld.logic.Jobs;
using Rimworld.model.inventory;
using Rimworld.model.Pathfinding;
using UnityEngine;

namespace Rimworld.model.entities
{
    public class GameCharacter : MovableEntity
    {
        public GameCharacter()
            : base()
        {
        }


        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            Update_DoJob(deltaTime);
        }

        void GetNewJob()
        {

            myJob = World.current.jobQueue.Dequeue();
            if (myJob == null)
                return;

            DestTile = myJob.tile;
            myJob.RegisterJobStoppedCallback(OnJobStopped);

            // Immediately check to see if the job tile is reachable.
            // NOTE: We might not be pathing to it right away (due to 
            // requiring materials), but we still need to verify that the
            // final location can be reached.

            pathAStar = new Path_AStar(World.current, currTile, DestTile);  // This will calculate a path from curr to dest.
            if (pathAStar.Length() == 0)
            {
                Debug.LogError("Path_AStar returned no path to target job tile!");
                AbandonJob();
                DestTile = currTile;
            }
        }

        public override void AbandonJob()
        {
            base.AbandonJob();
            World.current.jobQueue.Enqueue(myJob);
            myJob = null;
        }

        void OnJobStopped(Job j)
        {
            // Job completed (if non-repeating) or was cancelled.

            j.UnregisterJobStoppedCallback(OnJobStopped);

            if (j != myJob)
            {
                Debug.LogError("Character being told about job that isn't his. You forgot to unregister something.");
                return;
            }

            myJob = null;
        }

        float jobSearchCooldown = 0;

        Job myJob;

        void Update_DoJob(float deltaTime)
        {
            // Do I have a job?
            jobSearchCooldown -= Time.deltaTime;
            if (myJob == null)
            {
                if (jobSearchCooldown > 0)
                {
                    // Don't look for job now.
                    return;
                }

                GetNewJob();

                if (myJob == null)
                {
                    // There was no job on the queue for us, so just return.
                    jobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
                    DestTile = currTile;
                    return;
                }
            }

            // We have a job! (And the job tile is reachable)

            // STEP 1: Does the job have all the materials it needs?
            if (myJob.HasAllMaterial() == false)
            {
                // No, we are missing something!

                // STEP 2: Are we CARRYING anything that the job location wants?
                if (inventory != null)
                {
                    if (myJob.DesiresInventoryType(inventory) > 0)
                    {
                        // If so, deliver the goods.
                        //  Walk to the job tile, then drop off the stack into the job.
                        if (currTile == myJob.tile)
                        {
                            // We are at the job's site, so drop the inventory
                            World.current.inventoryManager.PlaceInventory(myJob, inventory);
                            myJob.DoWork(0); // This will call all cbJobWorked callbacks, because even though
                                             // we aren't progressing, it might want to do something with the fact
                                             // that the requirements are being met.

                            // Are we still carrying things?
                            if (inventory.stackSize == 0)
                            {
                                inventory = null;
                            }
                            else
                            {
                                Debug.LogError("Character is still carrying inventory, which shouldn't be. Just setting to NULL for now, but this means we are LEAKING inventory.");
                                inventory = null;
                            }

                        }
                        else
                        {
                            // We still need to walk to the job site.
                            DestTile = myJob.tile;
                            return;
                        }
                    }
                    else
                    {
                        // We are carrying something, but the job doesn't want it!
                        // Dump the inventory at our feet
                        // TODO: Actually, walk to the nearest empty tile and dump it there.
                        if (World.current.inventoryManager.PlaceInventory(currTile, inventory) == false)
                        {
                            Debug.LogError("Character tried to dump inventory into an invalid tile (maybe there's already something here.");
                            // FIXME: For the sake of continuing on, we are still going to dump any
                            // reference to the current inventory, but this means we are "leaking"
                            // inventory.  This is permanently lost now.
                            inventory = null;
                        }
                    }
                }
                else
                {
                    // At this point, the job still requires inventory, but we aren't carrying it!

                    // Are we standing on a tile with goods that are desired by the job?
                    if (currTile.inventory != null &&
                        (myJob.canTakeFromStockpile || currTile.furniture == null || currTile.furniture.IsStockpile() == false) &&
                        myJob.DesiresInventoryType(currTile.inventory) > 0)
                    {
                        // Pick up the stuff!
                        Debug.Log("Pick up the stuff");

                        World.current.inventoryManager.PlaceInventory(
                            this,
                            currTile.inventory,
                            myJob.DesiresInventoryType(currTile.inventory)
                        );

                    }
                    else
                    {
                        // Walk towards a tile containing the required goods.
                        Debug.Log("Walk to the stuff");
                        Debug.Log(myJob.canTakeFromStockpile);


                        // Find the first thing in the Job that isn't satisfied.
                        GameInventory desired = myJob.GetFirstDesiredInventory();

                        if (currTile != nextTile)
                        {
                            // We are still moving somewhere, so just bail out.
                            return;
                        }

                        // Any chance we already have a path that leads to the items we want?
                        if (pathAStar != null && pathAStar.EndTile() != null && pathAStar.EndTile().inventory != null && pathAStar.EndTile().inventory.objectType == desired.objectType)
                        {
                            // We are already moving towards a tile that contains what we want!
                            // so....do nothing?
                        }
                        else
                        {
                            Path_AStar newPath = World.current.inventoryManager.GetPathToClosestInventoryOfType(
                                desired.objectType,
                                currTile,
                                desired.maxStackSize - desired.stackSize,
                                myJob.canTakeFromStockpile
                            );

                            if (newPath == null)
                            {
                                //Debug.Log("pathAStar is null and we have no path to object of type: " + desired.objectType);
                                // Cancel the job, since we have no way to get any raw materials!
                                AbandonJob();
                                return;
                            }


                            Debug.Log("pathAStar returned with length of: " + newPath.Length());

                            if (newPath == null || newPath.Length() == 0)
                            {
                                Debug.Log("No tile contains objects of type '" + desired.objectType + "' to satisfy job requirements.");
                                AbandonJob();
                                return;
                            }

                            DestTile = newPath.EndTile();

                            // Since we already have a path calculated, let's just save that.
                            pathAStar = newPath;

                            // Ignore first tile, because that's what we're already in.
                            nextTile = newPath.Dequeue();
                        }

                        // One way or the other, we are now on route to an object of the right type.
                        return;
                    }

                }

                return; // We can't continue until all materials are satisfied.
            }
        }
    }
}
