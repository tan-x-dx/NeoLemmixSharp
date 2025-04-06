using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class GadgetArchetypeDataReader : NeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;
    private readonly Dictionary<string, NeoLemmixGadgetBehaviour> _gadgetBehaviourLookup = new(34, StringComparer.OrdinalIgnoreCase)
    {
        { "ANTISPLATPAD", NeoLemmixGadgetBehaviour.AntiSplatPad },
        { "BACKGROUND", NeoLemmixGadgetBehaviour.Background },
        { "BUTTON", NeoLemmixGadgetBehaviour.UnlockButton },
        { "ENTRANCE", NeoLemmixGadgetBehaviour.Entrance },
        { "EXIT", NeoLemmixGadgetBehaviour.Exit },
        { "FIRE", NeoLemmixGadgetBehaviour.Fire },
        { "FORCE_LEFT", NeoLemmixGadgetBehaviour.ForceLeft },
        { "FORCE_RIGHT", NeoLemmixGadgetBehaviour.ForceRight },
        { "FORCELEFT", NeoLemmixGadgetBehaviour.ForceLeft },
        { "FORCERIGHT", NeoLemmixGadgetBehaviour.ForceRight },
        { "LOCKED_EXIT", NeoLemmixGadgetBehaviour.LockedExit },
        { "LOCKEDEXIT", NeoLemmixGadgetBehaviour.LockedExit },
        { "MOVING_BACKGROUND", NeoLemmixGadgetBehaviour.Background },
        { "ONE_WAY_LEFT", NeoLemmixGadgetBehaviour.OneWayLeft },
        { "ONE_WAY_DOWN", NeoLemmixGadgetBehaviour.OneWayDown },
        { "ONE_WAY_RIGHT", NeoLemmixGadgetBehaviour.OneWayRight },
        { "ONE_WAY_UP", NeoLemmixGadgetBehaviour.OneWayUp },
        { "ONEWAYLEFT", NeoLemmixGadgetBehaviour.OneWayLeft },
        { "ONEWAYDOWN", NeoLemmixGadgetBehaviour.OneWayDown },
        { "ONEWAYRIGHT", NeoLemmixGadgetBehaviour.OneWayRight },
        { "ONEWAYUP", NeoLemmixGadgetBehaviour.OneWayUp },
        { "PICKUP_SKILL", NeoLemmixGadgetBehaviour.PickupSkill },
        { "PICKUPSKILL", NeoLemmixGadgetBehaviour.PickupSkill },
        { "RECEIVER", NeoLemmixGadgetBehaviour.Receiver },
        { "SINGLE_USE_TRAP", NeoLemmixGadgetBehaviour.TrapOnce },
        { "SPLATPAD", NeoLemmixGadgetBehaviour.SplatPad },
        { "SPLITTER", NeoLemmixGadgetBehaviour.Splitter },
        { "TELEPORTER", NeoLemmixGadgetBehaviour.Teleporter },
        { "TRAP", NeoLemmixGadgetBehaviour.Trap },
        { "TRAPONCE", NeoLemmixGadgetBehaviour.TrapOnce },
        { "UPDRAFT", NeoLemmixGadgetBehaviour.Updraft },
        { "UNLOCKBUTTON", NeoLemmixGadgetBehaviour.UnlockButton },
        { "WATER", NeoLemmixGadgetBehaviour.Water },
        { "WINDOW", NeoLemmixGadgetBehaviour.Entrance },
    };

    public GadgetArchetypeDataReader(
        NeoLemmixGadgetArchetypeData gadgetArchetypeData)
        : base("EFFECT")
    {
        _gadgetArchetypeData = gadgetArchetypeData;

        SetNumberOfTokens(16);

        RegisterTokenAction("TRIGGER_X", SetTriggerX);
        RegisterTokenAction("TRIGGER_Y", SetTriggerY);
        RegisterTokenAction("TRIGGER_WIDTH", SetTriggerWidth);
        RegisterTokenAction("TRIGGER_HEIGHT", SetTriggerHeight);
        RegisterTokenAction("SOUND", SetSound);
        RegisterTokenAction("SOUND_ACTIVATE", SetSoundActivate);
        RegisterTokenAction("RESIZE_HORIZONTAL", SetResizeHorizontal);
        RegisterTokenAction("RESIZE_VERTICAL", SetResizeVertical);
        RegisterTokenAction("RESIZE_BOTH", SetResizeBoth);
        RegisterTokenAction("DEPRECATED", SetDeprecated);
        RegisterTokenAction("DEFAULT_WIDTH", SetDefaultWidth);
        RegisterTokenAction("DEFAULT_HEIGHT", SetDefaultHeight);
        RegisterTokenAction("DIGIT_X", SetDigitX);
        RegisterTokenAction("DIGIT_Y", SetDigitY);
        RegisterTokenAction("DIGIT_ALIGNMENT", SetDigitAlignment);
        RegisterTokenAction("DIGIT_LENGTH", SetDigitLength);
    }

    public override bool ShouldProcessSection(ReadOnlySpan<char> token)
    {
        return TokensMatch(token, IdentifierToken) ||
               TokensMatch(token, "NO_EFFECT");
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        NxlvReadingHelpers.GetTokenPair(line, out _, out var secondToken, out _);
        _gadgetArchetypeData.Behaviour = GetNeoLemmixGadgetType(secondToken);
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        if (firstToken[0] == '$')
        {
            FinishedReading = true;
            return true;
        }

        return ProcessLineTokens(line);
    }

    private void SetTriggerX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.TriggerX = int.Parse(secondToken);
    }

    private void SetTriggerY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.TriggerY = int.Parse(secondToken);
    }

    private void SetTriggerWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.TriggerWidth = int.Parse(secondToken);
    }

    private void SetTriggerHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // Subtract 1 from height because of differences in physics between engines
        _gadgetArchetypeData.TriggerHeight = Math.Max(int.Parse(secondToken) - 1, 1);
    }

    private void SetSound(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetSoundActivate(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetResizeHorizontal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType |= ResizeType.ResizeHorizontal;
    }

    private void SetResizeVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType |= ResizeType.ResizeVertical;
    }

    private void SetResizeBoth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.ResizeType = ResizeType.ResizeBoth;
    }

    private void SetDeprecated(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetDefaultWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.DefaultWidth = int.Parse(secondToken);
    }

    private void SetDefaultHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _gadgetArchetypeData.DefaultHeight = int.Parse(secondToken);
    }

    private void SetDigitX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetDigitY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetDigitAlignment(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetDigitLength(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private NeoLemmixGadgetBehaviour GetNeoLemmixGadgetType(
        ReadOnlySpan<char> token)
    {
        var alternateLookup = _gadgetBehaviourLookup.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(token, out var result))
            return result;

        return NeoLemmixGadgetBehaviour.None;
    }
}