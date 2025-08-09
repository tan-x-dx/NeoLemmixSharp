using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class GadgetBuildingHelpers
{
    private const string Input01Name = "Input 01";
    private const string Input02Name = "Input 02";
    private const string Input03Name = "Input 03";
    private const string Input04Name = "Input 04";
    private const string Input05Name = "Input 05";
    private const string Input06Name = "Input 06";
    private const string Input07Name = "Input 07";
    private const string Input08Name = "Input 08";
    private const string Input09Name = "Input 09";
    private const string Input10Name = "Input 10";
    private const string Input11Name = "Input 11";
    private const string Input12Name = "Input 12";
    private const string Input13Name = "Input 13";
    private const string Input14Name = "Input 14";
    private const string Input15Name = "Input 15";
    private const string Input16Name = "Input 16";

    private static readonly GadgetTriggerName[] BasicInputNames =
    [
        new(Input01Name),
        new(Input02Name),
        new(Input03Name),
        new(Input04Name),
        new(Input05Name),
        new(Input06Name),
        new(Input07Name),
        new(Input08Name),
        new(Input09Name),
        new(Input10Name),
        new(Input11Name),
        new(Input12Name),
        new(Input13Name),
        new(Input14Name),
        new(Input15Name),
        new(Input16Name)
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

    public static ReadOnlySpan<GadgetTriggerName> GetInputNamesForCount(int numberOfInputs)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfInputs);
        ArgumentOutOfRangeException.ThrowIfZero(numberOfInputs);

        if (numberOfInputs <= BasicInputNames.Length)
            return new ReadOnlySpan<GadgetTriggerName>(BasicInputNames, 0, numberOfInputs);

        return ConstructLargeInputNameArray(numberOfInputs);
    }

    private static ReadOnlySpan<GadgetTriggerName> ConstructLargeInputNameArray(int numberOfInputs)
    {
        var result = new GadgetTriggerName[numberOfInputs];

        new ReadOnlySpan<GadgetTriggerName>(BasicInputNames).CopyTo(result);

        for (var i = BasicInputNames.Length; i < result.Length; i++)
        {
            var inputName = $"Input {i + 1}";
            result[i] = new GadgetTriggerName(inputName);
        }

        return result;
    }

    public static ReadOnlySpan<GadgetStateName> GetStateNamesForCount(int numberOfStates)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfStates);
        ArgumentOutOfRangeException.ThrowIfZero(numberOfStates);

        if (numberOfStates <= BasicStateNames.Length)
            return new ReadOnlySpan<GadgetStateName>(BasicStateNames, 0, numberOfStates);

        return ConstructLargeStateNameArray(numberOfStates);
    }

    private static ReadOnlySpan<GadgetStateName> ConstructLargeStateNameArray(int numberOfStates)
    {
        var result = new GadgetStateName[numberOfStates];

        new ReadOnlySpan<GadgetStateName>(BasicStateNames).CopyTo(result);

        for (var i = BasicStateNames.Length; i < result.Length; i++)
        {
            var stateName = $"State {i + 1}";
            result[i] = new GadgetStateName(stateName);
        }

        return result;
    }

    public static string GetGadgetName(IGadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData)
    {
        return string.IsNullOrWhiteSpace(gadgetData.OverrideName)
            ? gadgetArchetypeData.GadgetName
            : gadgetData.OverrideName;
    }

    public static GadgetBounds CreateGadgetBounds(
        HitBoxGadgetArchetypeData gadgetArchetypeData,
        GadgetData gadgetData)
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
