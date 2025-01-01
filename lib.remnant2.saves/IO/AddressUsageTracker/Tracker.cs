namespace lib.remnant2.saves.IO.AddressUsageTracker;

// This is a helper class for the ReaderBase to check if we have any unread gaps
public class Tracker
{
    private readonly SortedDictionary<int,AddressRange> _ranges = [];

    public Tracker() { }

    public Tracker(Tracker tracker)
    {
        _ranges = new SortedDictionary<int,AddressRange>(tracker._ranges);
    }

    public void AddRange(int start, int end)
    {
        _ranges.Add(start, new AddressRange { Begin = start, End = end });
        MergeRanges();
    }
    private void MergeRanges()
    {
        AddressRange? previous = null;
        // ToArray avoids the 'Collection was modified...' exception by making a copy
        foreach (int rangeStart in _ranges.Keys.ToArray())
        {
            if (previous == null || previous.End < rangeStart)
            {
                previous = _ranges[rangeStart];
                continue;
            }
            previous.End = _ranges[rangeStart].End;
            _ranges.Remove(rangeStart);
        }
    }
    public SortedDictionary<int, AddressRange> GetRanges()
    {
        return new SortedDictionary<int, AddressRange>(_ranges);
    }
}
