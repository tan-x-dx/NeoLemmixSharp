using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings.BlockerHelpers;
using NeoLemmixSharp.Engine.Level.Lemmings.ZombieHelpers;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>, ISimpleHasher<HatchGroup>
{
    public const int BlockerQuantityThreshold = 20;
    public const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize32;

    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private readonly SpacialHashGrid<Lemming> _lemmingPositionHelper;

    private readonly IBlockerHelper _blockerHelper;
    private readonly IZombieHelper _zombieHelper;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    public LemmingManager(
        LevelData levelData,
        Lemming[] lemmings,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _lemmings = lemmings;
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);
        _lemmings.ValidateUniqueIds();

        _lemmingPositionHelper = new SpacialHashGrid<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _blockerHelper = GetBlockerHelper(levelData, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
        _zombieHelper = GetZombieHelper(levelData, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);
    }

    private IBlockerHelper GetBlockerHelper(
        LevelData levelData,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        var maxNumberOfBlockers = levelData.GetMaxNumberOfBlockers();

        return maxNumberOfBlockers switch
        {
            null => new PositionTrackingBlockerHelper(this, horizontalBoundaryBehaviour, verticalBoundaryBehaviour),
            0 => new EmptyBlockerHelper(),
            > BlockerQuantityThreshold => new PositionTrackingBlockerHelper(this, horizontalBoundaryBehaviour, verticalBoundaryBehaviour),
            _ => new SmallListBlockerHelper(maxNumberOfBlockers.Value)
        };
    }

    private IZombieHelper GetZombieHelper(
        LevelData levelData,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        var levelHasZombies = levelData.LevelContainsAnyZombies();

        if (levelHasZombies)
            return new PositionTrackingZombieHelper(this, horizontalBoundaryBehaviour, verticalBoundaryBehaviour);

        return new EmptyZombieHelper();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialise()
    {
        foreach (var lemming in AllLemmings)
        {
            InitialiseLemming(lemming);
        }
    }

    private void InitialiseLemming(Lemming lemming)
    {
        if (lemming.CurrentAction == NoneAction.Instance)
            return;

        lemming.Initialise();

        _lemmingPositionHelper.AddItem(lemming);

        if (!lemming.State.IsZombie)
        {
            LemmingsOut++;
        }

        lemming.OnInitialization();
    }

    public void RemoveLemming(Lemming lemming)
    {
        lemming.State.IsActive = false;
        _lemmingPositionHelper.RemoveItem(lemming);

        if (lemming.State.IsZombie)
        {
            DeregisterZombie(lemming);
        }

        if (lemming.CurrentAction == BlockerAction.Instance)
        {
            DeregisterBlocker(lemming);
        }

        LemmingsRemoved++;
        lemming.OnRemoval();
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        _lemmingPositionHelper.UpdateItemPosition(lemming);

        // The helpers will deal with these updates.
        // If not relevant for this lemming, nothing will happen
        _blockerHelper.UpdateBlockerPosition(lemming);
        _zombieHelper.UpdateZombiePosition(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SimpleSet<Lemming> GetAllLemmingsNearRegion(LevelPositionPair levelRegion) => _lemmingPositionHelper.GetAllItemsNearRegion(levelRegion);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterBlocker(Lemming lemming) => _blockerHelper.RegisterBlocker(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeregisterBlocker(Lemming lemming) => _blockerHelper.DeregisterBlocker(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DoBlockerCheck(Lemming lemming) => _blockerHelper.CheckBlockers(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanAssignBlocker(Lemming lemming) => _blockerHelper.CanAssignBlocker(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterZombie(Lemming lemming) => _zombieHelper.RegisterZombie(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DeregisterZombie(Lemming lemming) => _zombieHelper.DeregisterZombie(lemming);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DoZombieCheck(Lemming lemming) => _zombieHelper.CheckZombies(lemming);

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.UnHash(int index) => _lemmings[index];

    int ISimpleHasher<HatchGroup>.NumberOfItems => _hatchGroups.Length;
    int ISimpleHasher<HatchGroup>.Hash(HatchGroup item) => item.Id;
    HatchGroup ISimpleHasher<HatchGroup>.UnHash(int index) => _hatchGroups[index];
}