using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

public sealed class LevelRegionHitBox : IHitBox
{
    private readonly ILevelRegion _levelRegion;
    private readonly LargeBitArray _lemmingIdsInHitBox;
    private readonly LargeSimpleSet<LemmingAction> _targetActions;
    private readonly SmallSimpleSet<Orientation> _targetOrientations;
    private readonly SmallSimpleSet<FacingDirection> _targetFacingDirections;

    public LevelRegionHitBox(
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

    public bool IsEmpty => _levelRegion.IsEmpty;

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