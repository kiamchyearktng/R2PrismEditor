using System.Diagnostics.CodeAnalysis;
using lib.remnant2.saves.Model.Memory;
using lib.remnant2.saves.Model.Parts;

namespace lib.remnant2.saves.Model;

public class PersistenceContainer : ModelBase
{
    public required uint Version;
    public required List<ulong> Destroyed;
    public required List<KeyValuePair<ulong, Actor>> Actors;

    public PersistenceContainer()
    {

    }

    [SetsRequiredMembers]
    public PersistenceContainer(Reader r, SerializationContext ctx, int containerOffset)
    {
        ReadOffset = r.Position + containerOffset;
        Version = r.Read<uint>();
        int indexOffset = r.Read<int>();
        int dynamicOffset = r.Read<int>();

        r.Position = indexOffset;
        uint infoCount = r.Read<uint>();
        List<FInfo> actorInfo = new((int)infoCount);
        for (uint i = 0; i < infoCount; i++)
        {
            actorInfo.Add(r.Read<FInfo>());
        }

        Destroyed = [];
        uint destroyedCount = r.Read<uint>();
        for (uint i = 0; i < destroyedCount; i++)
        {
            Destroyed.Add(r.Read<ulong>());
        }
        ReadLength = r.Position + containerOffset - ReadOffset;

        Actors = new List<KeyValuePair<ulong, Actor>>(actorInfo.Count);
        for (int index = 0; index < actorInfo.Count; index++)
        {
            FInfo info = actorInfo[index];
            r.Position = info.Offset;
            byte[] actorBytes = r.ReadBytes(info.Size);
            Reader actorReader = new(actorBytes);
            Actor a = new(actorReader, ctx, info.Offset + containerOffset);
            Actors.Add(new KeyValuePair<ulong, Actor>(info.UniqueID, a));
        }

        r.Position = dynamicOffset;
        uint dynamicCount = r.Read<uint>();
        for (uint i = 0; i < dynamicCount; i++)
        {
            ActorDynamicData da = new(r);
            Actor a = Actors.Single(x => x.Key == da.UniqueId).Value;
            a.DynamicData = da;
        }

    }
    
    public void Write(Writer w, int containerOffset)
    {
        WriteOffset = (int)w.Position + containerOffset;
        w.Write(Version);
        long patchOffset = (int)w.Position;
        w.Write(0); // index offset, unknown yet, will patch later
        w.Write(0); // dynamic offset, unknown yet, will patch later
        List<FInfo> actorInfo = [];
        foreach (KeyValuePair<ulong, Actor> a in Actors)
        {
            Writer actorWriter = new();
            a.Value.WriteNonDynamic(actorWriter, (int)w.Position+ containerOffset);
            byte []actorData = actorWriter.ToArray();
            actorInfo.Add(new()
            {
                UniqueID = a.Key,
                Offset = (int)w.Position,
                Size = actorData.Length
            });
            w.WriteBytes(actorData);
        }
        int dynamicOffset = (int)w.Position;
        w.Write(Actors.Count(x => x.Value.DynamicData != null));
        foreach (KeyValuePair<ulong, Actor> a in Actors)
        {
            a.Value.DynamicData?.Write(w);
        }
        int indexOffset = (int)w.Position;
        w.Write(actorInfo.Count);
        foreach (FInfo info in actorInfo)
        {
            w.Write(info);
        }
        w.Write(Destroyed.Count);
        foreach (ulong d in Destroyed)
        {
            w.Write(d);
        }

        long endOffset = w.Position;
        w.Position = patchOffset;
        w.Write(indexOffset);
        w.Write(dynamicOffset);
        w.Position = endOffset;
        WriteLength = (int)w.Position + containerOffset - WriteOffset;
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        for (int index = 0; index < Actors.Count; index++)
        {
            KeyValuePair<ulong, Actor> a = Actors[index];
            yield return (a.Value, index);
        }
    }
}
