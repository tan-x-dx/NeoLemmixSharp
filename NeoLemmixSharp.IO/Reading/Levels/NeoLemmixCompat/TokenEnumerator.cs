namespace NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat;

public ref struct TokenEnumerator
{
    private readonly ReadOnlySpan<char> _input;
    private int _index;
    private int _spanStart;
    private int _spanLength;

    public TokenEnumerator(ReadOnlySpan<char> input)
    {
        _input = input;
        _index = 0;
        _spanStart = -1;
        _spanLength = -1;
    }

    public bool MoveNext()
    {
        _spanStart = -1;

        while (_index < _input.Length)
        {
            var c = _input[_index++];

            if (char.IsWhiteSpace(c))
            {
                if (_spanStart == -1)
                    continue;

                return true;
            }

            if (_spanStart == -1)
            {
                _spanStart = _index - 1;
                _spanLength = 1;
            }
            else
            {
                _spanLength++;
            }
        }

        if (_spanStart == -1)
            return false;

        _spanLength = _input.Length - _spanStart;
        return true;
    }

    public readonly ReadOnlySpan<char> Current => _input.Slice(_spanStart, _spanLength);
    public readonly int CurrentSpanStart => _spanStart;
}
