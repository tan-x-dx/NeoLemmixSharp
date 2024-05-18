using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public sealed class TerrainPainter
{
    private readonly PixelChangeList _pixelChangeList = new();
    private readonly Texture2D _terrainTexture;
    private readonly PixelType[] _terrainPixelTypes;
    private readonly Color[] _terrainColors;
    private readonly int _terrainWidth;

    private int _firstIndexOfFrameUpdates;

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
        var previousLatestFrameWithUpdate = _pixelChangeList.LatestFrameWithChange();
        var currentLatestFrameWithUpdate = LevelScreen.UpdateScheduler.ElapsedTicks;

        if (previousLatestFrameWithUpdate != currentLatestFrameWithUpdate)
        {
            _firstIndexOfFrameUpdates = _pixelChangeList.Count;
        }

        var fromColor = _terrainColors[pixel.Y * _terrainWidth + pixel.X];

        var pixelChangeData = new PixelChangeData(currentLatestFrameWithUpdate, pixel.X, pixel.Y, fromColor, toColor, fromPixelType, toPixelType);

        _pixelChangeList.AddTerrainChangeData(in pixelChangeData);
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
        var startIndex = _firstIndexOfFrameUpdates;
        _firstIndexOfFrameUpdates = _pixelChangeList.Count;
        return _pixelChangeList.SliceToEnd(startIndex);
    }

    private sealed class PixelChangeList
    {
        private PixelChangeData[] _terrainChanges = new PixelChangeData[1 << 12];
        private int _count;

        public int Count => _count;

        public ref readonly PixelChangeData this[int index] => ref _terrainChanges[index];

        public ReadOnlySpan<PixelChangeData> Slice(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative length");
            if (_count - start < length)
                throw new ArgumentOutOfRangeException(nameof(start), "Start index with length is out of bounds");

            return new ReadOnlySpan<PixelChangeData>(_terrainChanges, start, length);
        }

        public ReadOnlySpan<PixelChangeData> SliceToEnd(int start)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");

            return new ReadOnlySpan<PixelChangeData>(_terrainChanges, start, Math.Max(0, _count - start));
        }

        public void AddTerrainChangeData(in PixelChangeData pixelChangeData)
        {
            var arraySize = _terrainChanges.Length;
            if (_count == arraySize)
            {
                var newArray = new PixelChangeData[arraySize * 2];
                Array.Copy(_terrainChanges, newArray, arraySize);
                _terrainChanges = newArray;
            }

            _terrainChanges[_count++] = pixelChangeData;
        }

        public int LatestFrameWithChange()
        {
            if (_count == 0)
                return -1;

            ref readonly var pixelChangeData = ref _terrainChanges[_count - 1];

            return pixelChangeData.Frame;
        }
    }

    public readonly struct PixelChangeData
    {
        public readonly int Frame;
        public readonly int X;
        public readonly int Y;
        public readonly Color FromColor;
        public readonly Color ToColor;
        public readonly PixelType FromPixelType;
        public readonly PixelType ToPixelType;

        public PixelChangeData(int frame, int x, int y, Color fromColor, Color toColor, PixelType fromPixelType, PixelType toPixelType)
        {
            Frame = frame;
            X = x;
            Y = y;
            FromColor = fromColor;
            ToColor = toColor;
            FromPixelType = fromPixelType;
            ToPixelType = toPixelType;
        }
    }
}