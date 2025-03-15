namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

/*public sealed class AnimationTriggerReader : NeoLemmixDataReader
{
    private readonly List<AnimationTriggerData> _animationTriggerData;
    private readonly Dictionary<string, NeoLemmixGadgetStateType> _gadgetStateTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "READY", NeoLemmixGadgetStateType.Idle },
        { "BUSY", NeoLemmixGadgetStateType.Active },
        { "DISABLED", NeoLemmixGadgetStateType.Disabled },
        { "EXHAUSTED", NeoLemmixGadgetStateType.Disabled }
    };

    private readonly Dictionary<string, GadgetSecondaryAnimationAction> _gadgetSecondaryAnimationActionLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "PLAY", GadgetSecondaryAnimationAction.Play },
        { "PAUSE", GadgetSecondaryAnimationAction.Pause },
        { "STOP", GadgetSecondaryAnimationAction.Stop },
        { "LOOPTOZERO", GadgetSecondaryAnimationAction.LoopToZero },
        { "MATCHPHYSICS", GadgetSecondaryAnimationAction.MatchPrimaryAnimationPhysics }
    };

    private AnimationTriggerData? _currentAnimationTriggerData;

    public AnimationTriggerReader(List<AnimationTriggerData> animationTriggerData)
        : base("$TRIGGER")
    {
        _animationTriggerData = animationTriggerData;

        RegisterTokenAction("CONDITION", SetStateType);
        RegisterTokenAction("STATE", SetAnimationAction);
        RegisterTokenAction("HIDE", SetHidden);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentAnimationTriggerData = new AnimationTriggerData();
        FinishedReading = false;
        return false;
    }

    private void SetStateType(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentAnimationTriggerData!.StateType = GetNeoLemmixGadgetStateType(line, secondToken);
    }

    private void SetAnimationAction(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentAnimationTriggerData!.AnimationAction = GetNeoLemmixGadgetAnimationAction(line, secondToken);
    }

    private void SetHidden(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentAnimationTriggerData!.Hide = true;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _animationTriggerData.Add(_currentAnimationTriggerData!);
        _currentAnimationTriggerData = null;
        FinishedReading = true;
    }

    private NeoLemmixGadgetStateType GetNeoLemmixGadgetStateType(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetStateTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NxlvReadingHelpers.ThrowUnknownTokenException<NeoLemmixGadgetStateType>("CONDITION", token, line);
    }

    private GadgetSecondaryAnimationAction GetNeoLemmixGadgetAnimationAction(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetSecondaryAnimationActionLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NxlvReadingHelpers.ThrowUnknownTokenException<GadgetSecondaryAnimationAction>("STATE", token, line);
    }
}*/