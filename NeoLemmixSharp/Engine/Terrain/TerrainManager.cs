using System.Collections.Generic;
using System.Linq;
using NeoLemmixSharp.Engine.BoundaryBehaviours;
using NeoLemmixSharp.Engine.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Engine.Gadgets;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Rendering.Level;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.Terrain;

public sealed class TerrainManager
{
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly PixelType[] _data;

    private readonly List<Gadget> _gadgetsThatCanActAsSolid = new();
    private readonly List<Gadget> _gadgetsThatCanActAsIndestructible = new();
    private readonly Dictionary<GadgetType, Gadget[]> _gadgetLookup = new();

    public TerrainRenderer TerrainRenderer { get; }

    public int Width { get; }
    public int Height { get; }

    public TerrainManager(
        int width,
        int height,
        PixelType[] pixels,
        IEnumerable<Gadget> gadgets,
        TerrainRenderer terrainRenderer,
        BoundaryBehaviourType horizontalBoundaryBehaviourType,
        BoundaryBehaviourType verticalBoundaryBehaviourType)
    {
        Width = width;
        Height = height;

        _data = pixels;
        TerrainRenderer = terrainRenderer;

        SetUpGadgets(gadgets);

        _horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(
            horizontalBoundaryBehaviourType,
            width);

        _verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(
            verticalBoundaryBehaviourType,
            height);
    }

    private void SetUpGadgets(IEnumerable<Gadget> gadgets)
    {
        foreach (var gadgetGroup in gadgets.GroupBy(g => g.GadgetType))
        {
            _gadgetLookup.Add(gadgetGroup.Key, gadgetGroup.ToArray());

            foreach (var gadget in gadgetGroup)
            {
                if (gadget.CanActAsSolid)
                {
                    _gadgetsThatCanActAsSolid.Add(gadget);
                }

                if (gadget.CanActAsIndestructible)
                {
                    _gadgetsThatCanActAsIndestructible.Add(gadget);
                }
            }
        }
    }

    public LevelPosition NormalisePosition(in LevelPosition levelPosition)
    {
        return new LevelPosition(
            _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }

    public bool PositionOutOfBounds(in LevelPosition levelPosition)
    {
        return levelPosition.X < 0 ||
               levelPosition.X >= Width ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= Height;
    }

    private PixelType GetPixelData(in LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return PixelType.Void;

        var index = Width * levelPosition.Y + levelPosition.X;
        return _data[index];
    }

    public bool PixelIsSolidToLemming(in LevelPosition levelPosition, Lemming lemming)
    {
        var pixel = GetPixelData(levelPosition);
        if (pixel == PixelType.Solid || pixel == PixelType.Steel)
            return true;

        for (var i = 0; i < _gadgetsThatCanActAsSolid.Count; i++)
        {
            if (_gadgetsThatCanActAsSolid[i].IsSolidToLemming(levelPosition, lemming))
                return true;
        }

        return false;
    }

    public bool PixelIsIndestructibleToLemming(in LevelPosition levelPosition, Lemming lemming)
    {
        var pixel = GetPixelData(levelPosition);
        if (pixel == PixelType.Steel)
            return true;

        for (var i = 0; i < _gadgetsThatCanActAsIndestructible.Count; i++)
        {
            if (_gadgetsThatCanActAsIndestructible[i].IsIndestructibleToLemming(levelPosition, lemming))
                return true;
        }

        return false;
    }

    public bool HasGadgetThatMatchesTypeAndOrientation(
        GadgetType gadgetType,
        LevelPosition levelPosition,
        Orientation orientation)
    {
        if (!_gadgetLookup.TryGetValue(gadgetType, out var gadgetArray))
            return false;

        for (var i = 0; i < gadgetArray.Length; i++)
        {
            if (gadgetArray[i].MatchesOrientation(levelPosition, orientation))
                return true;
        }

        return false;
    }

    public void ErasePixel(in LevelPosition pixelToErase)
    {
        if (PositionOutOfBounds(pixelToErase))
            return;

        var index = Width * pixelToErase.Y + pixelToErase.X;
        var pixel = _data[index];

        if (pixel == PixelType.Solid)
        {
            _data[index] = PixelType.Empty;
            TerrainRenderer.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
        }
    }

    public void SetSolidPixel(in LevelPosition pixelToSet, uint colour)
    {
        if (PositionOutOfBounds(pixelToSet))
            return;

        var index = Width * pixelToSet.Y + pixelToSet.X;
        var pixel = _data[index];

        if (pixel == PixelType.Empty)
        {
            _data[index] = PixelType.Solid;
            TerrainRenderer.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}