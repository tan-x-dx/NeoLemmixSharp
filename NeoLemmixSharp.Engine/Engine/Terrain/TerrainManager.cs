using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Rendering;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Engine.Terrain;

public sealed class TerrainManager
{
    private readonly PixelType[] _pixels;
    private readonly GadgetManager _gadgetManager;

    private readonly TerrainRenderer _terrainRenderer;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    public int LevelWidth => _horizontalBoundaryBehaviour.LevelWidth;
    public int LevelHeight => _verticalBoundaryBehaviour.LevelHeight;

    public TerrainManager(
        PixelType[] pixels,
        GadgetManager gadgetManager,
        TerrainRenderer terrainRenderer,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _pixels = pixels;
        _gadgetManager = gadgetManager;

        _terrainRenderer = terrainRenderer;

        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
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
               levelPosition.X >= LevelWidth ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= LevelHeight;
    }

    [Pure]
    private PixelType PixelTypeAtPosition(LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return PixelType.Void;

        var index = LevelWidth * levelPosition.Y + levelPosition.X;
        return _pixels[index];
    }

    [Pure]
    public bool PixelIsSolidToLemming(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return pixel.IsSolidToOrientation(lemming.Orientation) ||
               _gadgetManager.HasGadgetOfTypeAtPosition(levelPosition, GadgetType.MetalGrate);
    }

    [Pure]
    public bool PixelIsIndestructibleToLemming(
        Lemming lemming,
        IDestructionAction destructionAction,
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return !pixel.CanBeDestroyed() ||
               !destructionAction.CanDestroyPixel(pixel, lemming.Orientation, lemming.FacingDirection) ||
               _gadgetManager.HasGadgetOfTypeAtPosition(levelPosition, GadgetType.MetalGrate);
    }

    [Pure]
    public bool PixelIsSteel(
        LevelPosition levelPosition)
    {
        var pixel = PixelTypeAtPosition(levelPosition);

        return pixel.IsSteel() ||
               _gadgetManager.HasGadgetOfTypeAtPosition(levelPosition, GadgetType.MetalGrate);
    }

    public void ErasePixel(
        Orientation orientation,
        IDestructionAction destructionAction,
        FacingDirection facingDirection,
        LevelPosition pixelToErase)
    {
        if (PositionOutOfBounds(pixelToErase))
            return;

        var index = LevelWidth * pixelToErase.Y + pixelToErase.X;
        var pixel = _pixels[index];

        if (!pixel.CanBeDestroyed() ||
            !destructionAction.CanDestroyPixel(pixel, orientation, facingDirection))
            return;

        _pixels[index] = PixelType.Empty;
        _terrainRenderer.SetPixelColor(pixelToErase.X, pixelToErase.Y, 0U);
    }

    public void SetSolidPixel(LevelPosition pixelToSet, uint color)
    {
        if (PositionOutOfBounds(pixelToSet))
            return;

        var index = LevelWidth * pixelToSet.Y + pixelToSet.X;
        var pixel = _pixels[index];

        if (pixel != PixelType.Empty)
            return;

        _pixels[index] |= PixelType.SolidToAllOrientations;
        _terrainRenderer.SetPixelColor(pixelToSet.X, pixelToSet.Y, color);
    }
}