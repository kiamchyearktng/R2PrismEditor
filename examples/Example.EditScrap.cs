using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{
    public static void EditScrap()
    {
        Console.WriteLine("Edit Scrap===========");

        const string scrapId = "/Game/World_Base/Items/Materials/Scraps/Material_Scraps.Material_Scraps_C";
        const int targetScrapValue = 12345;
        const string targetFileName = "scrap_changed.sav";

        string folder = Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        Console.WriteLine("Reading profile data...");
        SaveFile sf = SaveFile.Read(path);
        Navigator navigator = new(sf);

        Console.WriteLine("Looking for scrap on the first character...");

        Property? characters = navigator.GetProperty("Characters");

        ObjectProperty? character = characters!.GetItems<ObjectProperty>().FirstOrDefault();
        if (character == null)
        {
            Console.WriteLine("Do you have any characters?");
            return;
        }

        Property scrapItem = navigator.GetProperties("ItemBP", character.Object)
            .Single(x => x.Value is ObjectProperty { ClassName: scrapId });

        UObject instanceData = (scrapItem.GetParent<PropertyBag>(navigator)["InstanceData"].Value as ObjectProperty)!.Object!;

        Property scrap = navigator.GetProperty("Quantity", instanceData)!;

        Console.WriteLine($"Current scrap value is {scrap.Value}. Changing to {targetScrapValue}...");
        scrap.Value = targetScrapValue;

        Console.WriteLine($"Writing to {targetFileName}...");
        SaveFile.Write(targetFileName, sf);
        Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in the game save folder!");
    }
}
