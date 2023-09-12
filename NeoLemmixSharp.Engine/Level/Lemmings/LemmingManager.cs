using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>
{
    private const ChunkSizeType LemmingPositionChunkSize = ChunkSizeType.ChunkSize32;

    private readonly PositionHelper<Lemming> _lemmingPositionHelper;
    private readonly PositionHelper<Lemming> _blockerPositionHelper;
    private readonly LargeSimpleSet<Lemming> _activeLemmings;
    private readonly LargeSimpleSet<Lemming> _blockerScratchSpace;
    private readonly Lemming[] _lemmings;

    public int LemmingsToRelease { get; private set; }
    public int LemmingsOut { get; private set; }
    public int LemmingsRemoved { get; private set; }

    public int TotalNumberOfLemmings => _lemmings.Length;
    public ReadOnlySpan<Lemming> AllLemmings => new(_lemmings);

    public LargeSimpleSet<Lemming>.Enumerator ActiveLemmingsEnumerator => _activeLemmings.GetEnumerator();

    public LemmingManager(
        Lemming[] lemmings,
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour)
    {
        _lemmings = lemmings;
        Array.Sort(_lemmings, IdEquatableItemHelperMethods.Compare);
        _lemmings.ValidateUniqueIds();

        _lemmingPositionHelper = new PositionHelper<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _blockerPositionHelper = new PositionHelper<Lemming>(
            this,
            LemmingPositionChunkSize,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);

        _activeLemmings = new LargeSimpleSet<Lemming>(this);
        _blockerScratchSpace = new LargeSimpleSet<Lemming>(this);
    }

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
        _activeLemmings.Add(lemming);

        if (!lemming.State.IsZombie)
        {
            LemmingsOut++;
        }

        _lemmingPositionHelper.AddItem(lemming);
    }

    public bool LemmingIsActive(Lemming lemming) => _activeLemmings.Contains(lemming);

    public void RemoveLemming(Lemming lemming)
    {
        _activeLemmings.Remove(lemming);
        _lemmingPositionHelper.RemoveItem(lemming);
        LemmingsRemoved++;
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        _lemmingPositionHelper.UpdateItemPosition(lemming);
    }

    public void PopulateSetWithLemmingsNearRegion(
        LargeSimpleSet<Lemming> set,
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        _lemmingPositionHelper.PopulateSetWithItemsNearRegion(set, topLeftLevelPosition, bottomRightLevelPosition);
    }

    public void RegisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.AddItem(lemming);
    }

    public void DeregisterBlocker(Lemming lemming)
    {
        _blockerPositionHelper.RemoveItem(lemming);
    }

    public LargeSimpleSet<Lemming>.Enumerator BlockersNearLemmingEnumerator(Lemming lemming)
    {
        _blockerScratchSpace.Clear();
        _blockerPositionHelper.PopulateSetWithItemsNearRegion(_blockerScratchSpace, lemming.TopLeftPixel, lemming.BottomRightPixel);

        return _blockerScratchSpace.GetEnumerator();
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.Unhash(int index) => _lemmings[index];
}