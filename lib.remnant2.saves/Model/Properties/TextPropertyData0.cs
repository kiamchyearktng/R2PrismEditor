using System.Diagnostics.CodeAnalysis;

namespace lib.remnant2.saves.Model.Properties;

public class TextPropertyData0 : ModelBase
{
    public required string? Namespace;
    public required string? Key;
    public required string? SourceString;

    public TextPropertyData0()
    {
    }

    [SetsRequiredMembers]
    public TextPropertyData0(Reader r, SerializationContext ctx)
    {
        ReadOffset = r.Position + ctx.ContainerOffset;
        Namespace = r.ReadFString();
        Key = r.ReadFString();
        SourceString = r.ReadFString();
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        w.WriteFString(Namespace);
        w.WriteFString(Key);
        w.WriteFString(SourceString);
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        yield break;
    }
}
