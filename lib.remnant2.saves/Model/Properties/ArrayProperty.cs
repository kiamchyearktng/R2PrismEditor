using System.Diagnostics.CodeAnalysis;
using lib.remnant2.saves.Model.Parts;
using lib.remnant2.saves.Model.Properties.Parts;
using Serilog;

namespace lib.remnant2.saves.Model.Properties;

public class ArrayProperty : ModelBase
{
    public static ILogger Logger => Log.Logger.ForContext(Log.Category, Log.Parser).ForContext<ArrayProperty>();

    public required FName ElementType;
    public required List<object?> Items;
    public required byte Unknown;

    public ArrayProperty()
    {
    }

    [SetsRequiredMembers]
    public ArrayProperty(Reader r, SerializationContext ctx, uint count, byte unknown, FName elementType, int readOffset) 
    {
        ReadOffset = readOffset;
        Unknown = unknown;
        if (Unknown != 0)
        {
            Logger.Warning("unexpected non-zero value {value} of an unknown byte at {Offset}", Unknown,  r.Position);
        }
        ElementType = elementType;
        Items = new List<object?>((int)count);
        for (int i = 0; i < count; i++)
        {
            object o = PropertyValue.ReadPropertyValue(r, ctx, ElementType.Name).Value!;
            Items.Add(o);
        }
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        ElementType.Write(w, ctx);
        w.Write(Unknown);
        w.Write(Items.Count);
        foreach (object? item in Items) PropertyValue.WritePropertyValue(w, ctx, item, ElementType.Name);
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        for (int index = 0; index < Items.Count; index++)
        {
            object? item = Items[index];
            if (item is ModelBase node)
                yield return (node, index);
        }
    }
}
