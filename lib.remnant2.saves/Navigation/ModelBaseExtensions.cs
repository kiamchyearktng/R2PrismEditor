using lib.remnant2.saves.Model;

namespace lib.remnant2.saves.Navigation;
public static class ModelBaseExtension
{
    public static T GetParent<T>(this ModelBase obj, Navigator navigator) where T : ModelBase
    {
        return (T)navigator.Lookup(obj).Parent!.Object;
    }
}
