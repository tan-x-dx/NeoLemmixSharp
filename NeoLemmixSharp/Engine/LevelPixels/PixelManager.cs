using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine.LevelPixels;

public sealed class PixelManager
{
    private readonly PixelData _voidPixel;
    private readonly PixelData[] _data;
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

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
        _voidPixel = new PixelData { IsVoid = true };

        _data = new PixelData[Width * Height];

        _horizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(
            true,
            width);

        _verticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(
            true,
            height);

        for (var i = 0; i < _data.Length; i++)
        {
            _data[i] = new PixelData();
        }
    }

    public void SetTerrainSprite(TerrainSprite terrainSprite)
    {
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

    public PixelData GetPixelData(in LevelPosition levelPosition)
    {
        if (PositionOutOfBounds(levelPosition))
            return _voidPixel;

        var index = Width * levelPosition.Y + levelPosition.X;
        return _data[index];
    }

    public PixelData GetPixelData(int x, int y)
    {
        var index = Width * y + x;
        return _data[index];
    }

    public void ErasePixel(in LevelPosition pixelToErase)
    {
        var index = Width * pixelToErase.Y + pixelToErase.X;
        var pixel = _data[index];

        if (pixel is { IsVoid: false, IsSolid: true, IsSteel: false })
        {
            pixel.IsSolid = false;

            _terrainSprite.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
        }
    }

    public void SetSolidPixel(in LevelPosition pixelToSet, uint colour)
    {
        var pixel = GetPixelData(pixelToSet);
        if (pixel is { IsVoid: false, IsSolid: false })
        {
            pixel.IsSolid = true;
            _terrainSprite.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}