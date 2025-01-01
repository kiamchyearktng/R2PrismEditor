using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Model.Memory;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct OffsetInfo
{
    public long Names;
    public uint Version;
    public long Objects;
}
