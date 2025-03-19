using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class PrimaryAnimationReader : NeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;

    public PrimaryAnimationReader(
        NeoLemmixGadgetArchetypeData gadgetArchetypeData)
        : base("$PRIMARY_ANIMATION")
    {
        _gadgetArchetypeData = gadgetArchetypeData;

        SetNumberOfTokens(9);

        RegisterTokenAction("FRAMES", SetFrameCount);
        RegisterTokenAction("OFFSET_X", SetOffsetX);
        RegisterTokenAction("OFFSET_Y", SetOffsetY);
        RegisterTokenAction("NINE_SLICE_TOP", SetNineSliceTop);
        RegisterTokenAction("NINE_SLICE_RIGHT", SetNineSliceRight);
        RegisterTokenAction("NINE_SLICE_BOTTOM", SetNineSliceBottom);
        RegisterTokenAction("NINE_SLICE_LEFT", SetNineSliceLeft);
        RegisterTokenAction("COLOR", SetColor);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        // Special handling for pickups specifically
        if (TokensMatch(firstToken, "NAME") && TokensMatch(firstToken, "*PICKUP"))
        {
            _gadgetArchetypeData.IsSkillPickup = true;
            return false;
        }

        var alternateLookup = _tokenActions.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(firstToken, out var tokenAction))
        {
            tokenAction(line, secondToken, secondTokenIndex);
        }
        else
        {
            NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
        }

        return false;
    }

    private void SetFrameCount(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.PrimaryAnimationFrameCount = int.Parse(secondToken);
    }

    private void SetOffsetX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.PrimaryAnimationOffsetX = int.Parse(secondToken);
    }

    private void SetOffsetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.PrimaryAnimationOffsetY = int.Parse(secondToken);
    }

    private void SetNineSliceTop(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType |= ResizeType.ResizeVertical;
    }

    private void SetNineSliceRight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
    }

    private void SetNineSliceBottom(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType |= ResizeType.ResizeVertical;
    }

    private void SetNineSliceLeft(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
    }

    private void SetColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        FinishedReading = true;
    }
}