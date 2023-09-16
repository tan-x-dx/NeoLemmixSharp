using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelCursor
{
    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelInputController _controller;
    private readonly LemmingManager _lemmingManager;
    private readonly SkillSetManager _skillSetManager;

    private FacingDirection? _facingDirection;
    private bool _selectOnlyWalkers;
    private bool _selectOnlyUnassigned;

    private int _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre;

    public int NumberOfLemmingsUnderCursor { get; private set; }
    public LevelPosition CursorPosition { private get; set; }

    public Lemming? CurrentlyHighlightedLemming { get; private set; }

    public LevelCursor(
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour,
        ILevelControlPanel controlPanel,
        LevelInputController controller,
        LemmingManager lemmingManager,
        SkillSetManager skillSetManager)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _controlPanel = controlPanel;
        _controller = controller;
        _lemmingManager = lemmingManager;
        _skillSetManager = skillSetManager;
    }

    public void OnNewFrame()
    {
        NumberOfLemmingsUnderCursor = 0;
        CurrentlyHighlightedLemming = null;
        _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre = int.MaxValue;

        _selectOnlyWalkers = _controller.SelectOnlyWalkers.IsKeyDown;
        _selectOnlyUnassigned = _controller.SelectOnlyUnassignedLemmings.IsKeyDown;

        if (_controller.SelectLeftFacingLemmings.IsKeyDown)
        {
            _facingDirection = LeftFacingDirection.Instance;
        }
        else if (_controller.SelectRightFacingLemmings.IsKeyDown)
        {
            _facingDirection = RightFacingDirection.Instance;
        }
        else
        {
            _facingDirection = null;
        }
    }

    public LargeSimpleSet<Lemming>.Enumerator LemmingsNearCursorPosition()
    {
        var topLeftCursorPosition = CursorPosition + new LevelPosition(-7, -7);
        var bottomRightCursorPosition = CursorPosition + new LevelPosition(6, 6);

        return _lemmingManager.GetAllLemmingsNearRegion(topLeftCursorPosition, bottomRightCursorPosition);
    }

    public void CheckLemming(Lemming lemming)
    {
        if (!LemmingIsUnderCursor(lemming) || !LemmingIsAbleToBeSelected(lemming))
            return;

        NumberOfLemmingsUnderCursor++;
        if (NewCandidateIsHigherPriority(CurrentlyHighlightedLemming, lemming))
        {
            CurrentlyHighlightedLemming = lemming;
        }
    }

    private bool LemmingIsUnderCursor(Lemming lemming)
    {
        var lemmingPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, 4);

        var dx = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.X, lemmingPosition.X);
        var dy = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.Y, lemmingPosition.Y);

        return dx < 5 && dy < 5;
    }

    private bool LemmingIsAbleToBeSelected(Lemming lemming)
    {
        // Directional select
        if (_facingDirection is not null &&
            lemming.FacingDirection != _facingDirection &&
            !(false))//and(not(IsHighlight or IsReplay))
            return false;

        // Select only walkers
        if (_selectOnlyWalkers && lemming.CurrentAction != WalkerAction.Instance &&
            !(false)) //and(not(IsHighlight or IsReplay))
            return false;

        // Select only unassigned
        if (_selectOnlyUnassigned && lemming.State.HasPermanentSkill)
            return false;

        return true;
    }

    private bool NewCandidateIsHigherPriority(Lemming? previousCandidate, Lemming newCandidate)
    {
        if (previousCandidate is null)
        {
            _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre = GetDistanceSquaredFromCursorCentre(newCandidate);

            return true;
        }

        return NewCandidateHasMoreRelevantState(previousCandidate, newCandidate) ||
               NewCandidateHasMoreRelevantTeam(previousCandidate, newCandidate) ||
               NewCandidateHasHigherActionPriority(previousCandidate, newCandidate) ||
               NewCandidateIsCloserToCursorCentre(newCandidate);
    }

    private static bool NewCandidateHasMoreRelevantState(Lemming previousCandidate, Lemming newCandidate)
    {
        return (previousCandidate.State.IsNeutral && !newCandidate.State.IsNeutral) ||
               (previousCandidate.State.IsZombie && !newCandidate.State.IsZombie);
    }

    private bool NewCandidateHasMoreRelevantTeam(Lemming previousCandidate, Lemming newCandidate)
    {
        var skillTrackingData = _skillSetManager.GetSkillTrackingData(_controlPanel.SelectedSkillButtonId);
        if (skillTrackingData is null)
            return false;

        var previousCandidateMatchesTeam = previousCandidate.State.TeamAffiliation == skillTrackingData.Team;
        var newCandidateMatchesTeam = newCandidate.State.TeamAffiliation == skillTrackingData.Team;

        return newCandidateMatchesTeam && !previousCandidateMatchesTeam;
    }

    private static bool NewCandidateHasHigherActionPriority(Lemming previousCandidate, Lemming newCandidate)
    {
        return newCandidate.CurrentAction.CursorSelectionPriorityValue >
               previousCandidate.CurrentAction.CursorSelectionPriorityValue;
    }

    private bool NewCandidateIsCloserToCursorCentre(Lemming newCandidate)
    {
        var newCandidateDistanceSquaredFromCursorCentre = GetDistanceSquaredFromCursorCentre(newCandidate);

        if (newCandidateDistanceSquaredFromCursorCentre >= _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre)
            return false;

        _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre = newCandidateDistanceSquaredFromCursorCentre;
        return true;
    }

    private int GetDistanceSquaredFromCursorCentre(Lemming lemming)
    {
        var lemmingPosition = lemming.Orientation.Move(lemming.LevelPosition, lemming.FacingDirection.DeltaX, 4);

        var dx = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.X, lemmingPosition.X);
        var dy = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.Y, lemmingPosition.Y);

        return dx * dx + dy * dy;
    }
}