using System.Diagnostics.CodeAnalysis;

namespace lib.remnant2.saves.Model.Properties;

public class TextProperty : ModelBase
{
    public required uint Flags;
    public required byte HistoryType;
    public required object Value;

    public TextProperty()
    {
    }

    [SetsRequiredMembers]
    public TextProperty(Reader r, SerializationContext ctx)
    {
        ReadOffset = r.Position + ctx.ContainerOffset;
        Flags = r.Read<uint>();
        HistoryType = r.Read<byte>();
        Value = HistoryType switch
        {
            0 => new TextPropertyData0(r, ctx),
            255 => new TextPropertyData255(r, ctx),
            _ => throw new InvalidOperationException("unsupported history type")
        };
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        w.Write(Flags);
        w.Write(HistoryType);
        switch (HistoryType)
        {
            case 0:
                ((TextPropertyData0)Value).Write(w, ctx);
                break;
            case 255:
                ((TextPropertyData255)Value).Write(w, ctx);
                break;
            default:
                throw new InvalidOperationException("unsupported history type");
        }
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }

    public override string? ToString()
    {
        return HistoryType switch
        {
            0 => ((TextPropertyData0)Value).SourceString,
            255 => ((TextPropertyData255)Value).Value,
            _ => throw new InvalidOperationException("unsupported history type")
        };
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        if (Value is ModelBase node )
            yield return (node, null);
    }
}
