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
    private readonly RectangularLevelRegion _position;

    private readonly ILevelRegion _levelRegion;
    private readonly LargeBitArray _lemmingIdsInHitBox;
    private readonly LargeSimpleSet<LemmingAction> _targetActions;
    private readonly SmallSimpleSet<Orientation> _targetOrientations;
    private readonly SmallSimpleSet<FacingDirection> _targetFacingDirections;

    public bool IsEnabled { get; set; }
    public bool InteractsWithLemming => true;

    public LevelRegionHitBoxBehaviour(
        RectangularLevelRegion position,
        ILevelRegion levelRegion,
        int totalNumberOfLemmings,
        LargeSimpleSet<LemmingAction> targetActions,
        SmallSimpleSet<Orientation> targetOrientations,
        SmallSimpleSet<FacingDirection> targetFacingDirections)
    {
        _position = position;
        _levelRegion = levelRegion;
        _lemmingIdsInHitBox = new LargeBitArray(totalNumberOfLemmings);
        _targetActions = targetActions;
        _targetOrientations = targetOrientations;
        _targetFacingDirections = targetFacingDirections;
    }

    public bool MatchesLemming(Lemming lemming) => MatchesLemmingData(lemming) &&
                                                   MatchesLemmingPosition(lemming);

    public bool MatchesPosition(LevelPosition levelPosition) => _levelRegion.ContainsPoint(levelPosition);

    private bool MatchesLemmingData(Lemming lemming) => _targetFacingDirections.Contains(lemming.FacingDirection) &&
                                                        _targetOrientations.Contains(lemming.Orientation) &&
                                                        _targetActions.Contains(lemming.CurrentAction);

    private bool MatchesLemmingPosition(Lemming lemming)
    {
        var offset = _position.TopLeft;
        var position1 = lemming.LevelPosition - offset;
        var position2 = lemming.Orientation.MoveUp(position1, 1) - offset;

        return _levelRegion.ContainsPoint(position1) ||
               _levelRegion.ContainsPoint(position2);
    }

    public void OnLemmingEnterHitBox(Lemming lemming)
    {
    }

    public void OnLemmingInHitBox(Lemming lemming)
    {
        //if()
    }
}