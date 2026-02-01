using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager :
    IBitBufferCreator<RawBitBuffer, Lemming>,
    IPerfectHasher<HatchGroup>,
    IInitialisable,
    IDisposable
{
    private readonly LemmingManagerData _data;
    private readonly HatchGroup[] _hatchGroups;
    private readonly Lemming[] _lemmings;

    private const int RequiredNumberOfLemmingBitSets =
        2 + // spacial hash grids
        3; // lemming sets
    private readonly RawArray _lemmingByteBuffer;
    private int _bitArrayBufferUsageCount = RequiredNumberOfLemmingBitSets;

    private readonly LemmingSpacialHashGrid _lemmingSpacialHashGrid;
    private readonly LemmingSpacialHashGrid _zombieSpacialHashGrid;
    private readonly LemmingSet _lemmingsToZombify;
    private readonly LemmingSet _allBlockers;
    private readonly LemmingSet _fastForwardLemmings;

    private readonly int _totalNumberOfHatchLemmings;
    private readonly int _numberOfPreplacedLemmings;
    private readonly int _maxNumberOfClonedLemmings;

    public Lemming SimulationLemming { get; }
    private readonly RawArray _simulationLemmingDataBuffer;

    private bool _isDisposed;

    public int LemmingsToRelease => _data.LemmingsToRelease;
    public int LemmingsOut => _data.LemmingsOut;
    public int LemmingsRemoved => _data.LemmingsRemoved;
    public int LemmingsSaved => _data.LemmingsSaved;

    public ReadOnlySpan<HatchGroup> AllHatchGroups => new(_hatchGroups);
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);
    public LemmingEnumerable AllBlockers => _allBlockers.AsEnumerable();
    public Lemming GetLemming(int lemmingId) => _lemmings[lemmingId];

    public LemmingManager(
        nint dataHandle,
        HatchGroup[] hatchGroups,
        Lemming[] lemmings,
        int numberOfPreplacedLemmings,
        int maxNumberOfClonedLemmings,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _data = PointerDataHelper.CreateItem<LemmingManagerData>(ref dataHandle);

        if (hatchGroups.Length > 0)
        {
            this.AssertUniqueIds(new ReadOnlySpan<HatchGroup>(hatchGroups));
            Array.Sort(hatchGroups, this);
        }
        _hatchGroups = hatchGroups;

        _lemmings = lemmings;
        this.AssertUniqueIds(new ReadOnlySpan<Lemming>(_lemmings));
        Array.Sort(_lemmings, this);

        _lemmingSpacialHashGrid = new LemmingSpacialHashGrid(
            this,
            EngineConstants.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        _zombieSpacialHashGrid = new LemmingSpacialHashGrid(
            this,
            EngineConstants.LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmings.Length);
        _lemmingByteBuffer = Helpers.AllocateBuffer<uint>(bitBufferLength * RequiredNumberOfLemmingBitSets);

        _lemmingsToZombify = new LemmingSet(this);
        _allBlockers = new LemmingSet(this);
        _fastForwardLemmings = new LemmingSet(this);

        _totalNumberOfHatchLemmings = CalculateTotalNumberOfLemmingsFromHatches(hatchGroups);
        _numberOfPreplacedLemmings = numberOfPreplacedLemmings;
        _maxNumberOfClonedLemmings = maxNumberOfClonedLemmings;

        _simulationLemmingDataBuffer = new RawArray(LemmingData.SizeInBytes);
        var handle = _simulationLemmingDataBuffer.Handle;
        SimulationLemming = new Lemming(ref handle, EngineConstants.SimulationLemmingId);
    }

    [Pure]
    private static int CalculateTotalNumberOfLemmingsFromHatches(HatchGroup[] hatchGroups)
    {
        var result = 0;

        foreach (var group in hatchGroups)
        {
            result += group.LemmingsToRelease;
        }

        return result;
    }

    public void Initialise()
    {
        foreach (var lemming in _lemmings)
        {
            InitialiseLemming(lemming);
        }

        //controlPanelTextualData.SetHatchData(0);
        UpdateControlPanelLemmingData();
        //controlPanelTextualData.SetGoalData(0);
    }

    private void InitialiseLemming(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        lemming.Initialise();

        _lemmingSpacialHashGrid.AddItem(lemming);
        UpdateLemmingFastForwardState(lemming);

        if (lemming.State.IsZombie)
        {
            RegisterZombie(lemming);
        }
        else
        {
            _data.LemmingsOut++;
            UpdateControlPanelLemmingData();
        }
    }

    public void Tick(bool isMajorTick)
    {
        if (isMajorTick)
        {
            UpdateHatchGroups();

            foreach (var lemming in _lemmings)
            {
                if (lemming.State.IsActive)
                {
                    lemming.Tick();
                    UpdateLemmingPosition(lemming);
                }
            }
        }
        else
        {
            foreach (var lemming in _fastForwardLemmings)
            {
                if (lemming.State.IsActive)
                {
                    lemming.Tick();
                    UpdateLemmingPosition(lemming);
                }
            }
        }

        ZombifyLemmings();
    }

    private void UpdateHatchGroups()
    {
        var hatchLemmingSpan = Helpers.CreateReadOnlySpan(_lemmings, _numberOfPreplacedLemmings, _totalNumberOfHatchLemmings);
        foreach (var hatchGroup in _hatchGroups)
        {
            var hatchGadget = hatchGroup.Tick();

            if (hatchGadget is null)
                continue;

            hatchGroup.OnSpawnLemming();
            var lemming = hatchLemmingSpan[_data.NumberOfLemmingsReleasedFromHatch++];

            lemming.AnchorPosition = hatchGadget.Position + hatchGadget.SpawnPointOffset;
            hatchGadget.HatchSpawnData.SpawnLemming(lemming);
            InitialiseLemming(lemming);
        }
    }

    private void ZombifyLemmings()
    {
        if (_lemmingsToZombify.Count == 0)
            return;

        foreach (var lemming in _lemmingsToZombify)
        {
            lemming.State.IsZombie = true;
        }

        _lemmingsToZombify.Clear();
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        if (!lemming.State.IsActive)
            return;

        _lemmingSpacialHashGrid.UpdateItemPosition(lemming);
        lemming.OnUpdatePosition();

        if (lemming.State.IsZombie)
            _zombieSpacialHashGrid.UpdateItemPosition(lemming);
    }

    public void UpdateLemmingFastForwardState(Lemming lemming)
    {
        if (lemming.IsFastForward)
        {
            _fastForwardLemmings.Add(lemming);
        }
        else
        {
            _fastForwardLemmings.Remove(lemming);
        }
    }

    public void UpdateZombieState(Lemming lemming)
    {
        if (lemming.State.IsZombie)
        {
            RegisterZombie(lemming);
        }
        else
        {
            DeregisterZombie(lemming);
        }
    }

    public void RemoveLemming(Lemming lemming, LemmingRemovalReason removalReason)
    {
        lemming.State.IsActive = false;
        _lemmingSpacialHashGrid.RemoveItem(lemming);

        if (lemming.State.IsZombie)
        {
            DeregisterZombie(lemming);
        }

        if (lemming.CurrentActionId == LemmingActionConstants.BlockerActionId)
        {
            DeregisterBlocker(lemming);
        }

        if (removalReason == LemmingRemovalReason.Exit)
        {
            _data.LemmingsSaved++;
        }

        _data.LemmingsRemoved++;
        lemming.OnRemoval(removalReason);
        UpdateControlPanelLemmingData();
    }

    private void UpdateControlPanelLemmingData()
    {
        LevelScreen.LevelControlPanel.TextualData.SetLemmingData(LemmingsOut - LemmingsRemoved);
    }

    public Lemming SimulateLemming(Lemming lemming, bool checkGadgets)
    {
        var simulationLemming = SimulationLemming;
        simulationLemming.SetRawDataFromOther(lemming);

        simulationLemming.Simulate(checkGadgets);

        return simulationLemming;
    }

    public void GetAllLemmingsNearRegion(RectangularRegion levelRegion, out LemmingEnumerable result)
    {
        _lemmingSpacialHashGrid.GetAllItemsNearRegion(levelRegion, out result);
    }

    public void RegisterBlocker(Lemming lemming)
    {
        _allBlockers.Add(lemming);
    }

    public void DeregisterBlocker(Lemming lemming)
    {
        _allBlockers.Remove(lemming);
    }

    public static bool DoBlockerCheck(Lemming lemming)
    {
        BlockerAction.DoBlockerCheck(lemming);
        return true;
    }

    [Pure]
    public bool CanAssignBlocker(Lemming lemming)
    {
        var firstBounds = BlockerAction.Instance.GetLemmingBounds(lemming);

        foreach (var blocker in _allBlockers)
        {
            var secondBounds = blocker.CurrentBounds;

            if (LevelScreen.RegionsOverlap(firstBounds, secondBounds))
                return false;
        }

        return true;
    }

    #region Zombie Handling

    public void RegisterZombie(Lemming lemming) => _zombieSpacialHashGrid.AddItem(lemming);
    public void DeregisterZombie(Lemming lemming) => _zombieSpacialHashGrid.RemoveItem(lemming);

    [Pure]
    public bool AnyZombies() => !_zombieSpacialHashGrid.IsEmpty;

    public void DoZombieCheck(Lemming lemming)
    {
        Debug.Assert(!lemming.State.IsZombie);

        var checkRegion = lemming.CurrentBounds;
        _zombieSpacialHashGrid.GetAllItemsNearRegion(checkRegion, out var nearbyZombies);

        if (nearbyZombies.Count == 0)
            return;

        foreach (var zombie in nearbyZombies)
        {
            Debug.Assert(zombie.State.IsZombie);

            var zombieRegion = zombie.CurrentBounds;

            if (LevelScreen.RegionsOverlap(checkRegion, zombieRegion))
            {
                RegisterLemmingForZombification(lemming);

                return;
            }
        }
    }

    private void RegisterLemmingForZombification(Lemming lemming)
    {
        _lemmingsToZombify.Add(lemming);
    }

    #endregion

    public bool CanCreateNewLemmingClone()
    {
        return _data.NumberOfClonedLemmings < _maxNumberOfClonedLemmings;
    }

    public bool TryGetNextClonedLemming([MaybeNullWhen(false)] out Lemming clonedLemming)
    {
        if (!CanCreateNewLemmingClone())
        {
            clonedLemming = null;
            return false;
        }

        var index = _numberOfPreplacedLemmings +
                    _totalNumberOfHatchLemmings +
                    _data.NumberOfClonedLemmings;

        clonedLemming = _lemmings[index];

        _data.NumberOfClonedLemmings++;
        _data.LemmingsOut++;

        UpdateControlPanelLemmingData();

        _lemmingSpacialHashGrid.AddItem(clonedLemming);

        return true;
    }

    public int NumberOfLemmings => _lemmings.Length;
    int IPerfectHasher<Lemming>.NumberOfItems => NumberOfLemmings;
    public int HashLemming(Lemming lemming) => lemming.Id;
    int IPerfectHasher<Lemming>.Hash(Lemming lemming) => HashLemming(lemming);
    Lemming IPerfectHasher<Lemming>.UnHash(int index) => _lemmings.At(index);
    unsafe void IBitBufferCreator<RawBitBuffer, Lemming>.CreateBitBuffer(out RawBitBuffer buffer)
    {
        if (_bitArrayBufferUsageCount == 0)
            throw new InvalidOperationException("Insufficient space for bit buffers!");
        _bitArrayBufferUsageCount--;
        var bitBufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(_lemmings.Length);

        uint* pointer = (uint*)_lemmingByteBuffer.Handle + (bitBufferLength * _bitArrayBufferUsageCount);
        buffer = new RawBitBuffer(pointer, bitBufferLength);
    }

    int IPerfectHasher<HatchGroup>.NumberOfItems => _hatchGroups.Length;
    int IPerfectHasher<HatchGroup>.Hash(HatchGroup item) => item.Id;
    HatchGroup IPerfectHasher<HatchGroup>.UnHash(int index) => _hatchGroups.At(index);

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            new Span<Lemming>(_lemmings).Clear();
            new Span<HatchGroup>(_hatchGroups).Clear();
            _lemmingByteBuffer.Dispose();
            _lemmingSpacialHashGrid.Dispose();
            _zombieSpacialHashGrid.Dispose();
            _lemmingsToZombify.Clear();
            _allBlockers.Clear();
            _simulationLemmingDataBuffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    public void OnSnapshotApplied()
    {
        ResetLemmings();
    }

    private void ResetLemmings()
    {
        _lemmingSpacialHashGrid.Clear();
        _zombieSpacialHashGrid.Clear();

        foreach (var lemming in _lemmings)
        {
            lemming.OnSnapshotApplied();

            if (!lemming.State.IsActive)
                continue;

            _lemmingSpacialHashGrid.AddItem(lemming); // Just do lemming positions here - zombies are handled elsewhere
        }
    }
}
