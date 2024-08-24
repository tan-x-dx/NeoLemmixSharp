using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class AnimationTriggerReader : INeoLemmixDataReader
{
    private readonly List<AnimationTriggerData> _animationTriggerData;

    private AnimationTriggerData? _currentAnimationTriggerData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TRIGGER";

    public AnimationTriggerReader(List<AnimationTriggerData> animationTriggerData)
    {
        _animationTriggerData = animationTriggerData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentAnimationTriggerData = new AnimationTriggerData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        var currentAnimationTriggerData = _currentAnimationTriggerData!;

        switch (firstToken)
        {
            case "CONDITION":
                currentAnimationTriggerData.StateType = GetNeoLemmixGadgetStateType(line, secondToken);
                break;

            case "STATE":
                currentAnimationTriggerData.AnimationAction = GetNeoLemmixGadgetAnimationAction(line, secondToken);
                break;

            case "HIDE":
                currentAnimationTriggerData.Hide = true;
                break;

            case "$END":
                _animationTriggerData.Add(currentAnimationTriggerData);
                _currentAnimationTriggerData = null;
                FinishedReading = true;
                break;

            default:
                NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
        }

        return false;
    }

    private static NeoLemmixGadgetStateType GetNeoLemmixGadgetStateType(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var result = token switch
        {
            "READY" => NeoLemmixGadgetStateType.Idle,
            "BUSY" => NeoLemmixGadgetStateType.Active,
            "DISABLED" => NeoLemmixGadgetStateType.Disabled,
            "EXHAUSTED" => NeoLemmixGadgetStateType.Disabled,

            _ => NxlvReadingHelpers.ThrowUnknownTokenException<NeoLemmixGadgetStateType>("CONDITION", token, line)
        };

        return result;
    }

    private static GadgetSecondaryAnimationAction GetNeoLemmixGadgetAnimationAction(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var result = token switch
        {
            "PLAY" => GadgetSecondaryAnimationAction.Play,
            "PAUSE" => GadgetSecondaryAnimationAction.Pause,
            "STOP" => GadgetSecondaryAnimationAction.Stop,
            "LOOPTOZERO" => GadgetSecondaryAnimationAction.LoopToZero,
            "MATCHPHYSICS" => GadgetSecondaryAnimationAction.MatchPrimaryAnimationPhysics,

            _ => NxlvReadingHelpers.ThrowUnknownTokenException<GadgetSecondaryAnimationAction>("STATE", token, line),
        };

        return result;
    }
}