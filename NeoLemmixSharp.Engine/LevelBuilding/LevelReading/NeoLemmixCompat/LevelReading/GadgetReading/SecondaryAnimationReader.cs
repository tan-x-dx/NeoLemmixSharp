using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

public sealed class SecondaryAnimationReader : INeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;

    private SecondaryAnimationData? _secondaryAnimationData;

    public SecondaryAnimationReader(NeoLemmixGadgetArchetypeData gadgetArchetypeData)
    {
        _gadgetArchetypeData = gadgetArchetypeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$ANIMATION";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        _secondaryAnimationData = new SecondaryAnimationData();
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        var secondaryAnimationData = _secondaryAnimationData!;

        switch (firstToken)
        {
            case "INITIAL_FRAME":
                var initialFrame = secondToken is "RANDOM"
                    ? -1
                    : int.Parse(secondToken);

                secondaryAnimationData.InitialFrame = initialFrame;
                break;

            case "FRAMES":
                secondaryAnimationData.NumberOfFrames = int.Parse(secondToken);
                break;

            case "NAME":
                secondaryAnimationData.Name = secondToken.GetString();
                break;

            case "OFFSET_X":
                secondaryAnimationData.OffsetX = int.Parse(secondToken);
                break;

            case "OFFSET_Y":
                secondaryAnimationData.OffsetY = int.Parse(secondToken);
                break;

            case "$END":
                _secondaryAnimationData = null;
                _gadgetArchetypeData.SecondaryAnimationData = secondaryAnimationData;
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
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