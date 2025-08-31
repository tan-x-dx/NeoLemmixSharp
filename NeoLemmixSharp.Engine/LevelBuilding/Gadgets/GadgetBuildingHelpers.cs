using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.Trigger;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class GadgetBuildingHelpers
{
    private const string Trigger01Name = "Trigger 01";
    private const string Trigger02Name = "Trigger 02";
    private const string Trigger03Name = "Trigger 03";
    private const string Trigger04Name = "Trigger 04";
    private const string Trigger05Name = "Trigger 05";
    private const string Trigger06Name = "Trigger 06";
    private const string Trigger07Name = "Trigger 07";
    private const string Trigger08Name = "Trigger 08";
    private const string Trigger09Name = "Trigger 09";
    private const string Trigger10Name = "Trigger 10";
    private const string Trigger11Name = "Trigger 11";
    private const string Trigger12Name = "Trigger 12";
    private const string Trigger13Name = "Trigger 13";
    private const string Trigger14Name = "Trigger 14";
    private const string Trigger15Name = "Trigger 15";
    private const string Trigger16Name = "Trigger 16";

    private static readonly GadgetTriggerName[] BasicTriggerNames =
    [
        new(Trigger01Name),
        new(Trigger02Name),
        new(Trigger03Name),
        new(Trigger04Name),
        new(Trigger05Name),
        new(Trigger06Name),
        new(Trigger07Name),
        new(Trigger08Name),
        new(Trigger09Name),
        new(Trigger10Name),
        new(Trigger11Name),
        new(Trigger12Name),
        new(Trigger13Name),
        new(Trigger14Name),
        new(Trigger15Name),
        new(Trigger16Name)
    ];

    private const string State01Name = "State 01";
    private const string State02Name = "State 02";
    private const string State03Name = "State 03";
    private const string State04Name = "State 04";
    private const string State05Name = "State 05";
    private const string State06Name = "State 06";
    private const string State07Name = "State 07";
    private const string State08Name = "State 08";
    private const string State09Name = "State 09";
    private const string State10Name = "State 10";
    private const string State11Name = "State 11";
    private const string State12Name = "State 12";
    private const string State13Name = "State 13";
    private const string State14Name = "State 14";
    private const string State15Name = "State 15";
    private const string State16Name = "State 16";

    private static readonly GadgetStateName[] BasicStateNames =
    [
        new(State01Name),
        new(State02Name),
        new(State03Name),
        new(State04Name),
        new(State05Name),
        new(State06Name),
        new(State07Name),
        new(State08Name),
        new(State09Name),
        new(State10Name),
        new(State11Name),
        new(State12Name),
        new(State13Name),
        new(State14Name),
        new(State15Name),
        new(State16Name)
    ];

    public static ReadOnlySpan<GadgetTriggerName> GetDefaultTriggerNamesForCount(int numberOfTriggers)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfTriggers);
        ArgumentOutOfRangeException.ThrowIfZero(numberOfTriggers);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(numberOfTriggers, EngineConstants.MaxAllowedNumberOfGadgetTriggers);

        if (numberOfTriggers <= BasicTriggerNames.Length)
            return new ReadOnlySpan<GadgetTriggerName>(BasicTriggerNames, 0, numberOfTriggers);

        return ConstructLargeTriggerNameArray(numberOfTriggers);
    }

    private static ReadOnlySpan<GadgetTriggerName> ConstructLargeTriggerNameArray(int numberOfTriggers)
    {
        var result = new GadgetTriggerName[numberOfTriggers];

        new ReadOnlySpan<GadgetTriggerName>(BasicTriggerNames).CopyTo(result);

        for (var i = BasicTriggerNames.Length; i < result.Length; i++)
        {
            var triggerName = $"Trigger {i + 1}";
            result[i] = new GadgetTriggerName(triggerName);
        }

        return result;
    }

    public static ReadOnlySpan<GadgetStateName> GetDefaultStateNamesForCount(int numberOfStates)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfStates);
        ArgumentOutOfRangeException.ThrowIfZero(numberOfStates);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(numberOfStates, EngineConstants.MaxAllowedNumberOfGadgetStates);

        return new ReadOnlySpan<GadgetStateName>(BasicStateNames, 0, numberOfStates);
    }

    public static string GetGadgetName(IGadgetArchetypeData gadgetArchetypeData, IGadgetInstanceData gadgetInstanceData)
    {
        GadgetName result = gadgetInstanceData.OverrideName.IsTrivial
            ? gadgetArchetypeData.GadgetName
            : gadgetInstanceData.OverrideName;
        return result.ToString();
    }

    public static GadgetBounds CreateGadgetBounds(
        HitBoxGadgetArchetypeData gadgetArchetypeData,
        HitBoxGadgetInstanceData gadgetData)
    {
        var resizeType = gadgetArchetypeData.ResizeType;
        var baseSize = gadgetArchetypeData.BaseSpriteSize;

        var result = new GadgetBounds
        {
            Position = gadgetData.Position
        };

        var size = new Size(
            resizeType.CanResizeHorizontally() ? gadgetData.GetProperty(GadgetProperty.Width) : baseSize.W,
            resizeType.CanResizeVertically() ? gadgetData.GetProperty(GadgetProperty.Height) : baseSize.H);

        size = new DihedralTransformation(gadgetData.Orientation, gadgetData.FacingDirection).Transform(size);

        result.Width = size.W;
        result.Height = size.H;

        return result;
    }
}
