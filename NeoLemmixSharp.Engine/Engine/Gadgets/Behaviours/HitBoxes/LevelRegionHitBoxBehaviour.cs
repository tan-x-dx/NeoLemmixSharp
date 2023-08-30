﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.LemmingActions;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.HitBoxes;

public sealed class LevelRegionHitBoxBehaviour : IHitBoxBehaviour
{
    private readonly ILevelRegion _levelRegion;
    private readonly LargeBitArray _lemmingIdsInHitBox;
    private readonly LargeSimpleSet<LemmingAction> _targetActions = SimpleSetHelpers.LargeSetForUniqueItemType<LemmingAction>(true);
    private readonly SmallSimpleSet<Orientation> _targetOrientations = SimpleSetHelpers.SmallSetForUniqueItemType<Orientation>(true);
    private readonly SmallSimpleSet<FacingDirection> _targetFacingDirections = SimpleSetHelpers.SmallSetForUniqueItemType<FacingDirection>(true);

    public bool IsEnabled { get; set; }
    public bool InteractsWithLemming => true;

    public LevelRegionHitBoxBehaviour(
        ILevelRegion levelRegion,
        int totalNumberOfLemmings)
    {
        _levelRegion = levelRegion;
        _lemmingIdsInHitBox = new LargeBitArray(totalNumberOfLemmings);
    }

    public bool MatchesLemming(Lemming lemming) => MatchesLemmingData(lemming) &&
                                                   MatchesLemmingPosition(lemming);

    private bool MatchesLemmingData(Lemming lemming) => _targetFacingDirections.Contains(lemming.FacingDirection) &&
                                                        _targetOrientations.Contains(lemming.Orientation) &&
                                                        _targetActions.Contains(lemming.CurrentAction);

    private bool MatchesLemmingPosition(Lemming lemming)
    {
        var position1 = lemming.LevelPosition;
        var position2 = lemming.Orientation.MoveUp(lemming.LevelPosition, 1);

        return _levelRegion.ContainsPoint(position1) ||
               _levelRegion.ContainsPoint(position2);
    }

    public bool MatchesPosition(LevelPosition levelPosition) => _levelRegion.ContainsPoint(levelPosition);

    public void OnLemmingInHitBox(Lemming lemming)
    {
        if (_lemmingIdsInHitBox.SetBit(lemming.Id))
        {

        }
        else
        {

        }
    }

    public void OnLemmingNotInHitBox(Lemming lemming)
    {
        _lemmingIdsInHitBox.ClearBit(lemming.Id);
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