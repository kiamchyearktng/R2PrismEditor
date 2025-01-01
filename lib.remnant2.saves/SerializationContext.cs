using lib.remnant2.saves.Model;

namespace lib.remnant2.saves;

public class SerializationContext
{
    public required List<string> NamesTable;
    // Required when reading UObject and also when reading Persistence Blob
    // so we could decode individual character data, while reading profile save
    // This can be null on during nested SaveData reads
    public required string? ClassPath;
    // This is used so we could populate ObjectProperty with object path for display purposes
    // This is also used during writing of ObjectProperty to update object index
    public List<UObject>? Objects;

    // This is used during read/write in a nested container
    // to keep track of outermost file offsets
    public int ContainerOffset;

    public Options? Options;

    // Caching NameTable entries for faster access
    private readonly Dictionary<string, int> _namesTableIndex = [];
    private readonly object _lock = new();

    public int GetNamesTableIndex(string name)
    {
        if (!_namesTableIndex.ContainsKey(name))
        {
            lock (_lock)
            {
                _namesTableIndex.TryAdd(name, NamesTable.FindIndex(x => x == name));
            }
        }
        return _namesTableIndex[name];
    }
}
