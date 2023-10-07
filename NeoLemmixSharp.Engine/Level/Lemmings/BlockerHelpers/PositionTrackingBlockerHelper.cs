using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.Level.Lemmings.BlockerHelpers;

public sealed class PositionTrackingBlockerHelper : IBlockerHelper
{
    private readonly SpacialHashGrid<Lemming> _blockerSpacialHashGrid;

    public PositionTrackingBlockerHelper(
        IPerfectHasher<Lemming> hasher,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _blockerSpacialHashGrid = new SpacialHashGrid<Lemming>(
            hasher,
            LemmingManager.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    public void RegisterBlocker(Lemming lemming) => _blockerSpacialHashGrid.AddItem(lemming);
    public void UpdateBlockerPosition(Lemming lemming)
    {
        if (!_blockerSpacialHashGrid.IsTrackingItem(lemming))
            return;

        _blockerSpacialHashGrid.UpdateItemPosition(lemming);
    }

    public bool LemmingIsBlocking(Lemming lemming) => _blockerSpacialHashGrid.IsTrackingItem(lemming);
    public void DeregisterBlocker(Lemming lemming) => _blockerSpacialHashGrid.RemoveItem(lemming);

    public bool CanAssignBlocker(Lemming lemming)
    {
        if (_blockerSpacialHashGrid.IsEmpty)
            return true;

        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        var nearbyBlockers = _blockerSpacialHashGrid.GetAllItemsNearRegion(firstBounds);

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
        if (lemming.CurrentAction == BlockerAction.Instance)
            return;

        var anchorPosition = lemming.LevelPosition;
        var footPosition = lemming.FootPosition;

        var checkRegion = new LevelPositionPair(anchorPosition, footPosition);
        var blockerSet = _blockerSpacialHashGrid.GetAllItemsNearRegion(checkRegion);

        foreach (var blocker in blockerSet)
        {
            if (BlockerAction.DoBlockerCheck(blocker, lemming, anchorPosition, footPosition))
                return;
        }
    }
}