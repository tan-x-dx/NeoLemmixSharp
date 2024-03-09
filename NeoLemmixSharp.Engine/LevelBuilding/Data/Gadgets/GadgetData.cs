using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public sealed class GadgetData
{
    private readonly SimpleDictionary<GadgetProperty, int> _properties = GadgetPropertyHelpers.CreateSimpleIntDictionary();

    public required int Id { get; init; }
    public required int GadgetBuilderId { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    public required bool ShouldRender { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public void AddProperty(GadgetProperty property, int value)
    {
        _properties.Add(property, value);
    }

    public int GetProperty(GadgetProperty property)
    {
        return _properties[property];
    }

    public bool TryGetProperty(GadgetProperty property, out int value)
    {
        return _properties.TryGetValue(property, out value);
    }

    public void GetDihedralTransformation(out DihedralTransformation dihedralTransformation)
    {
        dihedralTransformation = new DihedralTransformation(
            Orientation.RotNum,
            FacingDirection == FacingDirection.LeftInstance);
    }
}