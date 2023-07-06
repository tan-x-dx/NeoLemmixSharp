using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Rendering;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Terrain;

public sealed class TerrainManager
{
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly PixelType[] _pixels;

    public TerrainRenderer TerrainRenderer { get; }

    public int Width { get; }
    public int Height { get; }

    public TerrainManager(
        int width,
        int height,
        PixelType[] pixels,
        TerrainRenderer terrainRenderer,
        BoundaryBehaviourType horizontalBoundaryBehaviourType,
        BoundaryBehaviourType verticalBoundaryBehaviourType)
    {
        Width = width;
        Height = height;

        _pixels = pixels;
        TerrainRenderer = terrainRenderer;

        _horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(
            horizontalBoundaryBehaviourType,
            width);

        _verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(
            verticalBoundaryBehaviourType,
            height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PositionOutOfBounds(LevelPosition levelPosition)
    {
        return levelPosition.X < 0 ||
               levelPosition.X >= Width ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= Height;
    }

    public bool PixelIsSolidToLemming(LevelPosition levelPosition, Lemming lemming)
    {
        var index = Width * levelPosition.Y + levelPosition.X;
        var pixel = _pixels[index];
        if (pixel == PixelType.Solid || pixel == PixelType.Steel)
            return true;

        return GadgetCollections.MetalGrates.TryGetGadgetThatMatchesTypeAndOrientation(levelPosition, lemming.Orientation, out _);
    }

    public bool PixelIsIndestructibleToLemming(LevelPosition levelPosition, Lemming lemming)
    {
        var index = Width * levelPosition.Y + levelPosition.X;
        var pixel = _pixels[index];
        if (pixel == PixelType.Steel)
            return true;

        return GadgetCollections.MetalGrates.TryGetGadgetThatMatchesTypeAndOrientation(levelPosition, lemming.Orientation, out _);
    }

    public void ErasePixel(LevelPosition pixelToErase)
    {
        if (PositionOutOfBounds(pixelToErase))
            return;

        var index = Width * pixelToErase.Y + pixelToErase.X;
        var pixel = _pixels[index];

        if (pixel == PixelType.Solid)
        {
            _pixels[index] = PixelType.Empty;
            TerrainRenderer.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
        }
    }

    public void SetSolidPixel(LevelPosition pixelToSet, uint colour)
    {
        if (PositionOutOfBounds(pixelToSet))
            return;

        var index = Width * pixelToSet.Y + pixelToSet.X;
        var pixel = _pixels[index];

        if (pixel == PixelType.Empty)
        {
            _pixels[index] = PixelType.Solid;
            TerrainRenderer.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}