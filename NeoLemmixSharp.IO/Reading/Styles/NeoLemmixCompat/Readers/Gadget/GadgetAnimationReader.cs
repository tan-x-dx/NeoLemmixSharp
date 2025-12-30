using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.GadgetData;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

internal sealed class GadgetAnimationReader : NeoLemmixDataReader
{
    private static readonly Dictionary<string, NeoLemmixGadgetCondition> _gadgetStateTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "READY", NeoLemmixGadgetCondition.Idle },
        { "BUSY", NeoLemmixGadgetCondition.Active },
        { "DISABLED", NeoLemmixGadgetCondition.Disabled },
        { "EXHAUSTED", NeoLemmixGadgetCondition.Exhausted }
    };

    private static readonly Dictionary<string, NeoLemmixStateType> _gadgetSecondaryAnimationActionLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "PLAY", NeoLemmixStateType.Play },
        { "PAUSE", NeoLemmixStateType.Pause },
        { "STOP", NeoLemmixStateType.Stop },
        { "LOOPTOZERO", NeoLemmixStateType.LoopToZero },
        { "MATCHPHYSICS", NeoLemmixStateType.MatchPrimaryAnimationPhysics }
    };

    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;
    private NeoLemmixGadgetAnimationData? _currentNeoLemmixGadgetAnimationData;
    private AnimationTriggerData? _currentAnimationTriggerData;

    public GadgetAnimationReader(NeoLemmixGadgetArchetypeData gadgetArchetypeData)
        : base(string.Empty)
    {
        _gadgetArchetypeData = gadgetArchetypeData;

        SetNumberOfTokens(17);

        RegisterTokenAction("FRAMES", SetFrameCount);
        RegisterTokenAction("INITIAL_FRAME", SetInitialFrame);
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

        RegisterTokenAction("$TRIGGER", OnEnterTrigger);
        RegisterTokenAction("CONDITION", SetTriggerCondition);
        RegisterTokenAction("STATE", SetTriggerState);
        RegisterTokenAction("HIDE", SetTriggerHide);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        return Helpers.StringSpansMatch(token, "$PRIMARY_ANIMATION") ||
               Helpers.StringSpansMatch(token, "$ANIMATION");
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentNeoLemmixGadgetAnimationData = new NeoLemmixGadgetAnimationData();

        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        // Special handling for pickups specifically
        if (Helpers.StringSpansMatch(firstToken, "NAME") &&
            Helpers.StringSpansMatch(secondToken, "*PICKUP"))
        {
            _gadgetArchetypeData.IsSkillPickup = true;
            return false;
        }

        return ProcessTokenPair(line, firstToken, secondToken, secondTokenIndex);
    }

    private void SetFrameCount(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentNeoLemmixGadgetAnimationData!.FrameCount = int.Parse(secondToken);
    }

    private void SetInitialFrame(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentNeoLemmixGadgetAnimationData!.InitialFrame = Helpers.StringSpansMatch(secondToken, "RANDOM")
            ? -1
            : int.Parse(secondToken);
    }

    private void SetName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentNeoLemmixGadgetAnimationData!.TextureFilePath = Helpers.StringSpansMatch(secondToken, "*BLANK")
            ? string.Empty
            : ConstructFilePathForAdditionalTexture(secondToken);
    }

    private string ConstructFilePathForAdditionalTexture(ReadOnlySpan<char> secondToken)
    {
        var originalFilePath = _gadgetArchetypeData.FilePath.AsSpan();
        var originalFilePathWithoutExtension = Helpers.GetFullFilePathWithoutExtension(originalFilePath);

        return $"{originalFilePathWithoutExtension}_{secondToken}{DefaultFileExtensions.PngFileExtension}";
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
        _currentNeoLemmixGadgetAnimationData!.OffsetX = int.Parse(secondToken);
    }

    private void SetOffsetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentNeoLemmixGadgetAnimationData!.OffsetY = int.Parse(secondToken);
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

    private void OnEnterTrigger(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        Debug.Assert(_currentAnimationTriggerData is null);

        _currentAnimationTriggerData = new AnimationTriggerData();
    }

    private void SetTriggerCondition(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        Debug.Assert(_currentAnimationTriggerData is not null);

        _currentAnimationTriggerData!.Condition = GetNeoLemmixGadgetStateType(line, secondToken);
    }

    private void SetTriggerState(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        Debug.Assert(_currentAnimationTriggerData is not null);

        _currentAnimationTriggerData!.StateType = GetNeoLemmixGadgetAnimationAction(line, secondToken);
    }

    private void SetTriggerHide(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
      //  Debug.Assert(_currentAnimationTriggerData is not null);

        _currentAnimationTriggerData?.Hide = true;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var currentAnimationData = _currentNeoLemmixGadgetAnimationData!;

        if (_currentAnimationTriggerData is not null)
        {
            currentAnimationData.AnimationTriggers.Add(_currentAnimationTriggerData);
            _currentAnimationTriggerData = null;
            return;
        }

        _gadgetArchetypeData.AnimationData.Add(currentAnimationData);
        _currentNeoLemmixGadgetAnimationData = null;

        FinishedReading = true;
    }

    private static NeoLemmixGadgetCondition GetNeoLemmixGadgetStateType(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetStateTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NxlvReadingHelpers.ThrowUnknownTokenException<NeoLemmixGadgetCondition>("CONDITION", token, line);
    }

    private static NeoLemmixStateType GetNeoLemmixGadgetAnimationAction(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetSecondaryAnimationActionLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NxlvReadingHelpers.ThrowUnknownTokenException<NeoLemmixStateType>("STATE", token, line);
    }
}
