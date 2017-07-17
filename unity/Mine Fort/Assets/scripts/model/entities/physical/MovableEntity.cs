
using Rimworld.model.inventory;
using Rimworld.model.Pathfinding;
using UnityEngine;
namespace Rimworld.model.entities
{
    //C vvvxxxx
    public class MovableEntity : PhysicalEntity
    {
        public MovableEntity()
            : base()
        {
        }
        internal Path_AStar pathAStar;

        public Tile nextTile { get; set; }
        public GameInventory inventory;
        public override float X
        {
            get
            {
                if (nextTile == null)
                {
                    return base.X;
                }
                return Mathf.Lerp(currTile.X, nextTile.X, movementPercentage);
            }
        }


        public override void Update(float deltaTime)
        {
            //Debug.Log("Character Update");
            base.Update(deltaTime);

            Update_DoMovement(deltaTime);

        }

        public virtual void AbandonJob()
        {
            nextTile = DestTile = currTile;
        }


        void Update_DoMovement(float deltaTime)
        {
            if (currTile == DestTile || DestTile==null)
            {
                pathAStar = null;
                return; // We're already were we want to be.
            }

            // currTile = The tile I am currently in (and may be in the process of leaving)
            // nextTile = The tile I am currently entering
            // destTile = Our final destination -- we never walk here directly, but instead use it for the pathfinding

            if (nextTile == null || nextTile == currTile)
            {
                // Get the next tile from the pathfinder.
                if (pathAStar == null || pathAStar.Length() == 0)
                {
                    // Generate a path to our destination
                    pathAStar = new Path_AStar(World.current, currTile, DestTile);  // This will calculate a path from curr to dest.
                    if (pathAStar.Length() == 0)
                    {
                        Debug.LogError("Path_AStar returned no path to destination!");
                        AbandonJob();
                        return;
                    }

                    // Let's ignore the first tile, because that's the tile we're currently in.
                    nextTile = pathAStar.Dequeue();

                }


                // Grab the next waypoint from the pathing system!
                nextTile = pathAStar.Dequeue();

                if (nextTile == currTile)
                {
                    Debug.LogError("Update_DoMovement - nextTile is currTile?");
                }
            }

            /*		if(pathAStar.Length() == 1) {
                        return;
                    }
            */
            // At this point we should have a valid nextTile to move to.

            // What's the total distance from point A to point B?
            // We are going to use Euclidean distance FOR NOW...
            // But when we do the pathfinding system, we'll likely
            // switch to something like Manhattan or Chebyshev distance
            float distToTravel = Mathf.Sqrt(
                Mathf.Pow(currTile.X - nextTile.X, 2) +
                Mathf.Pow(currTile.Y - nextTile.Y, 2)
            );

            if (nextTile.IsEnterable() == GameConsts.ENTERABILITY.Never)
            {
                // Most likely a wall got built, so we just need to reset our pathfinding information.
                // FIXME: Ideally, when a wall gets spawned, we should invalidate our path immediately,
                //		  so that we don't waste a bunch of time walking towards a dead end.
                //		  To save CPU, maybe we can only check every so often?
                //		  Or maybe we should register a callback to the OnTileChanged event?
                Debug.LogError("FIXME: A character was trying to enter an unwalkable tile.");
                nextTile = null;    // our next tile is a no-go
                pathAStar = null;   // clearly our pathfinding info is out of date.
                return;
            }
            else if (nextTile.IsEnterable() == GameConsts.ENTERABILITY.Soon)
            {
                // We can't enter the NOW, but we should be able to in the
                // future. This is likely a DOOR.
                // So we DON'T bail on our movement/path, but we do return
                // now and don't actually process the movement.
                return;
            }

            // How much distance can be travel this Update?
            float distThisFrame = speed / nextTile.movementCost * deltaTime;

            // How much is that in terms of percentage to our destination?
            float percThisFrame = distThisFrame / distToTravel;

            // Add that to overall percentage travelled.
            movementPercentage += percThisFrame;

            if (movementPercentage >= 1)
            {
                // We have reached our destination

                // TODO: Get the next tile from the pathfinding system.
                //       If there are no more tiles, then we have TRULY
                //       reached our destination.

                currTile = nextTile;
                movementPercentage = 0;
                // FIXME?  Do we actually want to retain any overshot movement?
            }


        }

        public override float Y
        {
            get
            {
                if (nextTile == null)
                {
                    return base.Y;
                }
                return Mathf.Lerp(currTile.Y, nextTile.Y, movementPercentage);

            }
        }
        



        public void SetDestination(Tile tile)
        {
            if (currTile.IsNeighbour(tile, true) == false)
            {
                Utils.Log("Character::SetDestination -- Our destination tile isn't actually our neighbour.");
            }

            DestTile = tile;
        }

        // If we aren't moving, then destTile = currTile
        internal Tile _destTile;
        internal Tile DestTile
        {
            get { return _destTile; }
            set
            {
                if (_destTile != value)
                {
                    _destTile = value;
                    pathAStar = null;	// If this is a new destination, then we need to invalidate pathfinding.
                }
            }
        }

        
        float movementPercentage; // Goes from 0 to 1 as we move from currTile to destTile

        float speed = 5f;	// Tiles per second

    }
}
