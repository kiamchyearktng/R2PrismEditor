using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{

    public static void Cass()
    {
        Console.WriteLine("Cass===========");

        string folder = Utils.GetSteamSavePath();
        string savePath = Path.Combine(folder, Environment.GetEnvironmentVariable("DEBUG_REMNANT_SAVE") ?? "save_0.sav");
        SaveFile sf = SaveFile.Read(savePath);
        Navigator navigator = new(sf);

        Actor cass = navigator.GetActor("Character_NPC_Cass_C")!;

        List<Component>? inventoryList = navigator.FindComponents("Inventory", cass);
        if (inventoryList is { Count: > 0 })
        {
            PropertyBag inventory = inventoryList[0].Properties!;

            ArrayStructProperty asp = (ArrayStructProperty)inventory["Items"].Value!;
            Console.WriteLine("Your Cass has following inventory:");

            foreach (object? o in asp.Items)
            {
                PropertyBag itemProperties = (PropertyBag)o!;

                Property item = itemProperties.Properties.Single(x => x.Key == "ItemBP").Value;
                Property hidden = itemProperties.Properties.Single(x => x.Key == "Hidden").Value;
                Property slot = itemProperties.Properties.Single(x => x.Key == "EquipmentSlotIndex").Value;

                if ((byte)hidden.Value! != 0)
                {
                    //Console.WriteLine($"    **************HIDDEN:");
                    continue;
                }

                string message = "";
                if ((int)slot.Value! != -1)
                {
                    message = $" ***********************EQUIPPED in slot {(int)slot.Value}";
                }

                Console.WriteLine(
                    $"    {Utils.GetShortenedAssetPath(((ObjectProperty)item.Value!).ClassName!)}{message}");

            }
        }
        else
        {
            Console.WriteLine("Cass inventory not found");
        }
    }
}
