using lib.remnant2.saves.Model.Memory;
using lib.remnant2.saves.Model.Parts;

namespace examples;
public static class LoadedDataExtension
{
    public static UObjectLoadedData ShallowCopyLoadedData(this UObjectLoadedData data)
    {
        UObjectLoadedData result = new()
        {
            Name = new FName()
            {
                Name = data.Name.Name,
                Index =  data.Name.Index,
                Number = null
            },
            OuterId = data.OuterId
        };
        return result;
    }
}
