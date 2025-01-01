using System.Diagnostics.CodeAnalysis;
using lib.remnant2.saves.Model.Parts;
using Serilog;

namespace lib.remnant2.saves.Model.Properties;

public class EnumProperty : ModelBase
{
    public static ILogger Logger => Log.Logger.ForContext(Log.Category, Log.Parser).ForContext<EnumProperty>();
    public required FName EnumType;
    public required byte Unknown;
    public required FName EnumValue;

    public EnumProperty()
    {
    }

    [SetsRequiredMembers]
    public EnumProperty(Reader r, SerializationContext ctx)
    {
        ReadOffset = r.Position + ctx.ContainerOffset;
        EnumType = new(r, ctx.NamesTable);
        Unknown = r.Read<byte>();
        if (Unknown != 0)
        {
            Logger.Warning("unexpected non-zero value {value} of an unknown byte at {Offset}", Unknown, r.Position);
        }
        EnumValue = new(r, ctx.NamesTable);
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        EnumType.Write(w, ctx);
        w.Write(Unknown);
        EnumValue.Write(w, ctx);
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }

    public override string ToString()
    {
        return EnumValue.ToString();
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        yield break;
    }
}
