using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.PositionTracking;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Lemmings.ZombieHelpers;

public sealed class PositionTrackingZombieHelper : IZombieHelper
{
    private readonly SpacialHashGrid<Lemming> _zombieSpacialHashGrid;

    public PositionTrackingZombieHelper(
        IPerfectHasher<Lemming> hasher,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _zombieSpacialHashGrid = new SpacialHashGrid<Lemming>(
            hasher,
            LemmingManager.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
    }

    public void RegisterZombie(Lemming lemming) => _zombieSpacialHashGrid.AddItem(lemming);
    public void UpdateZombiePosition(Lemming lemming)
    {
        if (!_zombieSpacialHashGrid.IsTrackingItem(lemming))
            return;

        _zombieSpacialHashGrid.UpdateItemPosition(lemming);
    }

    public void DeregisterZombie(Lemming lemming) => _zombieSpacialHashGrid.RemoveItem(lemming);

    public bool AnyZombies() => !_zombieSpacialHashGrid.IsEmpty;

    public void CheckZombies(Lemming lemming)
    {
        Debug.Assert(!lemming.State.IsZombie);

        var checkRegion = new LevelPositionPair(lemming.TopLeftPixel, lemming.BottomRightPixel);
        var nearbyZombies = _zombieSpacialHashGrid.GetAllItemsNearRegion(checkRegion);

        if (nearbyZombies.IsEmpty)
            return;

        foreach (var zombie in nearbyZombies)
        {
            Debug.Assert(zombie.State.IsZombie);

            var zombieRegion = new LevelPositionPair(zombie.TopLeftPixel, zombie.BottomRightPixel);

            if (checkRegion.Overlaps(zombieRegion))
            {
                LevelConstants.LemmingManager.RegisterLemmingForZombification(lemming);

                return;
            }
        }
    }
}