using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class LemmingBuilder
{
    private readonly LevelData _levelData;
    private readonly Lemming[] _lemmingList;

    public LemmingBuilder(LevelData levelData)
    {
        _levelData = levelData;

        var requiredCapacity = _levelData.CalculateTotalNumberOfLemmingsInLevel();

        _lemmingList = new Lemming[requiredCapacity];
    }

    public Lemming[] BuildLevelLemmings()
    {
        var i = 0;

        AddLemmings(ref i, CollectionsMarshal.AsSpan(_levelData.PrePlacedLemmingData));

        while (i < _lemmingList.Length)
        {
            var lemming = new Lemming(
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
        }

        return _lemmingList;
    }

    private void AddLemmings(ref int index, ReadOnlySpan<LemmingInstanceData> lemmingDataSpan)
    {
        foreach (var prototype in lemmingDataSpan)
        {
            var lemming = new Lemming(
                index,
                prototype.Orientation,
                prototype.FacingDirection,
                prototype.InitialLemmingActionId,
                prototype.TribeId);

            lemming.AnchorPosition = prototype.Position;

            lemming.State.SetData(prototype.TribeId, prototype.State);

            _lemmingList[index] = lemming;
            index++;
        }
    }
}
