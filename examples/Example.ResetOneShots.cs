using lib.remnant2.saves.Model;
using lib.remnant2.saves.Navigation;

namespace examples;

internal partial class Example
{
    public static void ResetOneShots()
    {
        Console.WriteLine("Reset One Shots===========");
        
        const string targetFileName = "resetoneshots.sav";

        string folder = Utils.GetSteamSavePath();
        string file = Environment.GetEnvironmentVariable("DEBUG_REMNANT_SAVE") ?? "save_0.sav";
        string savePath = Path.Combine(folder, file);

        SaveFile sf = SaveFile.Read(savePath);
        Navigator navigator = new(sf);


        UObject x = navigator.GetObject("BP_RemnantSaveGame_C")!;

        if (!x.Properties!.Contains("RolledOneShots"))
        {
            Console.WriteLine("This save does not have rolled one shots");
            return;
        }

        x.Properties!.Properties.RemoveAll(y => y.Key == "RolledOneShots");
        x.Properties!.RefreshLookup();

        Console.WriteLine($"Writing to {targetFileName}...");
        SaveFile.Write(targetFileName, sf);
        Console.WriteLine($"You have to copy {targetFileName} over your {file} in '{folder}' folder!");

    }
}
