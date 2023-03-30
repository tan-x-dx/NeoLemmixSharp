using NeoLemmixSharp.Engine.LevelBoundaryBehaviours;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Rendering;

namespace NeoLemmixSharp.Engine;

public sealed class PixelManager
{
    private readonly PixelData _voidPixel;
    private readonly PixelData[] _data;
    public IHorizontalBoundaryBehaviour HorizontalBoundaryBehaviour { get; }
    public IVerticalBoundaryBehaviour VerticalBoundaryBehaviour { get; }

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

        HorizontalBoundaryBehaviour = BoundaryHelpers.GetHorizontalBoundaryBehaviour(
            horizontalBoundaryBehaviourType,
            width,
            height);

        VerticalBoundaryBehaviour = BoundaryHelpers.GetVerticalBoundaryBehaviour(
            verticalBoundaryBehaviourType,
            width,
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

    public LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            HorizontalBoundaryBehaviour.NormaliseX(levelPosition.X),
            VerticalBoundaryBehaviour.NormaliseY(levelPosition.Y));
    }

    public bool PositionOutOfBounds(LevelPosition levelPosition)
    {
        return levelPosition.X < 0 ||
               levelPosition.X >= Width ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= Height;
    }

    public PixelData GetPixelData(LevelPosition levelPosition)
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

    public void ErasePixel(LevelPosition pixelToErase)
    {
        var index = Width * pixelToErase.Y + pixelToErase.X;
        var pixel = _data[index];

        if (pixel is { IsVoid: false, IsSolid: true, IsSteel: false })
        {
            pixel.IsSolid = false;

            _terrainSprite.SetPixelColour(pixelToErase.X, pixelToErase.Y, 0U);
        }
    }

    public void SetSolidPixel(LevelPosition pixelToSet, uint colour)
    {
        var pixel = GetPixelData(pixelToSet);
        if (pixel is { IsVoid: false, IsSolid: false })
        {
            pixel.IsSolid = true;
            _terrainSprite.SetPixelColour(pixelToSet.X, pixelToSet.Y, colour);
        }
    }
}