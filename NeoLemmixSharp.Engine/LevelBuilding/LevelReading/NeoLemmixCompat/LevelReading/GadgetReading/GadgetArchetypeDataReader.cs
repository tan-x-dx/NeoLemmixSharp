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
    public string IdentifierToken { get; }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        throw new NotImplementedException();
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        switch (firstToken)
        {
            case "EFFECT":

                break;

            case "TRIGGER_X":

                break;

            case "TRIGGER_Y":

                break;

            case "TRIGGER_WIDTH":

                break;

            case "TRIGGER_HEIGHT":

                break;

            case "SOUND":

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