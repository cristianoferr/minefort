using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using Rimworld.logic;
using Rimworld.model.entities;
using Rimworld.logic.Jobs;
using Rimworld.model.Pathfinding;

namespace Rimworld.model.inventory
{
    [MoonSharpUserData]
    public class InventoryManager
    {

        // This is a list of all "live" inventories.
        // Later on this will likely be organized by rooms instead
        // of a single master list. (Or in addition to.)
        public Dictionary<string, List<GameInventory>> inventories;

        public InventoryManager()
        {
            inventories = new Dictionary<string, List<GameInventory>>();
        }

        void CleanupInventory(GameInventory inv)
        {
            if (inv.stackSize == 0)
            {
                if (inventories.ContainsKey(inv.objectType))
                {
                    inventories[inv.objectType].Remove(inv);
                }
                if (inv.tile != null)
                {
                    inv.tile.inventory = null;
                    inv.tile = null;
                }
                if (inv.character != null)
                {
                    inv.character.inventory = null;
                    inv.character = null;
                }
            }

        }

        public bool PlaceInventory(Tile tile, GameInventory inv)
        {

            bool tileWasEmpty = tile.inventory == null;

            if (tile.PlaceInventory(inv) == false)
            {
                // The tile did not accept the inventory for whatever reason, therefore stop.
                return false;
            }

            CleanupInventory(inv);

            // We may also created a new stack on the tile, if the tile was previously empty.
            if (tileWasEmpty)
            {
                if (inventories.ContainsKey(tile.inventory.objectType) == false)
                {
                    inventories[tile.inventory.objectType] = new List<GameInventory>();
                }

                inventories[tile.inventory.objectType].Add(tile.inventory);

                World.current.OnInventoryCreated(tile.inventory);
            }

            return true;
        }

        public bool PlaceInventory(Job job, GameInventory inv)
        {
            if (job.inventoryRequirements.ContainsKey(inv.objectType) == false)
            {
                Debug.LogError("Trying to add inventory to a job that it doesn't want.");
                return false;
            }

            job.inventoryRequirements[inv.objectType].stackSize += inv.stackSize;

            if (job.inventoryRequirements[inv.objectType].maxStackSize < job.inventoryRequirements[inv.objectType].stackSize)
            {
                inv.stackSize = job.inventoryRequirements[inv.objectType].stackSize - job.inventoryRequirements[inv.objectType].maxStackSize;
                job.inventoryRequirements[inv.objectType].stackSize = job.inventoryRequirements[inv.objectType].maxStackSize;
            }
            else
            {
                inv.stackSize = 0;
            }

            CleanupInventory(inv);

            return true;
        }

        public bool PlaceInventory(GameCharacter character, GameInventory sourceInventory, int amount = -1)
        {
            if (amount < 0)
            {
                amount = sourceInventory.stackSize;
            }
            else
            {
                amount = Mathf.Min(amount, sourceInventory.stackSize);
            }

            if (character.inventory == null)
            {
                character.inventory = sourceInventory.Clone();
                character.inventory.stackSize = 0;
                inventories[character.inventory.objectType].Add(character.inventory);
            }
            else if (character.inventory.objectType != sourceInventory.objectType)
            {
                Debug.LogError("Character is trying to pick up a mismatched inventory object type.");
                return false;
            }

            character.inventory.stackSize += amount;

            if (character.inventory.maxStackSize < character.inventory.stackSize)
            {
                sourceInventory.stackSize = character.inventory.stackSize - character.inventory.maxStackSize;
                character.inventory.stackSize = character.inventory.maxStackSize;
            }
            else
            {
                sourceInventory.stackSize -= amount;
            }

            CleanupInventory(sourceInventory);

            return true;
        }

        /// <summary>
        /// Gets the type of the closest inventory of.
        /// </summary>
        /// <returns>The closest inventory of type.</returns>
        /// <param name="objectType">Object type.</param>
        /// <param name="t">T.</param>
        /// <param name="desiredAmount">Desired amount. If no stack has enough, it instead returns the largest</param>
        public GameInventory GetClosestInventoryOfType(string objectType, Tile t, int desiredAmount, bool canTakeFromStockpile)
        {
            Path_AStar path = GetPathToClosestInventoryOfType(objectType, t, desiredAmount, canTakeFromStockpile);
            return path.EndTile().inventory;
        }

        public Path_AStar GetPathToClosestInventoryOfType(string objectType, Tile t, int desiredAmount, bool canTakeFromStockpile)
        {
            if (inventories.ContainsKey(objectType) == false)
            {
                //Debug.LogError("GetClosestInventoryOfType -- no items of desired type.");
                return null;
            }

            Path_AStar path = new Path_AStar(World.current, t, null, objectType, desiredAmount, canTakeFromStockpile);

            return path;

        }
    }

}
