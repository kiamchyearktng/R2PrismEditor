using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Model.Memory;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PackageVersion
{
    public uint UE4Version;
    public uint UE5Version;
}
