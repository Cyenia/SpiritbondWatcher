using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System.Linq;
using Dalamud.Plugin.Services;
using Lumina.Text;

namespace SpiritbondWatcher;

public static class GearChecker
{
    private static readonly InventoryType[] InventoriesToSearch =
    [
        InventoryType.Inventory1,
        InventoryType.Inventory2,
        InventoryType.Inventory3,
        InventoryType.Inventory4,
        InventoryType.EquippedItems,
        InventoryType.ArmoryOffHand,
        InventoryType.ArmoryHead,
        InventoryType.ArmoryBody,
        InventoryType.ArmoryHands,
        InventoryType.ArmoryLegs,
        InventoryType.ArmoryFeets,
        InventoryType.ArmoryEar,
        InventoryType.ArmoryNeck,
        InventoryType.ArmoryWrist,
        InventoryType.ArmoryRings,
        InventoryType.ArmoryMainHand,
    ];
    
    public static void CheckGear(IDataManager data, IChatGui chat, Config config, string args)
    {
        var items =
            (from bondedItem in Inventory.GetBondedItems(InventoriesToSearch)
                join item in data.Excel.GetSheet<Item>()
                    on bondedItem equals item.RowId
                select item.Name).ToList();

        var stringBuilder = new SeStringBuilder();
        stringBuilder.PushColorType(69);

        var message = string.Empty;
        
        if (items.Count != 0)
        {
            var newLine = config.BondedGearDisplayLineByLine;
            stringBuilder.Append("Gear fully bonded:" + (newLine ? "\n" : " "));

            if (items.Count > 10)
            {
                message += string.Format((newLine ? "\n({0} more)" : " and {0} more"), items.Count - 10);
                items = items.Take(10).ToList();
            }

            for (var i = 0; i < items.Count; i++)
            {
                if (i != 0) stringBuilder.Append(newLine ? "\n" : " ");
                stringBuilder.Append(items[i]);
            }

            stringBuilder.Append(message);
            chat.Print(stringBuilder.ToArray());
        }
        else if(args != "zone")
        {
            message = "No gear fully bonded";

            stringBuilder.Append(message);
            chat.Print(stringBuilder.ToArray());
        }
    }
}