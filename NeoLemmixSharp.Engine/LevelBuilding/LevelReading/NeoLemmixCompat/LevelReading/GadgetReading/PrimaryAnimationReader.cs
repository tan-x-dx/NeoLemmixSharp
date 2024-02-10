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
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

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

    public void Dispose()
    {
    }
}