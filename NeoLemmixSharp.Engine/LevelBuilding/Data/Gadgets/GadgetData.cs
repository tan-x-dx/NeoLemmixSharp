using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public sealed class GadgetData
{
    private readonly SimpleDictionary<GadgetProperty, object> _properties = GadgetPropertyHelpers.CreateSimpleDictionary<object>();

    public required int Id { get; init; }
    public required int GadgetBuilderId { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public void AddProperty(GadgetProperty property, object value)
    {
        _properties.Add(property, value);
    }

    public T GetProperty<T>(GadgetProperty property)
    {
        var value = _properties[property];

        if (value is T result)
            return result;

        ThrowTypeMismatchException(property, typeof(T), value);
        return default;
    }

    public bool TryGetProperty<T>(GadgetProperty property, [MaybeNullWhen(false)] out T value)
    {
        if (!_properties.TryGetValue(property, out var rawValue))
        {
            value = default;
            return false;
        }

        if (rawValue is T result)
        {
            value = result;
            return true;
        }

        ThrowTypeMismatchException(property, typeof(T), rawValue);
        value = default;
        return false;
    }

    [DoesNotReturn]
    private static void ThrowTypeMismatchException(
        GadgetProperty gadgetProperty,
        MemberInfo expectedType,
        object actualValue)
    {
        var actualType = actualValue.GetType();

        throw new InvalidOperationException(
            $"Expected type: [{expectedType.Name}] for property [{gadgetProperty}]. Instead got type: [{actualType.Name}] ({actualValue})");
    }
}