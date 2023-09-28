using System.Runtime.CompilerServices;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class HitBox
{
    private readonly ILevelRegion _levelRegion;
    private readonly ItemTracker<Lemming> _lemmingsInHitBox;
    private readonly LargeSimpleSet<LemmingAction> _targetActions = ExtendedEnumTypeComparer<LemmingAction>.LargeSetForType();
    private readonly SmallSimpleSet<Orientation> _targetOrientations = ExtendedEnumTypeComparer<Orientation>.SmallSetForType();
    private readonly SmallSimpleSet<FacingDirection> _targetFacingDirections = ExtendedEnumTypeComparer<FacingDirection>.SmallSetForType();

    public HitBox(
        ILevelRegion levelRegion,
        ISimpleHasher<Lemming> lemmingHasher)
    {
        _levelRegion = levelRegion;
        _lemmingsInHitBox = new ItemTracker<Lemming>(lemmingHasher);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Tick() => _lemmingsInHitBox.Tick();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesLemming(Lemming lemming)
    {
        var anchorPosition = lemming.LevelPosition;
        var footPosition = lemming.FootPosition;

        return MatchesLemmingData(lemming) &&
               (MatchesPosition(anchorPosition) ||
                MatchesPosition(footPosition));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return MatchesLemmingData(lemming) &&
               MatchesPosition(levelPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesLemmingData(Lemming lemming) => _targetFacingDirections.Contains(lemming.FacingDirection) &&
                                                       _targetOrientations.Contains(lemming.Orientation) &&
                                                       _targetActions.Contains(lemming.CurrentAction);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MatchesPosition(LevelPosition levelPosition) => _levelRegion.ContainsPoint(levelPosition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int OnLemmingInHitBox(Lemming lemming)
    {
        return _lemmingsInHitBox.EvaluateItem(lemming);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncludeAction(LemmingAction lemmingAction)
    {
        _targetActions.Add(lemmingAction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExcludeAction(LemmingAction lemmingAction)
    {
        _targetActions.Remove(lemmingAction);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncludeOrientation(Orientation orientation)
    {
        _targetOrientations.Add(orientation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExcludeOrientation(Orientation orientation)
    {
        _targetOrientations.Remove(orientation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IncludeFacingDirection(FacingDirection facingDirection)
    {
        _targetFacingDirections.Add(facingDirection);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExcludeFacingDirection(FacingDirection facingDirection)
    {
        _targetFacingDirections.Remove(facingDirection);
    }
}