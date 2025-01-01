using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{
    public static void AddRing()
    {
        Console.WriteLine("Add Ring===========");

        const string ringId = "/Game/World_Jungle/Items/Trinkets/Rings/ArchersCrest/Ring_ArchersCrest.Ring_ArchersCrest_C";
        const string targetFileName = "ring_added.sav";

        string folder = Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        Console.WriteLine("Reading profile data...");
        SaveFile sf = SaveFile.Read(path);
        Navigator navigator = new(sf);

        Console.WriteLine("Looking for the ring on the first character...");

        Property? characters = navigator.GetProperty("Characters");

        ObjectProperty? character = characters!.GetItems<ObjectProperty>().FirstOrDefault();
        if (character == null)
        {
            Console.WriteLine("Do you have any characters?");
            return;
        }

        Property? ringItem = navigator.GetProperties("ItemBP", character.Object)
            .SingleOrDefault(x => x.Value!.ToString() == ringId);

        if (ringItem != null)
        {
            Console.WriteLine("Your character already has the ring");
            return;
        }

        ringItem = navigator.GetProperties("ItemBP", character.Object)
            .FirstOrDefault(x => x.Value!.ToString()!.Contains("Ring_"));

        if (ringItem == null)
        {
            Console.WriteLine("Your character does not have any rings, we need an existing ring to clone from");
            return;
        }

        UObject itemObject = (ringItem.Value as ObjectProperty)!.Object!.
            ShallowCopyObject(navigator);
        UObject instanceDataObject = (ringItem.GetParent<PropertyBag>(navigator)["InstanceData"].Value as ObjectProperty)!.Object!.ShallowCopyObject(navigator);

        itemObject.ObjectPath = ringId;

        PropertyBag newItemBag = new()
        {
            Properties = ringItem.GetParent<PropertyBag>(navigator).Properties
                .Select(x => new KeyValuePair<string, Property>(x.Key, x.Value.ShallowCopyProperty())).ToList(),
        };
        newItemBag.RefreshLookup();

        newItemBag["ItemBP"].Value = new ObjectProperty { ObjectIndex = itemObject.ObjectIndex };
        newItemBag["InstanceData"].Value = new ObjectProperty { ObjectIndex = instanceDataObject.ObjectIndex };
        newItemBag["New"].Value = 1; // Just for in-game display
        newItemBag["EquipmentSlotIndex"].Value = -1; // In case ring we cloned from is equipped

        ArrayStructProperty inventoryArray = ringItem.GetParent<PropertyBag>(navigator).GetParent<ArrayStructProperty>(navigator);
        int maxId = inventoryArray.Items.Select(x => (int)((PropertyBag)x!)["ID"].Value!).Max();
        newItemBag["ID"].Value = maxId + 1;

        inventoryArray.Items.Add(newItemBag);

        Console.WriteLine($"Writing to {targetFileName}...");
        SaveFile.Write(targetFileName, sf);
        Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in the game save folder!");
    }
}
