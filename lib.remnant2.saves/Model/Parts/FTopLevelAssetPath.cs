using System.Diagnostics.CodeAnalysis;

namespace lib.remnant2.saves.Model.Parts;

public class FTopLevelAssetPath
{
    public required string? Path;
    public required string? Name;

    public FTopLevelAssetPath()
    {

    }

    [SetsRequiredMembers]
    public FTopLevelAssetPath(Reader r)
    {
        Path = r.ReadFString();
        Name = r.ReadFString();
    }

    public void Write(Writer w)
    {
        w.WriteFString(Path);
        w.WriteFString(Name);
    }
}
