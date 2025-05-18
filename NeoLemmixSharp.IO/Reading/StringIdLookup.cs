namespace NeoLemmixSharp.IO.Reading;

internal readonly struct StringIdLookup
{
    private readonly List<string> _strings;

    internal int Count => _strings.Count;

    public StringIdLookup()
    {
        _strings = [];
    }

    internal void SetCapacity(int capacity) => _strings.Capacity = capacity;

    internal void Add(string s) => _strings.Add(s);

    internal string this[int index] => _strings[index];
}
