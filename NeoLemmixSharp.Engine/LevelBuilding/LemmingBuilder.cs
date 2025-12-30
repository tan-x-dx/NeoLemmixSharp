using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;

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

        for (; i < _levelData.PrePlacedLemmingData.Count; i++)
        {
            var prototype = _levelData.PrePlacedLemmingData[i];

            var lemming = new Lemming(
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
        }

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
}
