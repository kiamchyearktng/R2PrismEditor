using Serilog;
using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Compression.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CompressedFileHeader
{
    public static ILogger Logger => Log.Logger
        .ForContext(Log.Category, Log.Compression)
        .ForContext<CompressedFileHeader>();


    public uint Crc32;
    public int DecompressedSize;
    public int Version;

    public readonly void DumpDebug()
    {
        Logger.Debug("Dumping CompressedFileHeader");
        Logger.Debug($"Crc32 {Crc32:X8}");
        Logger.Debug($"DecompressedSize {DecompressedSize}");
        Logger.Debug($"Version {Version}");
        Logger.Debug("--");
    }
}
