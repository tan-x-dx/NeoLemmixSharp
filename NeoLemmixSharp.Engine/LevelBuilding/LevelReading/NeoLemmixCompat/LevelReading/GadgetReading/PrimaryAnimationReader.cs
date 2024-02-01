using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

public sealed class PrimaryAnimationReader : INeoLemmixDataReader
{
    private readonly GadgetArchetypeData _gadgetArchetypeData;

    public PrimaryAnimationReader(GadgetArchetypeData gadgetArchetypeData)
    {
        _gadgetArchetypeData = gadgetArchetypeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$PRIMARY_ANIMATION";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        if (firstToken[0] == '$')
        {
            FinishedReading = true;
            return true;
        }

        switch (firstToken)
        {
            case "FRAMES":
                _gadgetArchetypeData.PrimaryAnimationFrameCount = int.Parse(secondToken);
                break;

            case "NINE_SLICE_TOP":

                break;

            case "NINE_SLICE_RIGHT":

                break;

            case "NINE_SLICE_BOTTOM":

                break;

            case "NINE_SLICE_LEFT":

                break;

            case "COLOR":

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