namespace lib.remnant2.saves;

internal static class Extensions
{
    public static string ToTypeString(this object? o)
    {
        return o == null ? "null" : $"{o.GetType()}";
    }
}
