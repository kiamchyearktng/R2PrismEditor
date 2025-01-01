using System.Diagnostics.CodeAnalysis;
using lib.remnant2.saves.Model.Parts;

namespace lib.remnant2.saves.Model.Properties;
public class PropertyBag : ModelBase
{
    public required List<KeyValuePair<string, Property>> Properties;
    [NonSerialized]
    public Dictionary<string, Property> Lookup = [];

    public PropertyBag()
    {
    }

    [SetsRequiredMembers]
    public PropertyBag(Reader r, SerializationContext ctx)
    {
        ReadOffset = r.Position + ctx.ContainerOffset;
        Properties = [];
        while (true)
        {
            Property p = new(r, ctx);
            string name = p.Name.Name;
            if (name == "None")
            {
                break;
            }

            Properties.Add(new KeyValuePair<string, Property>(name,p));
            Lookup.TryAdd(name, p);
        }
        ReadLength = r.Position + ctx.ContainerOffset - ReadOffset;
    }

    public void Write(Writer w, SerializationContext ctx)
    {
        WriteOffset = (int)w.Position + ctx.ContainerOffset;
        foreach (KeyValuePair<string, Property> keyValuePair in Lookup)
        {
            keyValuePair.Value.Write(w, ctx);
        }

        ushort index = (ushort)ctx.GetNamesTableIndex("None");
        new Property { Name = new FName { Name = "None", Index = index, Number = null }}.Write(w, ctx);
        WriteLength = (int)w.Position + ctx.ContainerOffset - WriteOffset;
    }

    public Property this[string s] => Lookup[s];

    public bool Contains(string s)
    {
        return Lookup.ContainsKey(s);
    }

    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        for (int index = 0; index < Properties.Count; index++)
        {
            yield return (Properties[index].Value, index);
        }
    }

    public void RefreshLookup()
    {
        Lookup = [];
        foreach (KeyValuePair<string, Property> pair in Properties)
        {
            Lookup.Add(pair.Key,pair.Value);
        }
    }
}
