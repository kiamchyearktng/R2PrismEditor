using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Model.Memory;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FGuid
{
    public uint A;
    public uint B;
    public uint C;
    public uint D;
}
