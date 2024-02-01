using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

public sealed class GadgetArchetypeDataReader : INeoLemmixDataReader
{
    private readonly GadgetArchetypeData _gadgetArchetypeData;

    public GadgetArchetypeDataReader(GadgetArchetypeData gadgetArchetypeData)
    {
        _gadgetArchetypeData = gadgetArchetypeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "EFFECT";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

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
            case "TRIGGER_X":
                _gadgetArchetypeData.TriggerX = int.Parse(secondToken);
                break;

            case "TRIGGER_Y":
                _gadgetArchetypeData.TriggerY = int.Parse(secondToken);
                break;

            case "TRIGGER_WIDTH":
                _gadgetArchetypeData.TriggerWidth = int.Parse(secondToken);
                break;

            case "TRIGGER_HEIGHT":
                _gadgetArchetypeData.TriggerHeight = int.Parse(secondToken);
                break;

            case "SOUND":

                break;

            case "RESIZE_HORIZONTAL":

                break;

            case "RESIZE_VERTICAL":

                break;

            case "RESIZE_BOTH":

                break;

            case "DEFAULT_WIDTH":

                break;

            case "DEFAULT_HEIGHT":

                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing Gadget Archetype Data: [{firstToken}] line: \"{line}\"");
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