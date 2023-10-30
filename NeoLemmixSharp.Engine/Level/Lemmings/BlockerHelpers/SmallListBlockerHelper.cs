using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.Level.Lemmings.BlockerHelpers;

public sealed class SmallListBlockerHelper : IBlockerHelper
{
    private readonly SimpleList<Lemming> _blockerList;

    public SmallListBlockerHelper(int maxPossibleNumberOfBlockers)
    {
        _blockerList = new SimpleList<Lemming>(maxPossibleNumberOfBlockers);
    }

    public void RegisterBlocker(Lemming lemming) => _blockerList.Add(lemming);
    public void UpdateBlockerPosition(Lemming lemming) { } // Don't need to update blocker position
    public bool LemmingIsBlocking(Lemming lemming) => _blockerList.Contains(lemming);
    public void DeregisterBlocker(Lemming lemming) => _blockerList.Remove(lemming);

    public bool CanAssignBlocker(Lemming lemming)
    {
        if (_blockerList.Count == 0)
            return true;

        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        var nearbyBlockers = _blockerList.AsSpan();

        foreach (var blocker in nearbyBlockers)
        {
            var blockerTopLeft = blocker.TopLeftPixel;
            var blockerBottomRight = blocker.BottomRightPixel;

            var secondBounds = new LevelPositionPair(blockerTopLeft, blockerBottomRight);

            if (firstBounds.Overlaps(secondBounds))
                return false;
        }

        return true;
    }

    public void CheckBlockers(Lemming lemming)
    {
        if (_blockerList.Count == 0 ||
            lemming.CurrentAction == BlockerAction.Instance ||
            lemming.CurrentAction == JumperAction.Instance ||
            (lemming.CurrentAction == MinerAction.Instance && (lemming.PhysicsFrame == 1 || lemming.PhysicsFrame == 2)))
            return;

        var anchorPosition = lemming.LevelPosition;
        var footPosition = lemming.FootPosition;

        var blockers = _blockerList.AsSpan();

        foreach (var blocker in blockers)
        {
            if (BlockerAction.DoBlockerCheck(blocker, lemming, anchorPosition, footPosition))
                return;
        }
    }
}