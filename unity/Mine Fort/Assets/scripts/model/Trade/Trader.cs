#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software, 
// and you are welcome to redistribute it under certain conditions; See 
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion
using Rimworld.model.Inventory;
using System.Collections.Generic;
using System.Linq;

public class Trader
{
    public string Name { get; set; }

    public Currency Currency { get; set; }

    public float SaleMarginMultiplier { get; set; }

    public List<GameInventory> Stock { get; set; }

    /// <summary>
    /// Create a Trader from the current player
    /// This method will scan every stockpile build and add the found inventory to the stock
    /// It will also assign a 0.8f sale margin multiplayer to the Trader.
    /// </summary>
    public static Trader FromPlayer(Currency currency)
    {
        Trader trader = new Trader
        {
            Name = "Player",
            SaleMarginMultiplier = 0.8f,
            Stock = new List<GameInventory>(),
            Currency = currency
        };

        List<List<GameInventory>> worldInventories = World.Current.InventoryManager.Inventories.Values.Select(i => i.ToList()).ToList();
        
        foreach (List<GameInventory> worldInventory in worldInventories)
        {
            foreach (GameInventory inventory in worldInventory)
            {
                if (inventory.Tile != null && 
                    inventory.Tile.Furniture != null &&
                    inventory.Tile.Furniture.HasTypeTag("Stockpile"))
                {
                    trader.Stock.Add(inventory);
                }
            }
        }
        
        return trader;
    }
}
