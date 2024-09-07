using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Level.Rewind;

public sealed class TerrainPainter
{
    private const int InitialPixelChangeListSize = 1 << 12;

    private readonly TickOrderedList<PixelChangeData> _pixelChangeList = new(InitialPixelChangeListSize);
    private readonly Texture2D _terrainTexture;
    private readonly PixelType[] _terrainPixelTypes;
    private readonly Color[] _terrainColors;
    private readonly int _terrainWidth;

    private int _firstIndexOfTickUpdates;

    public TerrainPainter(
        Texture2D terrainTexture,
        PixelType[] terrainPixelTypes,
        Color[] terrainColors,
        int terrainWidth)
    {
        _terrainTexture = terrainTexture;
        _terrainPixelTypes = terrainPixelTypes;
        _terrainColors = terrainColors;
        _terrainWidth = terrainWidth;
    }

    public void RecordPixelChange(LevelPosition pixel, Color toColor, PixelType fromPixelType, PixelType toPixelType)
    {
        var previousLatestTickWithUpdate = _pixelChangeList.LatestTickWithData();
        var currentLatestTickWithUpdate = LevelScreen.UpdateScheduler.ElapsedTicks;

        if (previousLatestTickWithUpdate != currentLatestTickWithUpdate)
        {
            _firstIndexOfTickUpdates = _pixelChangeList.Count;
        }

        var fromColor = _terrainColors[pixel.Y * _terrainWidth + pixel.X];

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
            _terrainColors[pixelChangeData.Y * _terrainWidth + pixelChangeData.X] = pixelChangeData.ToColor;
        }

        _terrainTexture.SetData(_terrainColors);
    }

    private ReadOnlySpan<PixelChangeData> GetLatestPixelChanges()
    {
        var startIndex = _firstIndexOfTickUpdates;
        _firstIndexOfTickUpdates = _pixelChangeList.Count;
        return _pixelChangeList.SliceToEnd(startIndex);
    }

    public void RewindBackTo(int tick)
    {
        var pixelChanges = _pixelChangeList.GetSliceBackTo(tick);
        if (pixelChanges.Length == 0)
            return;

        for (var i = pixelChanges.Length - 1; i >= 0; i--)
        {
            ref readonly var pixelChangeData = ref pixelChanges[i];
            var index = pixelChangeData.Y * _terrainWidth + pixelChangeData.X;
            _terrainColors[index] = pixelChangeData.FromColor;
            ref var pixelType = ref _terrainPixelTypes[index];

            pixelType &= PixelType.TerrainDataInverseMask; // Clear out existing terrain data
            pixelType |= (PixelType.TerrainDataMask & pixelChangeData.FromPixelType); // Add in the original terrain data
        }

        _terrainTexture.SetData(_terrainColors);
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

        int ITickOrderedData.TickNumber => Tick;

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