using NeoLemmixSharp.IO.Data.Style;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat;

internal readonly ref struct NeoLemmixStyleReader : IStyleReader
{
    public NeoLemmixStyleReader(StyleIdentifier styleIdentifier)
    {

    }

    public StyleData ReadStyle()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
    }
}
