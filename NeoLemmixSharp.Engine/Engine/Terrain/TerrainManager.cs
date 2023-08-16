using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Gadgets.Collections;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Rendering;
using System.Diagnostics.Contracts;
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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            _horizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            _verticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool PositionOutOfBounds(LevelPosition levelPosition)
    {
        return levelPosition.X < 0 ||
               levelPosition.X >= Width ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= Height;
    }

    [Pure]
    public bool PixelIsSolidToLemming(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return false;

        var index = Width * levelPosition.Y + levelPosition.X;
        var pixel = _pixels[index];

        return pixel.IsSolidToOrientation(lemming.Orientation) ||
               GadgetCollections.MetalGrates.TryGetGadgetThatMatchesTypeAndOrientation(lemming, levelPosition, out _);
    }

    [Pure]
    public bool PixelIsIndestructibleToLemming(
        Lemming lemming,
        IDestructionAction destructionAction,
        LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return false;

        var index = Width * levelPosition.Y + levelPosition.X;
        var pixel = _pixels[index];

        return !pixel.CanBeDestroyed() ||
               !destructionAction.CanDestroyPixel(pixel, lemming.Orientation, lemming.FacingDirection) ||
               GadgetCollections.MetalGrates.TryGetGadgetThatMatchesTypeAndOrientation(lemming, levelPosition, out _);
    }

    [Pure]
    public bool PixelIsSteel(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return false;

        var index = Width * levelPosition.Y + levelPosition.X;
        var pixel = _pixels[index];

        return pixel.IsSteel() ||
               GadgetCollections.MetalGrates.TryGetGadgetThatMatchesTypeAndOrientation(lemming, levelPosition, out _);
    }

    public void ErasePixel(
        Orientation orientation,
        IDestructionAction destructionAction,
        FacingDirection facingDirection,
        LevelPosition pixelToErase)
    {
        if (PositionOutOfBounds(pixelToErase))
            return;

        var index = Width * pixelToErase.Y + pixelToErase.X;
        var pixel = _pixels[index];

        if (!pixel.CanBeDestroyed() ||
            !destructionAction.CanDestroyPixel(pixel, orientation, facingDirection))
            return;

        _pixels[index] = PixelType.Empty;
        TerrainRenderer.SetPixelColor(pixelToErase.X, pixelToErase.Y, 0U);
    }

    public void SetSolidPixel(LevelPosition pixelToSet, uint color)
    {
        if (PositionOutOfBounds(pixelToSet))
            return;

        var index = Width * pixelToSet.Y + pixelToSet.X;
        var pixel = _pixels[index];

        if (pixel != PixelType.Empty)
            return;

        _pixels[index] |= PixelType.SolidToAllOrientations;
        TerrainRenderer.SetPixelColor(pixelToSet.X, pixelToSet.Y, color);
    }
}