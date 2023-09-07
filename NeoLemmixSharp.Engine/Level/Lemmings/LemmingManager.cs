using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.PositionTracking;
using NeoLemmixSharp.Engine.Level.LemmingActions;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class LemmingManager : ISimpleHasher<Lemming>
{
    private readonly PositionHelper<Lemming> _lemmingPositionHelper;
    private readonly LargeSimpleSet<Lemming> _activeLemmings;
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
            lemmings,
            this,
            ChunkSizeType.ChunkSize32,
            horizontalBoundaryBehaviour,
            verticalBoundaryBehaviour);
        _activeLemmings = new LargeSimpleSet<Lemming>(this);
    }

    public void Activate()
    {
        foreach (var lemming in AllLemmings)
        {
            if (lemming.CurrentAction != NoneAction.Instance)
            {
                lemming.Activate();
                _activeLemmings.Add(lemming);
                _lemmingPositionHelper.UpdateItemPosition(lemming, true);
            }
        }
    }

    public bool LemmingIsActive(Lemming lemming) => _activeLemmings.Contains(lemming);

    public void RemoveLemming(Lemming lemming)
    {
        _activeLemmings.Remove(lemming);
        _lemmingPositionHelper.RemoveItem(lemming);
    }

    public void UpdateLemmingPosition(Lemming lemming)
    {
        _lemmingPositionHelper.UpdateItemPosition(lemming, false);
    }

    public void PopulateSetWithLemmingsNearRegion(
        LargeSimpleSet<Lemming> set,
        LevelPosition topLeftLevelPosition,
        LevelPosition bottomRightLevelPosition)
    {
        _lemmingPositionHelper.PopulateSetWithItemsNearRegion(set, topLeftLevelPosition, bottomRightLevelPosition);
    }

    int ISimpleHasher<Lemming>.NumberOfItems => _lemmings.Length;
    int ISimpleHasher<Lemming>.Hash(Lemming item) => item.Id;
    Lemming ISimpleHasher<Lemming>.Unhash(int index) => _lemmings[index];
}