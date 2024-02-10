using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

public sealed class GadgetArchetypeDataReader : INeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;

    public GadgetArchetypeDataReader(NeoLemmixGadgetArchetypeData gadgetArchetypeData)
    {
        _gadgetArchetypeData = gadgetArchetypeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "EFFECT";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        ReadingHelpers.GetTokenPair(line, out _, out var secondToken, out _);

        _gadgetArchetypeData.Type = GetNeoLemmixGadgetType(secondToken);
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

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
                // Subtract 1 from height because of differences in physics between engines
                _gadgetArchetypeData.TriggerHeight = int.Parse(secondToken) - 1;
                break;

            case "SOUND":

                break;

            case "SOUND_ACTIVATE":

                break;

            case "RESIZE_HORIZONTAL":
                _gadgetArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
                break;

            case "RESIZE_VERTICAL":
                _gadgetArchetypeData.ResizeType |= ResizeType.ResizeVertical;
                break;

            case "RESIZE_BOTH":
                _gadgetArchetypeData.ResizeType = ResizeType.ResizeBoth;
                break;

            case "DEFAULT_WIDTH":

                break;

            case "DEFAULT_HEIGHT":

                break;

            case "DIGIT_X":

                break;

            case "DIGIT_Y":

                break;

            case "DIGIT_ALIGNMENT":

                break;

            case "DIGIT_LENGTH":

                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException("Gadget Archetype Data", firstToken, line);
                break;
        }

        return false;
    }

    private static NeoLemmixGadgetType GetNeoLemmixGadgetType(
        ReadOnlySpan<char> token) => token switch
    {
        "ANTISPLATPAD" => NeoLemmixGadgetType.AntiSplatPad,
        "BACKGROUND" => NeoLemmixGadgetType.Background,
        "BUTTON" => NeoLemmixGadgetType.UnlockButton,
        "ENTRANCE" => NeoLemmixGadgetType.Entrance,
        "EXIT" => NeoLemmixGadgetType.Exit,
        "FIRE" => NeoLemmixGadgetType.Fire,
        "FORCE_LEFT" => NeoLemmixGadgetType.ForceLeft,
        "FORCE_RIGHT" => NeoLemmixGadgetType.ForceRight,
        "FORCELEFT" => NeoLemmixGadgetType.ForceLeft,
        "FORCERIGHT" => NeoLemmixGadgetType.ForceRight,
        "LOCKED_EXIT" => NeoLemmixGadgetType.LockedExit,
        "LOCKEDEXIT" => NeoLemmixGadgetType.LockedExit,
        "MOVING_BACKGROUND" => NeoLemmixGadgetType.Background,
        "ONE_WAY_LEFT" => NeoLemmixGadgetType.OneWayLeft,
        "ONE_WAY_DOWN" => NeoLemmixGadgetType.OneWayDown,
        "ONE_WAY_RIGHT" => NeoLemmixGadgetType.OneWayRight,
        "ONE_WAY_UP" => NeoLemmixGadgetType.OneWayUp,
        "ONEWAYLEFT" => NeoLemmixGadgetType.OneWayLeft,
        "ONEWAYDOWN" => NeoLemmixGadgetType.OneWayDown,
        "ONEWAYRIGHT" => NeoLemmixGadgetType.OneWayRight,
        "ONEWAYUP" => NeoLemmixGadgetType.OneWayUp,
        "PICKUP_SKILL" => NeoLemmixGadgetType.PickupSkill,
        "PICKUPSKILL" => NeoLemmixGadgetType.PickupSkill,
        "RECEIVER" => NeoLemmixGadgetType.Receiver,
        "SINGLE_USE_TRAP" => NeoLemmixGadgetType.TrapOnce,
        "SPLATPAD" => NeoLemmixGadgetType.SplatPad,
        "SPLITTER" => NeoLemmixGadgetType.Splitter,
        "TELEPORTER" => NeoLemmixGadgetType.Teleporter,
        "TRAP" => NeoLemmixGadgetType.Trap,
        "TRAPONCE" => NeoLemmixGadgetType.TrapOnce,
        "UPDRAFT" => NeoLemmixGadgetType.Updraft,
        "UNLOCKBUTTON" => NeoLemmixGadgetType.UnlockButton,
        "WATER" => NeoLemmixGadgetType.Water,
        "WINDOW" => NeoLemmixGadgetType.Entrance,

        _ => NeoLemmixGadgetType.None
    };

    public void ApplyToLevelData(LevelData levelData)
    {
    }

    public void Dispose()
    {
    }
}