using System.Runtime.InteropServices;

namespace lib.remnant2.saves.Model.Memory;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FRotator
{
    public double Pitch;
    public double Roll;
    public double Yaw;
}
