using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Rewind;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LemmingBuilder
{
    private readonly LevelData _levelData;
    private readonly Lemming[] _levelLemmings;
    private readonly RawArray _lemmingDataBuffer;

    public LemmingBuilder(LevelData levelData, SafeBufferAllocator safeBufferAllocator)
    {
        _levelData = levelData;

        var numberOfLemmings = _levelData.CalculateTotalNumberOfLemmingsInLevel();

        _levelLemmings = new Lemming[numberOfLemmings];
        _lemmingDataBuffer = safeBufferAllocator.AllocateRawArray<LemmingData>(numberOfLemmings);
    }

    public RawArray LemmingDataBuffer => _lemmingDataBuffer;

    public Lemming[] BuildLevelLemmings()
    {
        var i = 0;
        var dataHandle = new DataHandle(_lemmingDataBuffer);
        var preplacedLemmingData = _levelData.PrePlacedLemmingData;

        Debug.Assert(preplacedLemmingData.Count <= _levelLemmings.Length);

        while (i < preplacedLemmingData.Count)
        {
            var prototype = preplacedLemmingData[i];

            var lemming = Lemming.CreatePrePlacedLemming(dataHandle, i, prototype);

            _levelLemmings.At(i++) = lemming;
        }

        while (i < _levelLemmings.Length)
        {
            var lemming = Lemming.CreateHatchLemming(dataHandle, i);

            _levelLemmings.At(i++) = lemming;
        }

        return _levelLemmings;
    }
}
