using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.States;

public sealed class HitBox
{
    private readonly ILevelRegion _levelRegion;
    private readonly LargeSimpleSet<Lemming> _lemmingsInHitBox;
    private readonly LargeSimpleSet<LemmingAction> _targetActions = ExtendedEnumTypeComparer<LemmingAction>.LargeSetForType();
    private readonly SmallSimpleSet<Orientation> _targetOrientations = ExtendedEnumTypeComparer<Orientation>.SmallSetForType();
    private readonly SmallSimpleSet<FacingDirection> _targetFacingDirections = ExtendedEnumTypeComparer<FacingDirection>.SmallSetForType();

    public HitBox(
        ILevelRegion levelRegion,
        ISimpleHasher<Lemming> lemmingHasher)
    {
        _levelRegion = levelRegion;
        _lemmingsInHitBox = new LargeSimpleSet<Lemming>(lemmingHasher);
    }

    public bool MatchesLemming(Lemming lemming) => MatchesLemmingData(lemming) &&
                                                   MatchesLemmingPosition(lemming);

    public bool MatchesLemmingData(Lemming lemming) => _targetFacingDirections.Contains(lemming.FacingDirection) &&
                                                       _targetOrientations.Contains(lemming.Orientation) &&
                                                       _targetActions.Contains(lemming.CurrentAction);

    private bool MatchesLemmingPosition(Lemming lemming)
    {
        var anchorPosition = lemming.LevelPosition;
        var footPosition = lemming.FootPosition;

        return _levelRegion.ContainsPoint(anchorPosition) ||
               _levelRegion.ContainsPoint(footPosition);
    }

    public bool MatchesPosition(LevelPosition levelPosition) => _levelRegion.ContainsPoint(levelPosition);

    public void OnLemmingInHitBox(Lemming lemming)
    {
        if (_lemmingsInHitBox.Contains(lemming))
        {

        }
        else
        {

        }
    }

    public void OnLemmingNotInHitBox(Lemming lemming)
    {
        _lemmingsInHitBox.Remove(lemming);
    }

    public void IncludeAction(LemmingAction lemmingAction)
    {
        _targetActions.Add(lemmingAction);
    }

    public void ExcludeAction(LemmingAction lemmingAction)
    {
        _targetActions.Remove(lemmingAction);
    }

    public void IncludeOrientation(Orientation orientation)
    {
        _targetOrientations.Add(orientation);
    }

    public void ExcludeOrientation(Orientation orientation)
    {
        _targetOrientations.Remove(orientation);
    }

    public void IncludeFacingDirection(FacingDirection facingDirection)
    {
        _targetFacingDirections.Add(facingDirection);
    }

    public void ExcludeFacingDirection(FacingDirection facingDirection)
    {
        _targetFacingDirections.Remove(facingDirection);
    }
}