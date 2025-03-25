using System.Collections.Generic;
using Dalamud.Game.Inventory;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace SpiritbondWatcher;

public static unsafe class Inventory
{
    public static IEnumerable<uint> GetBondedItems(IEnumerable<InventoryType> inventoriesToSearch)
    {
        var bondedItems = new List<uint>();
        var manager = InventoryManager.Instance();

        foreach (var invType in inventoriesToSearch)
        {
            var container = manager->GetInventoryContainer(invType);
            var size = container->Size;

            for (var i = 0; i < size; i++)
            {
                var item = (GameInventoryItem*)container->GetInventorySlot(i);
                if (item->ItemId != 0 && !item->IsCollectable && item->SpiritbondOrCollectability >= 10000)
                {
                    bondedItems.Add(item->ItemId);
                }
            }
        }

        return bondedItems;
    } 
}