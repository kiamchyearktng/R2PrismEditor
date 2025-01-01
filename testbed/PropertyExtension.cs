using lib.remnant2.saves.Model.Properties;

namespace prismeditor;
public static class PropertyExtension
{
    public static Property ShallowCopyProperty(this Property property)
    {
        Property result = new()
        {
            Name = property.Name,
            Index = property.Index,
            Size = property.Size,
            NoRaw = property.NoRaw,
            Type = property.Type,
            Value = property.Value
        };
        return result;
    }
}
