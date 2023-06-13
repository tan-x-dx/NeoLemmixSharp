using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Engine.LevelGadgets;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class PixelManager
{
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly List<Gadget> _gadgetsThatCanActAsSolid = new();
    private readonly List<Gadget> _gadgetsThatCanActAsIndestructible = new();
    private readonly Dictionary<GadgetType, Gadget[]> _gadgetLookup;

    private PixelType[] _data;
    private TerrainSprite _terrainSprite;

    public int Width { get; }
    public int Height { get; }

    public PixelManager(
        int width,
        int height,
        BoundaryBehaviourType horizontalBoundaryBehaviourType,
        BoundaryBehaviourType verticalBoundaryBehaviourType)
    {
        Width = width;
        Height = height;

        _horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(
            true,
            width);

        _verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(
            true,
            height);
    }

    public void SetData(
        PixelType[] pixels,
        Gadget[] gadgets,
        TerrainSprite terrainSprite)
    {
        _data = pixels;
        _terrainSprite = terrainSprite;

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
            _terrainSprite.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
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
            _terrainSprite.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}