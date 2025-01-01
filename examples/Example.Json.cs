using lib.remnant2.saves;
using lib.remnant2.saves.Model;

namespace examples;
internal partial class Example
{
    public static void Json()
    {
        Console.WriteLine("Json===========");

        string folder = Utils.GetSteamSavePath();
        string profilePath = Path.Combine(folder, "profile.sav");
        string savePath = Path.Combine(folder, Environment.GetEnvironmentVariable("DEBUG_REMNANT_SAVE") ?? "save_0.sav");

        RoundtripJson(profilePath);
        RoundtripJson(savePath);
    }

    private static void RoundtripJson(string path)
    {
        Console.WriteLine($"Parsing {path}...");
        SaveFile sf = SaveFile.Read(path);

        string targetJsonPath = Path.GetFileName(Path.ChangeExtension(path, "json"));
        Console.WriteLine($"Writing json to {targetJsonPath}...");
        JsonReadWrite.ToJson(targetJsonPath, sf.SaveData);

        Console.WriteLine("Writing original data to memory blob...");
        Writer w = new();
        sf.SaveData.Write(w);
        byte[] original = w.ToArray();

        Console.WriteLine($"Reading json from {targetJsonPath}...");
        SaveData sd = JsonReadWrite.FromJson(targetJsonPath);

        Console.WriteLine("Writing round-tripped data to memory blob...");
        w = new();
        sd.Write(w);
        byte[] roundTripped = w.ToArray();

        Console.WriteLine(original.SequenceEqual(roundTripped)
            ? "Written and read data is the same"
            : "Written and read data is different");
    }
}
