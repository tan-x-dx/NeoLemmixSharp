using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Engine.LevelGadgets;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class PixelManager
{
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private PixelType[] _data;
    private Gadget[] _gadgets;
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
        _gadgets = gadgets;
        _terrainSprite = terrainSprite;
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

        for (var i = 0; i < _gadgets.Length; i++)
        {
            if (_gadgets[i].IsSolidToLemming(levelPosition, lemming))
                return true;
        }

        return false;
    }

    public bool PixelIsIndestructibleToLemming(in LevelPosition levelPosition, Lemming lemming)
    {
        var pixel = GetPixelData(levelPosition);
        if (pixel == PixelType.Steel)
            return true;

        for (var i = 0; i < _gadgets.Length; i++)
        {
            if (_gadgets[i].IsIndestructibleToLemming(levelPosition, lemming))
                return true;
        }

        return false;
    }

    public bool HasGadgetThatMatchesTypeAndOrientation(LevelPosition levelPosition, GadgetType gadgetType, Orientation orientation)
    {
        for (var i = 0; i < _gadgets.Length; i++)
        {
            if (_gadgets[i].MatchesTypeAndOrientation(levelPosition, gadgetType, orientation))
                return true;
        }

        return false;
    }

    public void ErasePixel(in LevelPosition pixelToErase)
    {
        if (PositionOutOfBounds(pixelToErase))
            return;

        var index = Width * pixelToErase.Y + pixelToErase.X;
        ref var pixel = ref _data[index];

        if (pixel == PixelType.Solid)
        {
            pixel = PixelType.Empty;
            _terrainSprite.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
        }
    }

    public void SetSolidPixel(in LevelPosition pixelToSet, uint colour)
    {
        if (PositionOutOfBounds(pixelToSet))
            return;

        var index = Width * pixelToSet.Y + pixelToSet.X;
        ref var pixel = ref _data[index];

        if (pixel == PixelType.Empty)
        {
            pixel = PixelType.Solid;
            _terrainSprite.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}