using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Level.Terrain;

public sealed class TerrainPainter
{
    private const int InitialPixelChangeListSize = 1 << 12;

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

        ref var pixelChangeData = ref _pixelChangeList.GetNewPixelChangeDataRef();

        pixelChangeData = new PixelChangeData(currentLatestFrameWithUpdate, pixel.X, pixel.Y, fromColor, toColor, fromPixelType, toPixelType);
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

    public void RewindBackTo(int frame)
    {
        var pixelChanges = _pixelChangeList.GetSliceBackTo(frame);
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

    private sealed class PixelChangeList
    {
        private PixelChangeData[] _terrainChanges = new PixelChangeData[InitialPixelChangeListSize];
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

        public ReadOnlySpan<PixelChangeData> GetSliceBackTo(int frame)
        {
            var index = GetSmallestIndexOfFrame(frame);

            return SliceToEnd(index);
        }

        /// <summary>
        /// Returns the smallest index such that the data at that index has a frame equal to or exceeding the input parameter
        /// <para>
        /// Binary search algorithm - O(log n)
        /// </para>
        /// </summary>
        private int GetSmallestIndexOfFrame(int frame)
        {
            if (_count == 0)
                return 0;

            var upperTestIndex = _count;
            var lowerTestIndex = 0;

            while (upperTestIndex - lowerTestIndex > 1)
            {
                var bestGuess = (lowerTestIndex + upperTestIndex) >> 1;
                ref readonly var test = ref _terrainChanges[bestGuess];

                if (test.Frame >= frame)
                {
                    upperTestIndex = bestGuess;
                }
                else
                {
                    lowerTestIndex = bestGuess;
                }
            }

            ref readonly var test1 = ref _terrainChanges[lowerTestIndex];
            return test1.Frame >= frame
                ? lowerTestIndex
                : upperTestIndex;
        }

        public ref PixelChangeData GetNewPixelChangeDataRef()
        {
            var arraySize = _terrainChanges.Length;
            if (_count == arraySize)
            {
                var newArray = new PixelChangeData[arraySize * 2];
                new ReadOnlySpan<PixelChangeData>(_terrainChanges).CopyTo(newArray);

                _terrainChanges = newArray;
            }

            return ref _terrainChanges[_count++];
        }

        public int LatestFrameWithChange()
        {
            if (_count == 0)
                return -1;

            ref readonly var pixelChangeData = ref _terrainChanges[_count - 1];

            return pixelChangeData.Frame;
        }
    }

    private readonly struct PixelChangeData
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