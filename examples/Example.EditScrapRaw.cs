using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;

namespace examples;

internal partial class Example
{
    public static void EditScrapRaw()
    {
        Console.WriteLine("Edit Scrap Raw API===========");

        const int targetScrapValue = 12345;
        const string targetFileName = "scrap_changed_raw.sav";

        string folder = Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        Console.WriteLine("Reading profile data...");
        SaveFile sf = SaveFile.Read(path);

        Console.WriteLine("Looking for scrap on the first character...");
        KeyValuePair<string, Property> charactersProp = sf.SaveData.Objects[0].Properties!.Properties.Single(x => x.Key == "Characters");

        ArrayProperty ap = (charactersProp.Value.Value as ArrayProperty)!;
        List<ObjectProperty> op = ap.Items.Select(x => (ObjectProperty)x!).ToList();
        ObjectProperty? character = op.FirstOrDefault(x => x.ObjectIndex >= 0);
        if (character == null)
        {
            Console.WriteLine("Do you have any characters?");
            return;
        }

        Property characterData = sf.SaveData.Objects[character.ObjectIndex].Properties!.Properties.SingleOrDefault(x => x.Key == "CharacterData").Value;
        StructProperty sp = (StructProperty)characterData.Value!;
        SaveData inner = (SaveData)sp.Value!;
        UObject master = inner.Objects.Single(x => x.Name == "Character_Master_Player_C");
        Component inventory = master.Components!.Single(x => x.ComponentKey == "Inventory");
        Property items = inventory.Properties!.Properties.Single(x => x.Key == "Items").Value;
        ArrayStructProperty asp = (ArrayStructProperty)items.Value!;

        const string scrapId = "/Game/World_Base/Items/Materials/Scraps/Material_Scraps.Material_Scraps_C";

        PropertyBag scrapInventory = (PropertyBag)asp.Items.Single(x =>
        {
            return ((PropertyBag)x!).Properties.Any(y =>
                y.Key == "ItemBP"
                && inner.Objects[((ObjectProperty)y.Value.Value!).ObjectIndex].ObjectPath == scrapId
                );
        })!;

        ObjectProperty instanceData = (ObjectProperty)scrapInventory.Properties.Single(x => x.Key == "InstanceData").Value.Value!;
        Property scrap = inner.Objects[instanceData.ObjectIndex].Properties!.Properties.Single(x => x.Key == "Quantity").Value;

        Console.WriteLine($"Current scrap value is {scrap.Value}. Changing to {targetScrapValue}...");
        scrap.Value = targetScrapValue;

        Console.WriteLine($"Writing to {targetFileName}...");
        SaveFile.Write(targetFileName, sf);
        Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in the game save folder!");
    }
}
