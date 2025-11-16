using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers;

public sealed class ThemeReader : NeoLemmixDataReader
{
    private ThemeData _themeData;

    public ThemeReader(ThemeData themeData) : base("LEMMINGS", UnknownTokenBehaviour.IgnoreLine)
    {
        _themeData = themeData;

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

    public override bool ShouldProcessSection(ReadOnlySpan<char> token) => true;

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        return true;
    }

    private void SetLemmingsTheme(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (Helpers.StringSpansMatch(secondToken, IoConstants.DefaultStyleIdentifierString))
        {
            var defaultTheme = StyleCache.DefaultStyleData.ThemeData;
            _themeData.LemmingSpriteData = defaultTheme.LemmingSpriteData;
            return;
        }
    }

    private void SetNamesPlural(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetNameSingular(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetMaskColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var color = NxlvReadingHelpers.ParseColor(secondToken);
        _themeData.Mask = color;
    }

    private void SetMinimapColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var color = NxlvReadingHelpers.ParseColor(secondToken);
        _themeData.Minimap = color;
    }

    private void SetBackgroundColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var color = NxlvReadingHelpers.ParseColor(secondToken);
        _themeData.Background = color;
    }

    private void SetOneWayArrowColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var color = NxlvReadingHelpers.ParseColor(secondToken);
        _themeData.OneWayArrows = color;
    }

    private void SetPickupBorderColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var color = NxlvReadingHelpers.ParseColor(secondToken);
        _themeData.PickupBorder = color;
    }

    private void SetPickupInsideColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var color = NxlvReadingHelpers.ParseColor(secondToken);
        _themeData.PickupInside = color;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        FinishedReading = true;
    }
}
