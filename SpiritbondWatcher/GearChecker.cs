using Dalamud.Data;
using Dalamud.Game.Gui;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using System.Linq;

namespace SpiritbondWatcher;

public static class GearChecker
{
    private static readonly InventoryType[] InventoriesToSearch = {
        InventoryType.EquippedItems,
        InventoryType.ArmoryBody,
        InventoryType.ArmoryEar,
        InventoryType.ArmoryFeets,
        InventoryType.ArmoryHands,
        InventoryType.ArmoryHead,
        InventoryType.ArmoryLegs,
        InventoryType.ArmoryNeck,
        InventoryType.ArmoryRings,
        InventoryType.ArmoryWrist,
        InventoryType.ArmoryMainHand,
        InventoryType.ArmoryOffHand,
        // InventoryType.Inventory1,
        // InventoryType.Inventory2,
        // InventoryType.Inventory3,
        // InventoryType.Inventory4,
    };
    
    public static void CheckGear(DataManager data, ChatGui chat, Config config, string args)
    {
        var items =
            (from bondedItem in Inventory.GetBondedItems(InventoriesToSearch)
                join item in data.Excel.GetSheet<Item>()
                    on bondedItem equals item.RowId
                select item.Name).ToList();

        var stringBuilder = new SeStringBuilder();
        stringBuilder.AddUiForeground(34);

        string message;
        
        if (items.Any())
        {
            var newLine = config.BondedGearDisplayLineByLine;
            message = "Gear fully bonded:" + (newLine ? "\n" : " ");

            if (items.Count > 10)
            {
                message += string.Join((newLine ? "\n" : ", "), items.Take(10));
                message += string.Format((newLine ? "\n({0} more)" : " and {0} more"), items.Count - 10);
            }
            else
            {
                message += string.Join((newLine ? "\n" : ", "), items);
            }

            stringBuilder.AddText(message);
            chat.Print(stringBuilder.BuiltString);
        }
        else if(args != "zone")
        {
            message = "No gear fully bonded";

            stringBuilder.AddText(message);
            chat.Print(stringBuilder.BuiltString);
        }
    }
}