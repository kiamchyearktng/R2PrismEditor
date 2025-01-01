namespace lib.remnant2.saves.Model;
public abstract class ModelBase
{
    // These four properties are not used for any business logic, they are used in a test
    // to check that for each object that we write the offset and the length remained the
    // same as they were when we read them. For some objects - those that are stored in
    // fragmented manner, e.g UObject and Actor, length is quite arbitrary
    // If these four are removed with everything that references them the library will
    // still work
    public int ReadOffset;
    public int WriteOffset;
    public int ReadLength;
    public int WriteLength;
    public abstract IEnumerable<(ModelBase obj, int? index)> GetChildren();
}
