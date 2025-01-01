using System.Diagnostics.CodeAnalysis;

namespace lib.remnant2.saves.Model.Properties;

public class TextPropertyData255 : ModelBase
{
    public required uint Flag;
    public string? Value;

    public TextPropertyData255()
    {
    }

    [SetsRequiredMembers]
    public TextPropertyData255(Reader r, SerializationContext ctx)
    {
        ReadOffset = r.Position + ctx.ContainerOffset;
        Flag = r.Read<uint>();
        if (Flag != 0)
        {
            Value = r.ReadFString();
        }
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        w.Write(Flag);
        if (Flag != 0)
        {
            w.WriteFString(Value!);
        }
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        yield break;
    }
}
