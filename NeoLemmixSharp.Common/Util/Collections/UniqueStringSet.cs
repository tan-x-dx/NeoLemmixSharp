namespace NeoLemmixSharp.Common.Util.Collections;

public sealed class UniqueStringSet
{
    private readonly HashSet<string> _uniqueStrings = new(32);

    public string GetUniqueStringInstance(ReadOnlySpan<char> chars)
    {
        chars = chars.Trim();
        var alternateLookup = _uniqueStrings.GetAlternateLookup<ReadOnlySpan<char>>();
        alternateLookup.Add(chars);
        alternateLookup.TryGetValue(chars, out var uniqueInstance);
        return uniqueInstance!;
    }
}
