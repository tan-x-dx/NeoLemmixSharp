using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Terrain;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class TerrainPainter
{
    private const int InitialPixelChangeListSize = 1 << 12;

    private readonly TickOrderedList<PixelChangeData> _pixelChangeList = new(InitialPixelChangeListSize);
    private readonly Texture2D _terrainTexture;
    private readonly ArrayWrapper2D<PixelType> _terrainPixelTypes;
    private readonly ArrayWrapper2D<Color> _terrainColors;

    private int _latestIndexOfTickUpdates;

    public TerrainPainter(
        Texture2D terrainTexture,
        in ArrayWrapper2D<PixelType> terrainPixelTypes,
        in ArrayWrapper2D<Color> terrainColors)
    {
        _terrainTexture = terrainTexture;
        _terrainPixelTypes = terrainPixelTypes;
        _terrainColors = terrainColors;
    }

    public void RecordPixelChange(Point pixel, Color toColor, PixelType fromPixelType, PixelType toPixelType)
    {
        var previousLatestTickWithUpdate = _pixelChangeList.LatestTickWithData();
        var currentLatestTickWithUpdate = LevelScreen.UpdateScheduler.ElapsedTicks;

        if (previousLatestTickWithUpdate != currentLatestTickWithUpdate)
        {
            _latestIndexOfTickUpdates = _pixelChangeList.Count;
        }

        var fromColor = _terrainColors[pixel];

        ref var pixelChangeData = ref _pixelChangeList.GetNewDataRef();

        pixelChangeData = new PixelChangeData(currentLatestTickWithUpdate, pixel.X, pixel.Y, fromColor, toColor, fromPixelType, toPixelType);
    }

    public void RepaintTerrain()
    {
        var pixelChanges = GetLatestPixelChanges();
        if (pixelChanges.Length == 0)
            return;

        foreach (ref readonly var pixelChangeData in pixelChanges)
        {
            var position = new Point(pixelChangeData.X, pixelChangeData.Y);
            _terrainColors[position] = pixelChangeData.ToColor;
        }

        _terrainTexture.SetData(_terrainColors.Array);
    }

    private ReadOnlySpan<PixelChangeData> GetLatestPixelChanges()
    {
        var startIndex = _latestIndexOfTickUpdates;
        _latestIndexOfTickUpdates = _pixelChangeList.Count;
        return _pixelChangeList.GetSliceToEnd(startIndex);
    }

    public void RewindBackTo(int tick)
    {
        var pixelChanges = _pixelChangeList.RewindBackTo(tick - 1);
        if (pixelChanges.Length == 0)
            return;

        for (var i = pixelChanges.Length - 1; i >= 0; i--)
        {
            ref readonly var pixelChangeData = ref pixelChanges[i];
            var position = new Point(pixelChangeData.X, pixelChangeData.Y);
            _terrainColors[position] = pixelChangeData.FromColor;
            ref var pixelType = ref _terrainPixelTypes[position];

            pixelType &= PixelType.TerrainDataInverseMask; // Clear out existing terrain data
            pixelType |= (PixelType.TerrainDataMask & pixelChangeData.FromPixelType); // Add in the original terrain data
        }

        _terrainTexture.SetData(_terrainColors.Array);
    }

    private readonly struct PixelChangeData : ITickOrderedData
    {
        public readonly int Tick;
        public readonly int X;
        public readonly int Y;
        public readonly Color FromColor;
        public readonly Color ToColor;
        public readonly PixelType FromPixelType;
        public readonly PixelType ToPixelType;

        public int TickNumber => Tick;

        public PixelChangeData(int tick, int x, int y, Color fromColor, Color toColor, PixelType fromPixelType, PixelType toPixelType)
        {
            Tick = tick;
            X = x;
            Y = y;
            FromColor = fromColor;
            ToColor = toColor;
            FromPixelType = fromPixelType;
            ToPixelType = toPixelType;
        }
    }
}
