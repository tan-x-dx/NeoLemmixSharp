﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public sealed class GadgetData
{
    private readonly SimpleDictionary<GadgetProperty, int> _properties = GadgetPropertyHelpers.CreateSimpleIntDictionary();

    public required int Id { get; init; }
    public required string Style { get; init; }
    public required string GadgetPiece { get; init; }
    public required int GadgetBuilderId { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    public required int InitialStateId { get; init; }
    public required GadgetRenderMode GadgetRenderMode { get; init; }

    public required Orientation Orientation { get; init; }
    public required FacingDirection FacingDirection { get; init; }

    public int NumberOfGadgetProperties => _properties.Count;

    public void SetProperty(GadgetProperty property, int value)
    {
        _properties[property] = value;
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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleDictionary<GadgetProperty, int>.Enumerator GetProperties() => _properties.GetEnumerator();
}