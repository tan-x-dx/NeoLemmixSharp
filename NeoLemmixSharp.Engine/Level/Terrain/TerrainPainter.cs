using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public sealed class TerrainPainter
{
    private readonly PixelChangeList _pixelChangeList = new();
    private readonly PixelType[] _terrainPixelTypes;
    private readonly int _terrainWidth;

    private readonly uint[] _terrainColors;

    private int _firstIndexOfFrameUpdates;

    public TerrainPainter(
        PixelType[] terrainPixelTypes,
        uint[] terrainColors,
        int terrainWidth)
    {
        _terrainPixelTypes = terrainPixelTypes;
        _terrainColors = terrainColors;
        _terrainWidth = terrainWidth;
    }

    public void RecordPixelChange(LevelPosition pixel, uint toColor, PixelType fromPixelType, PixelType toPixelType)
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

    public ReadOnlySpan<PixelChangeData> GetLatestPixelChanges()
    {
        var startIndex = _firstIndexOfFrameUpdates;
        _firstIndexOfFrameUpdates = _pixelChangeList.Count;
        return _pixelChangeList.SliceToEnd(startIndex);
    }

    private sealed class PixelChangeList
    {
        private PixelChangeData[] _terrainChanges;
        private int _count;

        public PixelChangeList()
        {
            _terrainChanges = new PixelChangeData[1 << 12];
        }

        public int Count => _count;

        public ref readonly PixelChangeData this[int index] => ref _terrainChanges[index];

        public ReadOnlySpan<PixelChangeData> Slice(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException(nameof(start), "Negative start index");
            if (start >= _count)
                throw new ArgumentOutOfRangeException(nameof(start), "Start index out of bounds");
            if (start + length >= _count)
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
        public readonly uint FromColor;
        public readonly uint ToColor;
        public readonly PixelType FromPixelType;
        public readonly PixelType ToPixelType;

        public PixelChangeData(int frame, int x, int y, uint fromColor, uint toColor, PixelType fromPixelType, PixelType toPixelType)
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