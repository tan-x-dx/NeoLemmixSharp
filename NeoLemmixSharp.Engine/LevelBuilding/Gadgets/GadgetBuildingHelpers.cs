using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;

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

    private static readonly GadgetInputName[] BasicInputNames =
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

    public static ReadOnlySpan<GadgetInputName> GetInputNamesForCount(int numberOfInputs)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(numberOfInputs);
        ArgumentOutOfRangeException.ThrowIfZero(numberOfInputs);

        if (numberOfInputs <= BasicInputNames.Length)
            return new ReadOnlySpan<GadgetInputName>(BasicInputNames, 0, numberOfInputs);

        return ConstructLargeInputNameArray(numberOfInputs);
    }

    private static ReadOnlySpan<GadgetInputName> ConstructLargeInputNameArray(int numberOfInputs)
    {
        var result = new GadgetInputName[numberOfInputs];

        new ReadOnlySpan<GadgetInputName>(BasicInputNames).CopyTo(result);

        for (var i = BasicInputNames.Length; i < result.Length; i++)
        {
            var inputName = $"Input {i + 1}";
            result[i] = new GadgetInputName(inputName);
        }

        return result;
    }

    public static string GetGadgetName(GadgetArchetypeData gadgetArchetypeData, GadgetData gadgetData)
    {
        return string.IsNullOrEmpty(gadgetData.OverrideName)
            ? gadgetArchetypeData.GadgetName
            : gadgetData.OverrideName;
    }

    public static GadgetBounds CreateGadgetBounds(
        GadgetArchetypeData gadgetArchetypeData,
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

    public static Point DecodePoint(int combinedBits)
    {
        short x = (short)(combinedBits & 0xffff);
        short y = (short)(combinedBits >>> 16);

        return new Point(x, y);
    }
}
