using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public sealed class GadgetData
{
    private readonly (GadgetProperty, object)[] _properties;

    public required int Id { get; init; }
    public required int GadgetBuilderId { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public GadgetData((GadgetProperty, object)[] properties)
    {
        _properties = properties;
    }

    public bool GetProperty<T>(GadgetProperty property, out T? value)
    {
        foreach (var (tupleProperty, tupleValue) in _properties)
        {
            if (property != tupleProperty)
                continue;

            if (tupleValue is T result)
            {
                value = result;
                return true;
            }

            value = default;
            return false;
        }

        value = default;
        return false;
    }
}