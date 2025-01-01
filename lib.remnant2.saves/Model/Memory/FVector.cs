using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Model.Memory;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FVector
{
    public double X;
    public double Y;
    public double Z;
}
