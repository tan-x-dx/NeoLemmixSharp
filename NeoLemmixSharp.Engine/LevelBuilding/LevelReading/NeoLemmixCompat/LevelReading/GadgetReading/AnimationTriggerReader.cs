using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

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
                currentAnimationTriggerData.Condition = secondToken.GetString();
                break;

            case "STATE":
                currentAnimationTriggerData.State = secondToken.GetString();
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

    public void ApplyToLevelData(LevelData levelData)
    {
    }

    public void Dispose()
    {
    }
}