using System.Runtime.InteropServices;

namespace prismeditor;

public class Utils
{
    private static readonly Guid SavedGamesGuid = new("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4");

    [DllImport("shell32.dll")]
    private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
        IntPtr hToken,
        out IntPtr pszPath);

    private static string GetSavePath()
    {
        IntPtr path = IntPtr.Zero;

        try
        {
            int hr = SHGetKnownFolderPath(SavedGamesGuid, 0, IntPtr.Zero, out path);
            Marshal.ThrowExceptionForHR(hr);
            return $@"{Marshal.PtrToStringUni(path)}\Remnant2";
        }
        finally
        {
            Marshal.FreeCoTaskMem(path);
        }
    }

    public static string GetSteamSavePath()
    {
        string? envPath = Environment.GetEnvironmentVariable("DEBUG_REMNANT_FOLDER");
        if (envPath != null) return envPath;
        string generalPath = GetSavePath();
        string[] possiblePaths = Directory.GetDirectories($@"{generalPath}\Steam");
        return possiblePaths[0];
    }

    public static string GetShortenedAssetPath(string path)
    {
        path = path[(path.LastIndexOfAny("/.".ToCharArray()) + 1)..];
        if (path.EndsWith("_C")) path = path[..^2];
        if (path.EndsWith("_UI")) path = path[..^2];
        path = path.Replace("_", " ").Trim();
        if (path.Contains(':')) path = path[(path.LastIndexOf(":", StringComparison.Ordinal) + 1)..];
        return $"'{path}'";
    }
}
