using lib.remnant2.saves;
using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace prismeditor.scripts
{
    public static partial class Scripts
    {
        public const string sourceFileName = "profile.json";
        public const string targetFileName = "profile.sav";

        internal static void ConsoleReseed()
        {
            Console.WriteLine("== Resetting prism seeds ==");

            string folder = Utils.GetSteamSavePath();
            string path = Path.Combine(folder, "profile.sav");

            Console.WriteLine("Reading profile data...");
            SaveFile sf = SaveFile.Read(path);
            Navigator navigator = new(sf);

            Property? characters = navigator.GetProperty("Characters");
            List<ObjectProperty>? characterList = characters!.GetItems<ObjectProperty>();
            //ObjectProperty character;
            if (characterList == null || characterList.Count == 0) {
                Console.WriteLine("No characters found");
                return;
            } else {
                //character = characterList[0];
            }

            List<Property>? seedPropertyList = navigator.GetProperties("CurrentSeed");
            List<PropertyBag> prismList = [];
            foreach (var property in seedPropertyList)
            {
                prismList.Add(property.GetParent<PropertyBag>(navigator));
            }

            var ra = new Random();
            foreach (var pbag in prismList)
            {
                Property currentSeed = pbag["CurrentSeed"];
                currentSeed.Value = ra.Next();

                if (pbag.Contains("PendingRoll"))
                {
                    Property pendingRoll = pbag["PendingRoll"];
                    pendingRoll.Value = 0;
                }
            }

            //List<Property>? test1 = navigator.GetProperties("CurrentSegments");
            //var test2 = test1[5].GetParent<PropertyBag>(navigator);
            //var test3 = test2.GetParent<UObject>(navigator);
            //var test4 = test3.GetParent<SaveData>(navigator);

            //Property currentExp = test3.Properties["PendingExperience"];
            //currentExp.Value = 1000f;

            //Property currentSegments = prismList[0]["CurrentSegments"];
            //PropertyBag segment = (PropertyBag)(currentSegments.Value as ArrayStructProperty).Items[0];
            //(segment["RowName"].Value as FName).Name = "RangedDamageIdealRange";

            //Property feedData = prismList[4]["CurrentFeedData"];
            //PropertyBag feedStat = (PropertyBag)(feedData.Value as ArrayStructProperty).Items[1];
            //(feedStat["RowName"].Value as FName).Name = "CriticalDamage";

            Console.WriteLine($"Writing to {targetFileName}...");
            SaveFile.Write(targetFileName, sf);
            Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in the game save folder!");
        }

        internal static void WriteFromJson()
        {
            Console.WriteLine("== Writing json to save ==");

            string steamSavePath = Utils.GetSteamSavePath();
            string profilePath = Path.Combine(steamSavePath, "profile.sav");

            Console.WriteLine($"Parsing {profilePath}...");
            SaveFile sf = SaveFile.Read(profilePath);

            // temp
            //string targetJsonPath = Path.GetFileName(Path.ChangeExtension(profilePath, "json"));

            Console.WriteLine($"Reading json from {sourceFileName}...");
            SaveData sd = JsonReadWrite.FromJson(sourceFileName);

            // ???
            sf.SaveData = sd;

            Console.WriteLine($"Writing to {targetFileName}...");
            SaveFile.Write(targetFileName, sf);
            Console.WriteLine($"You have to copy {targetFileName} over your profile.sav in the game save folder!");
        }

        //public static void Json()
        //{
        //    Console.WriteLine("Json===========");

        //    string folder = Utils.GetSteamSavePath();
        //    string profilePath = Path.Combine(folder, "profile.sav");
        //    string savePath = Path.Combine(folder, Environment.GetEnvironmentVariable("DEBUG_REMNANT_SAVE") ?? "save_0.sav");

        //    RoundtripJson(profilePath);
        //    RoundtripJson(savePath);
        //}

        internal static void GetJson()
        {
            string path = Path.Combine(Utils.GetSteamSavePath(), "profile.sav");
            Console.WriteLine($"Parsing {path}...");
            SaveFile sf = SaveFile.Read(path);

            string targetJsonPath = Path.GetFileName(Path.ChangeExtension(path, "json"));
            Console.WriteLine($"Writing json to {targetJsonPath}...");
            JsonReadWrite.ToJson(targetJsonPath, sf.SaveData);

            //Console.WriteLine("Writing original data to memory blob...");
            //Writer w = new();
            //sf.SaveData.Write(w);
            //byte[] original = w.ToArray();

            //Console.WriteLine($"Reading json from {targetJsonPath}...");
            //SaveData sd = JsonReadWrite.FromJson(targetJsonPath);

            //Console.WriteLine("Writing round-tripped data to memory blob...");
            //w = new();
            //sd.Write(w);
            //byte[] roundTripped = w.ToArray();

            //Console.WriteLine(original.SequenceEqual(roundTripped)
            //    ? "Written and read data is the same"
            //    : "Written and read data is different");
        }

        internal static void GetNamesTable()
        {
            string path = Path.Combine(Utils.GetSteamSavePath(), "profile.sav");
            Console.WriteLine($"Parsing {path}...");
            SaveFile sf = SaveFile.Read(path);

            var firstChar = sf.SaveData.Objects.First(x => x.Name == "SavedCharacter");
            var firstCharData = firstChar.Properties!["CharacterData"].Value as StructProperty;
            var firstCharNamesTable = (firstCharData!.Value as SaveData)!.NamesTable;
            
            foreach ( var firstCharName in firstCharNamesTable)
            {
                Console.WriteLine(firstCharName);
            }
            ;
        }

        internal static void ReadDataTable()
        {
            using FileStream stream = File.OpenRead("PrismStoneDataTable.json");
            var jsondata = JsonSerializer.Deserialize<List<RootDataTable>>(stream)![0];

            var segments = jsondata.Rows!.Where(x => x.Value is JsonObject y && y["PrismRequirementsToRoll"] == null).Select(x => x.Key).ToList();
            var segmentsstring = $"[{string.Join(", ", segments.Select(x => $"\"{x}\""))}]";
            Console.WriteLine(segmentsstring + "\n");

            var fusions = jsondata.Rows!.Where(x => x.Value is JsonObject y && y["PrismRequirementsToRoll"] != null).Select(x => x.Key).ToList();
            var fusionsstring = $"[{string.Join(", ", fusions.Select(x => $"\"{x}\""))}]";
            Console.WriteLine(fusionsstring + "\n");


            using FileStream stream2 = File.OpenRead("PrismStoneMythicDataTable.json");
            var jsondata2 = JsonSerializer.Deserialize<List<RootDataTable>>(stream2)![0];

            var legs = jsondata2.Rows!.Select(x => x.Key).ToList();
            var legsstring = $"[{string.Join(", ", legs.Select(x => $"\"{x}\""))}]";
            Console.WriteLine(legsstring + "\n");

            ;
            ;
        }
    }
}
