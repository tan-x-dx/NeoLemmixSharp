using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelCursor
{
    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;
    private readonly LevelInputController _controller;

    private FacingDirection? _facingDirection;
    private bool _selectOnlyWalkers;
    private bool _selectOnlyUnassigned;

    private int _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre;

    public int NumberOfLemmingsUnderCursor { get; private set; }
    public LevelPosition CursorPosition { private get; set; }

    public Lemming? CurrentlyHighlightedLemming { get; private set; }

    public Color Color1 { get; private set; }
    public Color Color2 { get; private set; }
    public Color Color3 { get; private set; }

    public LevelCursor(
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour,
        LevelInputController controller)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _controller = controller;

        SetSelectedTeam(null);
    }

    public void Tick()
    {
        NumberOfLemmingsUnderCursor = 0;
        CurrentlyHighlightedLemming = null;
        _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre = int.MaxValue;

        _selectOnlyWalkers = _controller.SelectOnlyWalkers.IsActionDown;
        _selectOnlyUnassigned = _controller.SelectOnlyUnassignedLemmings.IsActionDown;

        if (_controller.SelectLeftFacingLemmings.IsActionDown)
        {
            _facingDirection = FacingDirection.LeftInstance;
        }
        else if (_controller.SelectRightFacingLemmings.IsActionDown)
        {
            _facingDirection = FacingDirection.RightInstance;
        }
        else
        {
            _facingDirection = null;
        }
    }

    public SimpleSetEnumerable<Lemming> LemmingsNearCursorPosition()
    {
        var c = CursorPosition;

        var topLeftCursorPixel = new LevelPosition(c.X - 7, c.Y - 7);
        var bottomRightCursorPixel = new LevelPosition(c.X + 6, c.Y + 6);
        var levelRegion = new LevelPositionPair(topLeftCursorPixel, bottomRightCursorPixel);

        return LevelScreen.LemmingManager.GetAllLemmingsNearRegion(levelRegion);
    }

    public void SetSelectedTeam(Team? team)
    {
        if (team is null)
        {
            Color1 = LevelConstants.CursorColor1;
            Color2 = LevelConstants.CursorColor2;
            Color3 = LevelConstants.CursorColor3;

            return;
        }

        Color1 = team.HairColor;
        Color2 = team.BodyColor;
        Color3 = team.SkinColor;
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
        var lemmingPosition = lemming.CenterPosition;

        var dx = _horizontalBoundaryBehaviour.GetDelta(CursorPosition.X, lemmingPosition.X);
        var dy = _verticalBoundaryBehaviour.GetDelta(CursorPosition.Y, lemmingPosition.Y);

        return Math.Abs(dx) < 5 && Math.Abs(dy) < 5;
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

        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(LevelScreen.LevelControlPanel.SelectedSkillButtonId);
        if (skillTrackingData is null || skillTrackingData.SkillCount == 0)
            return false;

        return skillTrackingData.Skill.CanAssignToLemming(lemming);
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

    private static bool NewCandidateHasMoreRelevantTeam(Lemming previousCandidate, Lemming newCandidate)
    {
        var skillTrackingDataId = LevelScreen.LevelControlPanel.SelectedSkillAssignButton?.SkillTrackingDataId ?? -1;

        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(skillTrackingDataId);
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
        var lemmingPosition = lemming.CenterPosition;

        var dx = _horizontalBoundaryBehaviour.GetDelta(CursorPosition.X, lemmingPosition.X);
        var dy = _verticalBoundaryBehaviour.GetDelta(CursorPosition.Y, lemmingPosition.Y);

        return dx * dx + dy * dy;
    }
}