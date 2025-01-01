using System.Diagnostics.CodeAnalysis;
using lib.remnant2.saves.Model.Properties;

namespace lib.remnant2.saves.Model;

public class Component : ModelBase
{
    public required string ComponentKey;
    public PropertyBag? Properties;
    public Variables? Variables;
    public byte[]? ExtraComponentsData;

    public Component()
    {
    }

    [SetsRequiredMembers]
    public Component(string componentKey)
    {
        ComponentKey = componentKey;
    }
    public override IEnumerable<(ModelBase obj, int? index)> GetChildren()
    {
        if (Properties != null)
            yield return (Properties, null);
        if (Variables != null)
            yield return (Variables, null);
    }

    public override string ToString()
    {
        return ComponentKey;
    }
}
