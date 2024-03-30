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
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        var currentAnimationTriggerData = _currentAnimationTriggerData!;

        switch (firstToken)
        {
            case "CONDITION":
                currentAnimationTriggerData.StateType = GetNeoLemmixGadgetStateType(line, secondToken);
                break;

            case "STATE":
                currentAnimationTriggerData.State = secondToken.ToString();
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
                ReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
        }

        return false;
    }

    private static NeoLemmixGadgetStateType GetNeoLemmixGadgetStateType(ReadOnlySpan<char> line, ReadOnlySpan<char> token)
    {
        var result = NeoLemmixGadgetStateType.Idle;

        switch (token)
        {
            case "READY":
                result = NeoLemmixGadgetStateType.Idle;
                break;
                
            case "BUSY":
                result = NeoLemmixGadgetStateType.Active;
                break;

            case "DISABLED":
                result = NeoLemmixGadgetStateType.Disabled;
                break;

            case "EXHAUSTED":
                result = NeoLemmixGadgetStateType.Disabled;
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException("CONDITION", token, line);
                break;
        }

        return result;
    }
}