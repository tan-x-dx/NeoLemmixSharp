namespace NeoLemmixSharp.Common.Util;

public ref struct LineSplitEnumerator
{
    private ReadOnlySpan<char> _input;

    public ReadOnlySpan<char> Current { readonly get; private set; }

    public LineSplitEnumerator(ReadOnlySpan<char> input)
    {
        _input = input;
        Current = default;
    }

    public LineSplitEnumerator GetEnumerator() => this;

    public bool MoveNext()
    {
        var span = _input;
        if (span.Length == 0)
            return false;

        var index = span.IndexOfAny('\r', '\n');
        if (index == -1)
        {
            _input = ReadOnlySpan<char>.Empty;
            Current = span;
            return true;
        }

        if (index < span.Length - 1 && span[index] == '\r')
        {
            var next = span[index + 1];
            if (next == '\n')
            {
                Current = span[..index];
                _input = span[(index + 2)..];
                return true;
            }
        }

        Current = span[..index];
        _input = span[(index + 1)..];
        return true;
    }
}