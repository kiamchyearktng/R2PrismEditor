using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Model.Memory;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FileHeader
{
    public uint Crc32;
    public int DecompressedSize;
    public int Version;
    public int BuildNumber;
}
