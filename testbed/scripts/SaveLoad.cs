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
        private static string? _steamSavePath = null;
        public static string steamSavePath {
            get {
                _steamSavePath ??= Utils.GetSteamSavePath();
                return _steamSavePath;
            }
        }

        public static async Task<(int status, SaveFile? sf, Navigator? nav)> LoadSteam(Action<string>? logFunction = null)
        {
            try
            {
                logFunction?.Invoke("Loading from steam saves");
                SaveFile? saveFile = null;
                Navigator? navigator = null;
                await Task.Run(() =>
                {
                    string folder = steamSavePath;
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

        public static async Task<(int status, SaveFile? sf, Navigator? nav)> LoadStatic(Action<string>? logFunction = null)
        {
            try
            {
                logFunction?.Invoke("Loading from current location");
                SaveFile? saveFile = null;
                Navigator? navigator = null;
                await Task.Run(() =>
                {
                    saveFile = SaveFile.Read("profile.sav");
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

        public static async Task<(int status, string filepath)> SaveSteam(SaveFile saveFile, Action<string>? logFunction = null)
        {
            try
            {
                logFunction?.Invoke("Saving to steam saves");
                await Task.Run(() =>
                {
                    string folder = steamSavePath;
                    string path = Path.Combine(folder, staticFileName);
                    SaveFile.Write(path, saveFile);
                });

                string filepath = Path.Combine(steamSavePath, staticFileName);
                logFunction?.Invoke($"Saved at {filepath}");
                return (0, filepath);
            }
            catch (Exception ex)
            {
                logFunction?.Invoke($"Error ({ex.GetType()}): {ex.Message}");
                return (-1, "");
            }
        }

        public static async Task<(int status, string filepath)> SaveStatic(SaveFile saveFile, Action<string>? logFunction = null)
        {
            try
            {
                logFunction?.Invoke("Saving at current location");
                await Task.Run(() => { SaveFile.Write(staticFileName, saveFile); });

                string filepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", staticFileName);
                logFunction?.Invoke($"Saved at {filepath}");
                return (0, filepath);
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