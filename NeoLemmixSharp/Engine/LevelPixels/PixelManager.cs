using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class PixelManager
{
    private readonly VoidPixelData _voidPixel;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private IPixelData[] _data;
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
        _voidPixel = new VoidPixelData();

        _horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(
            true,
            width);

        _verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(
            true,
            height);
    }

    public void SetData(
        IPixelData[] pixels,
        TerrainSprite terrainSprite)
    {
        _data = pixels;
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

    public IPixelData GetPixelData(in LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return _voidPixel;

        var index = Width * levelPosition.Y + levelPosition.X;
        return _data[index];
    }

    public void ErasePixel(in LevelPosition pixelToErase)
    {
        var pixel = GetPixelData(pixelToErase);
        if (pixel.ErasePixel())
        {
            _terrainSprite.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
        }
    }

    public void SetSolidPixel(in LevelPosition pixelToSet, uint colour)
    {
        var pixel = GetPixelData(pixelToSet);
        if (pixel.SetSolid())
        {
            _terrainSprite.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}