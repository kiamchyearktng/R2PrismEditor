using System.Text;
using lib.remnant2.saves.IO;

namespace lib.remnant2.saves;
public class Writer : WriterBase
{
    public void WriteFString(string? value)
    {
        if (value == null)
        {
            Write(0);
            return;
        }
        if (value.All(char.IsAscii))
        {
            Write(value.Length + 1);
            WriteBytes(Encoding.ASCII.GetBytes(value));
            Write<byte>(0);
        }
        else
        {
            Write(-2*(value.Length + 1));
            WriteBytes(Encoding.Unicode.GetBytes(value));
            Write<short>(0);
        }
    }
}
