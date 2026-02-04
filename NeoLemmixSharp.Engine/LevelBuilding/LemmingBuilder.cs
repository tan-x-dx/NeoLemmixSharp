using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;

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
        _lemmingDataBuffer = safeBufferAllocator.AllocateRawArray(numberOfLemmings * LemmingData.SizeInBytes);
    }

    public RawArray LemmingDataBuffer => _lemmingDataBuffer;

    public Lemming[] BuildLevelLemmings()
    {
        var i = 0;
        nint handle = _lemmingDataBuffer.Handle;

        while (i < _levelData.PrePlacedLemmingData.Count)
        {
            var prototype = _levelData.PrePlacedLemmingData[i];

            var lemming = new Lemming(ref handle, i);

            lemming.CurrentAction = LemmingAction.GetActionOrDefault(prototype.InitialLemmingActionId);
            lemming.AnchorPosition = prototype.Position;
            lemming.SetRawData(prototype.Orientation, prototype.FacingDirection, prototype.TribeId, prototype.State);

            _levelLemmings.At(i++) = lemming;
        }

        while (i < _levelLemmings.Length)
        {
            var lemming = new Lemming(ref handle, i);

            lemming.AnchorPosition = default;
            lemming.Orientation = Orientation.Down;
            lemming.FacingDirection = FacingDirection.Right;
            lemming.CurrentAction = NoneAction.Instance;

            _levelLemmings.At(i++) = lemming;
        }

        return _levelLemmings;
    }
}
