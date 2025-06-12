namespace NeoLemmixSharp.IO.Reading;

internal readonly struct StringIdLookup(List<string> strings)
{
    private readonly List<string> _strings = strings;

    internal string this[int index] => _strings[index];
}

internal readonly struct MutableStringIdLookup
{
    private readonly List<string> _strings = [];

    internal int Count => _strings.Count;

    public MutableStringIdLookup()
    {
    }

    public static implicit operator StringIdLookup(MutableStringIdLookup lookup) => new(lookup._strings);

    internal void SetCapacity(int capacity) => _strings.Capacity = capacity;

    internal void Add(string s) => _strings.Add(s);
}
