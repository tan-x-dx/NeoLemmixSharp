using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers;

public sealed class ThemeReader : NeoLemmixDataReader
{
    private ThemeData _result;

    public ThemeReader(ThemeData result) : base("LEMMINGS", UnknownTokenBehaviour.IgnoreLine)
    {
        _result = result;

        SetNumberOfTokens(10);

        RegisterTokenAction("LEMMINGS", SetLemmingsTheme);
        RegisterTokenAction("NAMES_PLURAL", SetNamesPlural);
        RegisterTokenAction("NAMES_SINGULAR", SetNameSingular);
        RegisterTokenAction("MASK", SetMaskColor);
        RegisterTokenAction("MINIMAP", SetMinimapColor);
        RegisterTokenAction("BACKGROUND", SetBackgroundColor);
        RegisterTokenAction("ONE_WAYS", SetOneWayArrowColor);
        RegisterTokenAction("PICKUP_BORDER", SetPickupBorderColor);
        RegisterTokenAction("PICKUP_INSIDE", SetPickupInsideColor);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        return true;
    }

    private void SetLemmingsTheme(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (secondToken.Equals(IoConstants.DefaultStyleIdentifierString, StringComparison.OrdinalIgnoreCase))
        {
            var defaultTheme = StyleCache.DefaultStyleData.ThemeData;
            _result.LemmingSpriteData = defaultTheme.LemmingSpriteData;
            return;
        }
    }

    private void SetNamesPlural(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetNameSingular(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetMaskColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetMinimapColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetBackgroundColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetOneWayArrowColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetPickupBorderColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void SetPickupInsideColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        throw new NotImplementedException();
    }
}
