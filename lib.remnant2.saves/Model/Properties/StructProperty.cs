using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using lib.remnant2.saves.IO;
using lib.remnant2.saves.Model.Memory;
using lib.remnant2.saves.Model.Parts;
using Serilog;

namespace lib.remnant2.saves.Model.Properties;

public class StructProperty : ModelBase
{
    public static ILogger Logger => Log.Logger.ForContext(Log.Category, Log.Parser).ForContext<StructProperty>();
    public required FName Type;
    public required FGuid Guid;
    public required byte Unknown;
    public required object? Value;

    public StructProperty()
    {
    }

    [SetsRequiredMembers]
    public StructProperty(Reader r, SerializationContext ctx)
    {
        ReadOffset = r.Position + ctx.ContainerOffset;
        Type = new(r, ctx.NamesTable);
        Guid = r.Read<FGuid>();
        Unknown = r.Read<byte>();
        if (Unknown != 0)
        {
            Logger.Warning("unexpected non-zero value {value} of an unknown byte at {Offset}", Unknown, r.Position);
        }
        Value = ReadStructPropertyValue(r, ctx, Type.Name);
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    // Also called by ArrayStructProperty constructor
    public static object? ReadStructPropertyValue(Reader r, SerializationContext ctx, string type)
    {
        return type switch
        {
            "SoftClassPath" or "SoftObjectPath" => r.ReadFString(),
            "Timespan" => new TimeSpan(r.Read<long>()),
            "Guid" => r.Read<FGuid>(),
            "Vector" => r.Read<FVector>(),
            "Rotator" => r.Read<FRotator>(),
            "DateTime" => new DateTime(r.Read<long>()),
            "PersistenceBlob" => ReadPersistenceBlob(r, ctx),
            _ => new PropertyBag(r,ctx)
        };
    }
    private static object ReadPersistenceBlob(ReaderBase r, SerializationContext ctx)
    {
        int persistenceSize = r.Read<int>();
        int containerOffset = r.Position;
        byte[] bytes = r.ReadBytes(persistenceSize);
        Reader persistenceReader = new(bytes);

        if (ctx.ClassPath == "/Game/_Core/Blueprints/Base/BP_RemnantSaveGameProfile")
        {
            return new SaveData(persistenceReader, true, false, containerOffset,ctx.Options);
        }
        return new PersistenceContainer(persistenceReader, ctx,containerOffset);
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        Type.Write(w, ctx);
        w.Write(Guid);
        w.Write(Unknown);
        WriteStructPropertyValue(w, ctx, Type.Name, Value);
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }

    public static void WriteStructPropertyValue(Writer w, SerializationContext ctx, string type, object? value)
    {
        switch (type)
        {
            case "SoftClassPath":
            case "SoftObjectPath":
                w.WriteFString(value as string);
                break;
            case "Timespan":
                w.Write(GetTimeSpan(value).Ticks);
                break;
            case "Guid":
                w.Write((FGuid)value!);
                break;
            case "Vector":
                w.Write((FVector)value!);
                break;
            case "Rotator":
                w.Write((FRotator)value!);
                break;
            case "DateTime":
                w.Write(GetDateTime(value).Ticks);
                break;
            case "PersistenceBlob":
                WritePersistenceBlob(w, ctx, value!);
                break;
            default:
                ((PropertyBag)value!).Write(w,ctx);
                break;
        }
    }

    private static void WritePersistenceBlob(Writer w, SerializationContext ctx, object value)
    {
        Writer persistenceWriter = new();
        if (ctx.ClassPath == "/Game/_Core/Blueprints/Base/BP_RemnantSaveGameProfile")
        {
            ((SaveData)value).Write(persistenceWriter,(int)w.Position + ctx.ContainerOffset+4);
        }
        else
        {
            ((PersistenceContainer)value).Write(persistenceWriter, (int)w.Position + ctx.ContainerOffset+4);
        }

        byte[] data = persistenceWriter.ToArray();
        w.Write(data.Length);
        w.WriteBytes(data);
    }

    private static TimeSpan GetTimeSpan(object? value)
    {
        return value switch
        {
            null => throw new InvalidOperationException("expected timestamp got null"),
            string timespan => TimeSpan.Parse(timespan, CultureInfo.InvariantCulture),
            _ => (TimeSpan)value
        };
    }
    private static DateTime GetDateTime(object? value)
    {
        return value switch
        {
            null => throw new InvalidOperationException("expected datetime got null"),
            string timespan => DateTime.Parse(timespan, CultureInfo.InvariantCulture),
            _ => (DateTime)value
        };
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        if (Value is ModelBase node)
            yield return (node, null);
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }
}
