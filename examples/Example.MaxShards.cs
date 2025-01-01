using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{
    public static void MaxShards()
    {
        Console.WriteLine("Max Shards===========");

        const string shardId = "/Game/World_Base/Items/Materials/LumeniteCrystal/Material_CorruptedShard.Material_CorruptedShard_C";
        const string scrapId = "/Game/World_Base/Items/Materials/Scraps/Material_Scraps.Material_Scraps_C";
        const string targetFileName = "shards_maxed.sav";

        string folder = Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        Console.WriteLine("Reading profile data...");
        SaveFile sf = SaveFile.Read(path);
        Navigator navigator = new(sf);

        Console.WriteLine("Looking for existing shards on the first character...");

        Property? characters = navigator.GetProperty("Characters");

        ObjectProperty? character = characters!.GetItems<ObjectProperty>().FirstOrDefault();
        if (character == null)
        {
            Console.WriteLine("Do you have any characters?");
            return;
        }

        Property? shardItem = navigator.GetProperties("ItemBP", character.Object)
            .SingleOrDefault(x => x.Value!.ToString() == shardId);

        Property scrapItem = navigator.GetProperties("ItemBP", character.Object)
            .Single(x => x.Value!.ToString() == scrapId);


        if (shardItem != null)
        {
            Console.WriteLine("Your character has shards");
            var f = navigator.Lookup(shardItem);
            Property pp = navigator.GetProperty("InstanceData", f.Parent!.Object)!.Get<ObjectProperty>().Object!.Properties!["Quantity"];
            Console.WriteLine($"Updating quantity from {pp.Value} to 10");
            pp.Value = 10;
        }
        else
        {
            Console.WriteLine("Your character does not have shards, adding");
            
            UObject itemObject = (scrapItem.Value as ObjectProperty)!.Object!.ShallowCopyObject(navigator);
            itemObject.ObjectPath = shardId;

            UObject instanceDataObject = (scrapItem.GetParent<PropertyBag>(navigator)["InstanceData"].Value as ObjectProperty)!.Object!.ShallowCopyObject(navigator);
            instanceDataObject.LoadedData = instanceDataObject.LoadedData!.ShallowCopyLoadedData();
            instanceDataObject.Properties = new PropertyBag
            {
                Properties = instanceDataObject.Properties!.Properties
                    .Select(x => new KeyValuePair<string, Property>(x.Key, x.Value.ShallowCopyProperty())).ToList()
            };
            instanceDataObject.Properties.RefreshLookup();
            instanceDataObject.Properties["Quantity"].Value = 10;

            PropertyBag newItemBag = new()
            {
                Properties = scrapItem.GetParent<PropertyBag>(navigator).Properties
                    .Select(x => new KeyValuePair<string, Property>(x.Key, x.Value.ShallowCopyProperty())).ToList(),
            };
            newItemBag.RefreshLookup();
            newItemBag["ItemBP"].Value = new ObjectProperty { ObjectIndex = itemObject.ObjectIndex };
            newItemBag["InstanceData"].Value = new ObjectProperty { ObjectIndex = instanceDataObject.ObjectIndex };
            newItemBag["New"].Value = 1; // Just for in-game display

            ArrayStructProperty inventoryArray = scrapItem.GetParent<PropertyBag>(navigator).GetParent<ArrayStructProperty>(navigator);
            int maxId = inventoryArray.Items.Select(x => (int)((PropertyBag)x!)["ID"].Value!).Max();
            newItemBag["ID"].Value = maxId + 1;

            inventoryArray.Items.Add(newItemBag);
        }

        Console.WriteLine($"Writing to {targetFileName}...");
        SaveFile.Write(targetFileName, sf);
        Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in '{folder}' folder!");
    }
}
