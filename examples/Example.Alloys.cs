using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{
    public static void Alloys()
    {
        Console.WriteLine("Alloys===========");
        const string alienId = "/Game/World_DLC3/Items/Special/AlienAlloy/Material_AlienAlloy.Material_AlienAlloy_C";
        const string mythrilId = "/Game/World_DLC3/Items/Special/MythrilAlloy/Material_MythrilAlloy.Material_MythrilAlloy_C";
        const string ancientId = "/Game/World_DLC3/Items/Special/AncientAlloy/Material_AncientAlloy.Material_AncientAlloy_C";
        const string scrapId = "/Game/World_Base/Items/Materials/Scraps/Material_Scraps.Material_Scraps_C";
        const string targetFileName = "alloys.sav";

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

        AlloysPatch(navigator, character, alienId, scrapId, 100);
        AlloysPatch(navigator, character, mythrilId, scrapId, 100);
        AlloysPatch(navigator, character, ancientId, scrapId, 100);

        Console.WriteLine($"Writing to {targetFileName}...");
        SaveFile.Write(targetFileName, sf);
        Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in '{folder}' folder!");
    }


    private static List<string>? FindNameTable(Node? n)
    {
        Node? cur = n;
        while (cur != null && cur.Object.GetType() != typeof(SaveData))
        {
            cur = cur.Parent;
        }

        return ((SaveData?)cur?.Object)?.NamesTable;
    }

    private static void AlloysPatch(Navigator navigator, ObjectProperty character, string currencyId, string scrapId, int value)
    {
        Property? shardItem = navigator.GetProperties("ItemBP", character.Object)
            .SingleOrDefault(x => x.Value!.ToString() == currencyId);

        Property scrapItem = navigator.GetProperties("ItemBP", character.Object)
            .Single(x => x.Value!.ToString() == scrapId);


        if (shardItem != null)
        {
            Console.WriteLine($"Your character has {currencyId}");
            var f = navigator.Lookup(shardItem);

            PropertyBag props = navigator.GetProperty("InstanceData", f.Parent!.Object)!.Get<ObjectProperty>().Object!.Properties!;

            if (!props.Contains("Quantity"))
            {
                Property p = new Property
                {
                    Name = FName.Create("Quantity", FindNameTable(f)!),
                    Type = FName.Create("IntProperty", FindNameTable(f)!),
                    Value = 1,
                    Size = 4,
                    Index = 0,
                    NoRaw = 0
                };
                props.Properties.Add(new KeyValuePair<string, Property>(p.Name.Name, p));
                props.RefreshLookup();
            }
            Property pp = props["Quantity"];
            Console.WriteLine($"Updating quantity from {pp.Value} to {value}");
            pp.Value = value;
        }
        else
        {
            Console.WriteLine("Your character does not have shards, adding");

            UObject itemObject = (scrapItem.Value as ObjectProperty)!.Object!.ShallowCopyObject(navigator);
            itemObject.ObjectPath = currencyId;

            UObject instanceDataObject =
                (scrapItem.GetParent<PropertyBag>(navigator)["InstanceData"].Value as ObjectProperty)!.Object!
                .ShallowCopyObject(navigator);
            instanceDataObject.LoadedData = instanceDataObject.LoadedData!.ShallowCopyLoadedData();
            instanceDataObject.Properties = new PropertyBag
            {
                Properties = instanceDataObject.Properties!.Properties
                    .Select(x => new KeyValuePair<string, Property>(x.Key, x.Value.ShallowCopyProperty())).ToList()
            };
            instanceDataObject.Properties.RefreshLookup();
            instanceDataObject.Properties["Quantity"].Value = value;

            PropertyBag newItemBag = new()
            {
                Properties = scrapItem.GetParent<PropertyBag>(navigator).Properties
                    .Select(x => new KeyValuePair<string, Property>(x.Key, x.Value.ShallowCopyProperty())).ToList(),
            };
            newItemBag.RefreshLookup();
            newItemBag["ItemBP"].Value = new ObjectProperty { ObjectIndex = itemObject.ObjectIndex };
            newItemBag["InstanceData"].Value = new ObjectProperty { ObjectIndex = instanceDataObject.ObjectIndex };
            newItemBag["New"].Value = 1; // Just for in-game display

            ArrayStructProperty inventoryArray =
                scrapItem.GetParent<PropertyBag>(navigator).GetParent<ArrayStructProperty>(navigator);
            int maxId = inventoryArray.Items.Select(x => (int)((PropertyBag)x!)["ID"].Value!).Max();
            newItemBag["ID"].Value = maxId + 1;

            inventoryArray.Items.Add(newItemBag);
        }
    }
}
