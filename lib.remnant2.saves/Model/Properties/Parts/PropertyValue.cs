using System.Globalization;
using System.Numerics;
using lib.remnant2.saves.Model.Memory;
using lib.remnant2.saves.Model.Parts;

namespace lib.remnant2.saves.Model.Properties.Parts;

public class PropertyValue
{
    public required object? Value;
    public byte? NoRawByte;

    // This is also called from MapProperty and ArrayProperty constructor
    public static PropertyValue ReadPropertyValue(Reader r, SerializationContext ctx, string type, bool isRaw = true)
    {
        return type switch
        {
            "IntProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<int>() },
            "Int16Property" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<short>() },
            "Int64Property" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<long>() },
            "UInt64Property" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<ulong>() },
            "FloatProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<float>() },
            "DoubleProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<double>() },
            "UInt16Property" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<ushort>() },
            "UInt32Property" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.Read<uint>() },
            "StrProperty" or "SoftClassPath" or "SoftObjectProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = r.ReadFString() },
            "BoolProperty" => new PropertyValue { Value = r.Read<byte>(), NoRawByte = isRaw ? null : r.Read<byte>() },
            "NameProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = new FName(r, ctx.NamesTable) },

            "ByteProperty" => new PropertyValue { NoRawByte = null, Value = isRaw ? r.Read<byte>() : new ByteProperty(r, ctx) },
            "StructProperty" => new PropertyValue { NoRawByte = null, Value = isRaw ? r.Read<FGuid>() : new StructProperty(r, ctx) },
            "ObjectProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = new ObjectProperty(r, ctx) },
            "EnumProperty" => new PropertyValue { NoRawByte = null, Value = new EnumProperty(r, ctx) },
            //if (isRaw) { throw new InvalidOperationException("Raw map properties are not supported"); }
            "MapProperty" => new PropertyValue { NoRawByte = null, Value = new MapProperty(r, ctx) },
            "TextProperty" => new PropertyValue { NoRawByte = isRaw ? null : r.Read<byte>(), Value = new TextProperty(r, ctx) },
            "ArrayProperty" => new PropertyValue { NoRawByte = null, Value = ReadArrayProperty(r, ctx) },
            _ => throw new InvalidOperationException($"unknown property type {type}")
        };
    }

    private static T Get<T>(object? value)
    {
        if (value is BigInteger z)
        {
            value = (ulong)(z & ulong.MaxValue);
        }
        return (T)Convert.ChangeType(value!, typeof(T), CultureInfo.InvariantCulture);
    }
    private static object ReadArrayProperty(Reader r, SerializationContext ctx)
    {
        int readOffset = r.Position + ctx.ContainerOffset;
        FName elementType = new(r, ctx.NamesTable);
        byte unknown = r.Read<byte>();
        uint count = r.Read<uint>();
        return elementType.Name == "StructProperty"
            ? new ArrayStructProperty(r, ctx, count, unknown, elementType, readOffset)
            : new ArrayProperty(r, ctx, count, unknown, elementType, readOffset);
    }

    public static void WritePropertyValue(Writer w, SerializationContext ctx, object? value, string type, byte? noRow = null)
    {
        switch (type)
        {
            case "IntProperty":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<int>(value));
                break;
            case "Int16Property":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<short>(value));
                break;
            case "Int64Property":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<long>(value));
                break;
            case "UInt64Property":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<ulong>(value));
                break;
            case "FloatProperty":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<float>(value));
                break;
            case "DoubleProperty":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<double>(value));
                break;
            case "UInt16Property":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<ushort>(value));
                break;
            case "UInt32Property":
                if (noRow != null) w.Write(noRow.Value);
                w.Write(Get<uint>(value));
                break;
            case "SoftClassPath":
            case "SoftObjectProperty":
            case "StrProperty":
                if (noRow != null) w.Write(noRow.Value);
                w.WriteFString(value as string);
                break;
            case "BoolProperty":
                w.Write(Get<byte>(value));
                if (noRow != null) w.Write(noRow.Value);
                break;
            case "NameProperty":
                if (noRow != null) w.Write(noRow.Value);
                ((FName)value!).Write(w, ctx);
                break;
            case "ByteProperty":
                if (noRow != null) { ((ByteProperty)value!).Write(w, ctx); } else { w.Write((byte)value!); }
                break;
            case "StructProperty":
                if (noRow != null) { ((StructProperty)value!).Write(w, ctx); } else { w.Write((FGuid)value!); }
                break;
            case "ObjectProperty":
                if (noRow != null) w.Write(noRow.Value);
                ((ObjectProperty)value!).Write(w, ctx);
                break;
            case "EnumProperty":
                ((EnumProperty)value!).Write(w, ctx);
                break;
            case "MapProperty":
                //if (noRaw == null) { throw new InvalidOperationException("Raw map properties are not supported"); }
                ((MapProperty)value!).Write(w, ctx);
                break;
            case "TextProperty":
                if (noRow != null) w.Write(noRow.Value);
                ((TextProperty)value!).Write(w, ctx);
                break;
            case "ArrayProperty":
                WriteArrayProperty(w, ctx, value!);
                break;
            default:
                throw new InvalidOperationException($"unknown property type {type}");
        }
    }

    private static void WriteArrayProperty(Writer w, SerializationContext ctx, object value)
    {
        switch (value)
        {
            case ArrayStructProperty property:
                {
                    property.Write(w, ctx);
                    break;
                }
            case ArrayProperty property:
                {
                    property.Write(w, ctx);
                    break;
                }
            default:
                throw new InvalidOperationException("unexpected array type");
        }
    }
}
