using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

public sealed class PrimaryAnimationReader : INeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$PRIMARY_ANIMATION";

    public PrimaryAnimationReader(NeoLemmixGadgetArchetypeData gadgetArchetypeData)
    {
        _gadgetArchetypeData = gadgetArchetypeData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        // Special handling for pickups specifically
        if (firstToken is "NAME" && secondToken is "*PICKUP")
        {
            _gadgetArchetypeData.IsSkillPickup = true;
            return false;
        }

        switch (firstToken)
        {
            case "FRAMES":
                _gadgetArchetypeData.PrimaryAnimationFrameCount = int.Parse(secondToken);
                break;

            case "OFFSET_X":

                break;

            case "OFFSET_Y":

                break;

            case "NINE_SLICE_TOP":
                _gadgetArchetypeData.ResizeType |= ResizeType.ResizeVertical;
                break;

            case "NINE_SLICE_RIGHT":
                _gadgetArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
                break;

            case "NINE_SLICE_BOTTOM":
                _gadgetArchetypeData.ResizeType |= ResizeType.ResizeVertical;
                break;

            case "NINE_SLICE_LEFT":
                _gadgetArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
                break;

            case "COLOR":

                break;

            case "$END":
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