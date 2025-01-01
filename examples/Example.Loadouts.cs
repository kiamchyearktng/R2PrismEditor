using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{
    public static void Loadouts()
    {
        Console.WriteLine("Loadouts ===========");

        string folder = Utils.GetSteamSavePath();
        string path = Path.Combine(folder, "profile.sav");

        SaveFile sf = SaveFile.Read(path);
        Navigator navigator = new(sf);

        var chars = navigator.GetObjects("SavedCharacter");
        for (int charIndex = 0; charIndex < chars.Count; charIndex++)
        {
            var character = chars[charIndex];
            Console.WriteLine($"  Character index {charIndex}");
            Property? lr = navigator.GetProperty("LoadoutRecords", character);
            if (lr != null)
            {
                List<Property> ls = navigator.GetProperties("Entries", lr);
                for (int loadoutIndex = 0; loadoutIndex < ls.Count; loadoutIndex++)
                {
                    Console.WriteLine($"    Loadout index {loadoutIndex}");
                    ArrayStructProperty asp = ls[loadoutIndex].Get<ArrayStructProperty>();
                    if (asp.Items.Count == 0)
                    {
                        Console.WriteLine("      Empty");
                        continue;
                    }

                    foreach (object? aspItem in asp.Items)
                    {
                        PropertyBag pb = (PropertyBag)aspItem!;

                        int level = pb["Level"].Get<int>();
                        int index = pb["Index"].Get<int>();
                        string item = Utils.GetShortenedAssetPath(pb["ItemClass"].Get<string>());
                        string slot = Utils.GetShortenedAssetPath(pb["Slot"].Get<ObjectProperty>().ClassName!);

                        if (index != -1) slot = $"{slot}, index:{index}";
                        string levelStr = level > 0 ? $"; level - {level}" : "";

                        Console.WriteLine($"      {item}({slot}){levelStr}");
                    }
                }
            }
            else
            {
                Console.WriteLine("    No loadouts found");
            }
        }
    }
}