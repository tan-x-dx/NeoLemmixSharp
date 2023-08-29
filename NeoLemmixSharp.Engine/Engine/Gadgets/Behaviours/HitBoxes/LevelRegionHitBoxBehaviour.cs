using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.HitBoxes;

public sealed class LevelRegionHitBoxBehaviour : IHitBoxBehaviour
{
    private readonly ILevelRegion _levelRegion;
    private readonly LargeBitArray _lemmingIdsInHitBox;
    private readonly LargeSimpleSet<LemmingAction> _targetActions;
    private readonly SmallSimpleSet<Orientation> _targetOrientations;
    private readonly SmallSimpleSet<FacingDirection> _targetFacingDirections;

    public LevelRegionHitBoxBehaviour(
        ILevelRegion levelRegion,
        LargeSimpleSet<LemmingAction> targetActions,
        SmallSimpleSet<Orientation> targetOrientations,
        SmallSimpleSet<FacingDirection> targetFacingDirections)
    {
        _levelRegion = levelRegion;
        _targetActions = targetActions;
        _targetOrientations = targetOrientations;
        _targetFacingDirections = targetFacingDirections;
    }

    public bool MatchesLemming(Lemming lemming, LevelPosition levelPosition)
    {
        return _levelRegion.ContainsPoint(levelPosition) &&
               _targetActions.Contains(lemming.CurrentAction) &&
               _targetOrientations.Contains(lemming.Orientation) &&
               _targetFacingDirections.Contains(lemming.FacingDirection);
    }

    public void OnLemmingInHitBox(Lemming lemming)
    {
        //if()
    }
}