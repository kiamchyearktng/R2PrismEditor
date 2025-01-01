using lib.remnant2.saves;
using lib.remnant2.saves.Model;
using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties;
using lib.remnant2.saves.Navigation;
using prismeditor.definitions;
using System.IO;
using System.Reflection;

namespace prismeditor.scripts
{
    public static partial class Scripts
    {
        public const string staticFileName = "profile.sav";

        public static async Task<(int status, SaveFile? sf, Navigator? nav)> LoadSteam(Action<string>? logFunction = null)
        {
            try
            {
                logFunction?.Invoke("Loading from steam saves");
                SaveFile? saveFile = null;
                Navigator? navigator = null;
                await Task.Run(() =>
                {
                    string folder = Utils.GetSteamSavePath();
                    string path = Path.Combine(folder, "profile.sav");
                    saveFile = SaveFile.Read(path);
                    navigator = new Navigator(saveFile);
                });
                logFunction?.Invoke("Load successful");
                return (0, saveFile, navigator);
            }
            catch (Exception ex)
            {
                logFunction?.Invoke($"Error ({ex.GetType()}): {ex.Message}");
                return (-1, null, null);
            }
        }

        public static async Task<(int status, string filepath)> SaveStatic(SaveFile saveFile, Action<string>? logFunction = null)
        {
            try
            {
                await Task.Run(() => { SaveFile.Write(staticFileName, saveFile); });

                return (0, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", staticFileName));
            }
            catch (Exception ex)
            {
                logFunction?.Invoke($"Error ({ex.GetType()}): {ex.Message}");
                return (-1, "");
            }
        }

        public static async Task<(int status, List<PrismData>? prismDatas)> GetPrisms(Navigator navigator, Action<string>? logFunction = null)
        {
            try
            {
                Property? characters;
                List<ObjectProperty>? characterList = null;
                await Task.Run(() =>
                {
                    characters = navigator!.GetProperty("Characters");
                    characterList = characters!.GetItems<ObjectProperty>();
                });
                if (characterList == null || characterList.Count == 0)
                {
                    logFunction?.Invoke("No characters found");
                    return (1, null);
                }
                logFunction?.Invoke($"{characterList.Count} characters found");

                List<PrismData> prisms = [];

                await Task.Run(() =>
                {
                    for (int i = 0; i < characterList.Count; i++)
                    {
                        List<Property>? seedPropertyList = navigator.GetProperties("CurrentSeed", characterList[i].Object);

                        for (int j = 0; j < seedPropertyList.Count; j++)
                        {
                            prisms.Add(new(seedPropertyList[j].GetParent<PropertyBag>(navigator))
                            {
                                charIndex = i,
                                prismIndex = j,
                            });
                        }
                    }
                });

                return (0, prisms);
            }
            catch (Exception ex)
            {
                logFunction?.Invoke($"Failed ({ex.GetType()}): {ex.Message}");
                return (-1, null);
            }
        }
    }
}