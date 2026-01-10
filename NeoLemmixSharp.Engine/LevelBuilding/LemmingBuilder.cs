using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LemmingBuilder
{
    private readonly LevelData _levelData;
    private readonly Lemming[] _lemmingList;
    private readonly RawArray _lemmingDataBuffer;

    public LemmingBuilder(LevelData levelData, SafeBufferAllocator safeBufferAllocator)
    {
        _levelData = levelData;

        var numberOfLemmings = _levelData.CalculateTotalNumberOfLemmingsInLevel();

        _lemmingList = new Lemming[numberOfLemmings];
        _lemmingDataBuffer = safeBufferAllocator.AllocateRawArray(numberOfLemmings * LemmingData.SizeOfLemmingDataInBytes);
    }

    public RawArray LemmingDataBuffer => _lemmingDataBuffer;

    public Lemming[] BuildLevelLemmings()
    {
        var i = 0;
        nint handle = _lemmingDataBuffer.Handle;

        for (; i < _levelData.PrePlacedLemmingData.Count; i++)
        {
            var prototype = _levelData.PrePlacedLemmingData[i];

            var lemming = new Lemming(
                handle,
                i,
                prototype.Orientation,
                prototype.FacingDirection,
                prototype.InitialLemmingActionId,
                prototype.TribeId)
            {
                AnchorPosition = prototype.Position
            };

            lemming.State.SetData(prototype.TribeId, prototype.State);

            _lemmingList[i] = lemming;
            handle += LemmingData.SizeOfLemmingDataInBytes;
        }

        while (i < _lemmingList.Length)
        {
            var lemming = new Lemming(
                handle,
                i,
                Orientation.Down,
                FacingDirection.Right,
                LemmingActionConstants.NoneActionId,
                EngineConstants.ClassicTribeId)
            {
                AnchorPosition = new Point()
            };

            _lemmingList[i] = lemming;

            i++;
            handle += LemmingData.SizeOfLemmingDataInBytes;
        }

        return _lemmingList;
    }
}
