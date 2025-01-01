using System.Diagnostics.CodeAnalysis;
using lib.remnant2.saves.Model.Memory;
using lib.remnant2.saves.Model.Parts;

namespace lib.remnant2.saves.Model;

public class SaveData : ModelBase
{
    // The members order roughly correspond to the order of data in a save file
    // after objects offset object properties and components go, one set for
    // each object in the following objects table
    public PackageVersion? PackageVersion;
    public FTopLevelAssetPath? SaveGameClassPath;
    public required long NameTableOffset;
    public required uint Version;
    public required long ObjectsOffset;
    public required List<UObject> Objects;
    public required List<string> NamesTable;

    public SaveData()
    {
    }

    [SetsRequiredMembers]
    public SaveData(Reader r, bool hasPackageVersion = true, bool hasTopLevelAssetPath = true, int containerOffset = 0, Options? opts = null) 
    {
        ReadOffset = r.Position + containerOffset;
        if (hasPackageVersion) PackageVersion = r.Read<PackageVersion>();

        if (hasTopLevelAssetPath) SaveGameClassPath = new FTopLevelAssetPath(r);

        OffsetInfo oi = r.Read<OffsetInfo>();

        NameTableOffset = oi.Names;
        Version = oi.Version;
        ObjectsOffset = oi.Objects;

        int objectsDataOffset = r.Position;

        int maxPosition = r.Position;
        r.Position = (int)NameTableOffset;

        int len = r.Read<int>();
        NamesTable = new List<string>(len);
        for (int i = 0; i < len; i++)
        {
            string name = r.ReadFString() ?? throw new InvalidOperationException("unexpected null entry in names table");
            NamesTable.Add(name);
        }

        SerializationContext ctx = new()
        {
            NamesTable = NamesTable,
            ClassPath = SaveGameClassPath?.Path,
            ContainerOffset = containerOffset,
            Options = opts
        };

        maxPosition = int.Max(maxPosition, r.Position);
        r.Position = (int)ObjectsOffset;

        int numUniqueObjects = r.Read<int>();
        Objects = new List<UObject>(numUniqueObjects);
        for (int i = 0; i < numUniqueObjects; i++)
        {
            UObject o = new(r, ctx, i);
            Objects.Add(o);
        }

        ctx.Objects = Objects;

        maxPosition = int.Max(maxPosition, r.Position);
        r.Position = objectsDataOffset;

        foreach (UObject o in Objects)
        {
            o.ReadData(r, ctx);
        }

        maxPosition = int.Max(maxPosition, r.Position);
        r.Position = maxPosition;
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;

    }

    public void Write(Writer w, int containerOffset = 0)
    {
        WriteOffset = (int)w.Position + containerOffset;

        if (PackageVersion != null)
        {
            w.Write(PackageVersion.Value);
        }

        SaveGameClassPath?.Write(w);

        OffsetInfo offsetInfo = new()
        {
            Version = Version
        };
        // We do not know offsets yet, we will need to patch this later
        long offsetPosition = w.Position;
        w.Write(offsetInfo);

        SerializationContext ctx = new()
        {
            NamesTable = NamesTable,
            ClassPath = SaveGameClassPath?.Path,
            Objects = Objects, // for ObjectProperty
            ContainerOffset = containerOffset
        };

        foreach (UObject o in Objects)
        {
            o.WriteData(w, ctx);
        }

        offsetInfo.Objects = w.Position;
        w.Write(Objects.Count);
        foreach (UObject o in Objects)
        {
            o.Write(w, ctx);
        }

        offsetInfo.Names = w.Position;
        w.Write(NamesTable.Count);
        foreach (string? item in NamesTable)
        {
            w.WriteFString(item);
        }

        long endOffset = w.Position;
        w.Position = offsetPosition;
        w.Write(offsetInfo);
        w.Position = endOffset;
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        for (int index = 0; index < Objects.Count; index++)
        {
            UObject o = Objects[index];
            yield return (o, index);
        }
    }
}
