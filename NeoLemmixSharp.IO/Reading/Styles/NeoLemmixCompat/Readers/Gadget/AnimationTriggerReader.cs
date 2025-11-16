using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

internal sealed class AnimationTriggerReader : NeoLemmixDataReader
{
    private static readonly Dictionary<string, NeoLemmixGadgetStateType> _gadgetStateTypeLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "READY", NeoLemmixGadgetStateType.Idle },
        { "BUSY", NeoLemmixGadgetStateType.Active },
        { "DISABLED", NeoLemmixGadgetStateType.Disabled },
        { "EXHAUSTED", NeoLemmixGadgetStateType.Disabled }
    };

    private static readonly Dictionary<string, GadgetAnimationBehaviour> _gadgetSecondaryAnimationActionLookup = new(StringComparer.OrdinalIgnoreCase)
    {
        { "PLAY", GadgetAnimationBehaviour.Play },
        { "PAUSE", GadgetAnimationBehaviour.Pause },
        { "STOP", GadgetAnimationBehaviour.Stop },
        { "LOOPTOZERO", GadgetAnimationBehaviour.LoopToZero },
        { "MATCHPHYSICS", GadgetAnimationBehaviour.MatchPrimaryAnimationPhysics }
    };

    private readonly List<AnimationTriggerData> _animationTriggerData;
    private AnimationTriggerData? _currentAnimationTriggerData;

    public AnimationTriggerReader(List<AnimationTriggerData> animationTriggerData)
        : base("$ANIMATION")
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

    private static NeoLemmixGadgetStateType GetNeoLemmixGadgetStateType(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetStateTypeLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NxlvReadingHelpers.ThrowUnknownTokenException<NeoLemmixGadgetStateType>("CONDITION", token, line);
    }

    private static GadgetAnimationBehaviour GetNeoLemmixGadgetAnimationAction(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetSecondaryAnimationActionLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NxlvReadingHelpers.ThrowUnknownTokenException<GadgetAnimationBehaviour>("STATE", token, line);
    }
}

internal sealed class AnimationTriggerData
{
    public NeoLemmixGadgetStateType StateType { get; internal set; }
    public GadgetAnimationBehaviour AnimationAction { get; internal set; }
    public bool Hide { get; internal set; }
}
