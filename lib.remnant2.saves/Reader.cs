using System.Text;
using lib.remnant2.saves.IO;

namespace lib.remnant2.saves;

public class Reader(byte[] buffer) : ReaderBase(buffer)
{
    public string? ReadFString()
    {
        int len = Read<int>();
        switch (len)
        {
            case 0:
                return null;
            case < 0:
                return ReadUnicode(-len);
        }

        if (len + Position > Size)
        {
            throw new InvalidOperationException("FString length is too large");
        }

        string result = Encoding.ASCII.GetString(ReadBytes(len - 1));
        byte zero = Read<byte>();
        if (zero != 0)
        {
            throw new InvalidOperationException("did not read expected string terminator");
        }

        return result;
    }

    private string ReadUnicode(int len)
    {
        string result = Encoding.Unicode.GetString(ReadBytes((len - 1) * 2));
        short zero = Read<short>();
        if (zero != 0)
        {
            throw new InvalidOperationException("did not read expected string terminator");
        }

        return result;
    }
}
