using System.Runtime.CompilerServices;

namespace lib.remnant2.saves.IO;

public  class WriterBase
{
    public void Write<T>(T value)
    {

        byte[] buf = new byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref buf[0], value);
        WriteBytes(buf);
    }
    public void WriteBytes(byte[] value)
    {
        Stream.Write(new ReadOnlySpan<byte>(value));
    }

    public byte[] ToArray()
    {
        return Stream.ToArray();
    }

    public long Position
    {
        get => Stream.Position;
        set => Stream.Position = value;
    }

    public MemoryStream Stream { get; } = new();
}
