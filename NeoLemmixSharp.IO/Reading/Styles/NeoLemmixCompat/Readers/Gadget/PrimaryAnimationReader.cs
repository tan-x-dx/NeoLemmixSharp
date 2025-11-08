using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

internal sealed class PrimaryAnimationReader : NeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;
    private readonly NeoLemmixGadgetAnimationData _neoLemmixGadgetAnimationData = new();

    public PrimaryAnimationReader(
        NeoLemmixGadgetArchetypeData gadgetArchetypeData)
        : base("$PRIMARY_ANIMATION")
    {
        _gadgetArchetypeData = gadgetArchetypeData;

        SetNumberOfTokens(12);

        RegisterTokenAction("FRAMES", SetFrameCount);
        RegisterTokenAction("NAME", SetName);
        RegisterTokenAction("WIDTH", SetWidth);
        RegisterTokenAction("HEIGHT", SetHeight);
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
        if (TokensMatch(firstToken, "NAME") && TokensMatch(secondToken, "*PICKUP"))
        {
            _gadgetArchetypeData.IsSkillPickup = true;
            return false;
        }

        return ProcessTokenPair(line, firstToken, secondToken, secondTokenIndex);
    }

    private void SetFrameCount(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _neoLemmixGadgetAnimationData.FrameCount = int.Parse(secondToken);
    }

    private void SetName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {

    }

    private void SetWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.SpriteWidth = int.Parse(secondToken);
    }

    private void SetHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.SpriteHeight = int.Parse(secondToken);
    }

    private void SetOffsetX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _neoLemmixGadgetAnimationData.OffsetX = int.Parse(secondToken);
    }

    private void SetOffsetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _neoLemmixGadgetAnimationData.OffsetY = int.Parse(secondToken);
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
