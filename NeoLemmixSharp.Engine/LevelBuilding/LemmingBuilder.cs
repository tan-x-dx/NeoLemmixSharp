using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.IO.Data.Level;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public ref struct LemmingBuilder
{
    private readonly LevelData _levelData;
    private ArrayListWrapper<Lemming> _lemmingList;

    public LemmingBuilder(LevelData levelData)
    {
        _levelData = levelData;

        var totalCapacity = _levelData.MaxNumberOfClonedLemmings +
                            _levelData.HatchLemmingData.Count +
                            _levelData.PrePlacedLemmingData.Count;

        _lemmingList = new ArrayListWrapper<Lemming>(totalCapacity);
    }

    public Lemming[] BuildLevelLemmings()
    {
        AddLemmings(CollectionsMarshal.AsSpan(_levelData.PrePlacedLemmingData));
        AddLemmings(CollectionsMarshal.AsSpan(_levelData.HatchLemmingData));

        var i = _levelData.MaxNumberOfClonedLemmings;
        while (i-- > 0)
        {
            var lemming = new Lemming(
                _lemmingList.Count,
                Orientation.Down,
                FacingDirection.Right,
                LemmingActionConstants.NoneActionId,
                EngineConstants.ClassicTribeId)
            {
                AnchorPosition = new Point()
            };
            _lemmingList.Add(lemming);
        }

        return _lemmingList.GetArray();
    }

    private void AddLemmings(ReadOnlySpan<LemmingData> lemmingDataSpan)
    {
        foreach (var prototype in lemmingDataSpan)
        {
            var lemming = new Lemming(
                _lemmingList.Count,
                prototype.Orientation,
                prototype.FacingDirection,
                prototype.InitialLemmingActionId,
                prototype.TribeId)
            {
                AnchorPosition = prototype.Position
            };

            lemming.State.SetRawDataFromOther(prototype.State);

            _lemmingList.Add(lemming);
        }
    }
}
