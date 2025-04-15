using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Teams;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelCursor
{
    private FacingDirection? _facingDirection;
    private bool _selectOnlyWalkers;
    private bool _selectOnlyUnassigned;

    private int _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre;

    public int NumberOfLemmingsUnderCursor { get; private set; }
    public Point CursorPosition { private get; set; }

    public Lemming? CurrentlyHighlightedLemming { get; private set; }

    public Microsoft.Xna.Framework.Color Color1 { get; private set; }
    public Microsoft.Xna.Framework.Color Color2 { get; private set; }
    public Microsoft.Xna.Framework.Color Color3 { get; private set; }

    public LevelCursor()
    {
        SetSelectedTeam(null);
    }

    public void Tick()
    {
        NumberOfLemmingsUnderCursor = 0;
        CurrentlyHighlightedLemming = null;
        _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre = int.MaxValue;

        _selectOnlyWalkers = LevelScreen.LevelInputController.SelectOnlyWalkers.IsActionDown;
        _selectOnlyUnassigned = LevelScreen.LevelInputController.SelectOnlyUnassignedLemmings.IsActionDown;

        if (LevelScreen.LevelInputController.SelectLeftFacingLemmings.IsActionDown)
        {
            _facingDirection = FacingDirection.Left;
        }
        else if (LevelScreen.LevelInputController.SelectRightFacingLemmings.IsActionDown)
        {
            _facingDirection = FacingDirection.Right;
        }
        else
        {
            _facingDirection = null;
        }
    }

    [SkipLocalsInit]
    public void CheckLemmingsNearCursor()
    {
        Span<uint> scratchSpaceSpan = stackalloc uint[LevelScreen.LemmingManager.ScratchSpaceSize];
        GetLemmingsNearCursorPosition(scratchSpaceSpan, out var lemmingsNearCursor);
        foreach (var lemming in lemmingsNearCursor)
        {
            CheckLemming(lemming);
        }

        LevelScreen.LevelControlPanel.TextualData.SetCursorData(
            CurrentlyHighlightedLemming,
            NumberOfLemmingsUnderCursor);
    }

    private void GetLemmingsNearCursorPosition(Span<uint> scratchSpaceSpan, out LemmingEnumerable result)
    {
        var c = CursorPosition;

        // Cursor position is the bottom right pixel of the middle square in the cursor sprite.
        // Hence, for the top left position, add one extra pixel to fix offset.
        var topLeftCursorPixel = new Point(c.X - (EngineConstants.CursorRadius + 1), c.Y - (EngineConstants.CursorRadius + 1));
        var bottomRightCursorPixel = new Point(c.X + EngineConstants.CursorRadius, c.Y + EngineConstants.CursorRadius);
        var levelRegion = new RectangularRegion(topLeftCursorPixel, bottomRightCursorPixel);

        LevelScreen.LemmingManager.GetAllLemmingsNearRegion(scratchSpaceSpan, levelRegion, out result);
    }

    public void SetSelectedTeam(Team? team)
    {
        if (team is null)
        {
            Color1 = EngineConstants.CursorColor1;
            Color2 = EngineConstants.CursorColor2;
            Color3 = EngineConstants.CursorColor3;

            return;
        }

        Color1 = team.HairColor;
        Color2 = team.BodyColor;
        Color3 = team.SkinColor;
    }

    private void CheckLemming(Lemming lemming)
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

        var dx = LevelScreen.HorizontalBoundaryBehaviour.GetDelta(CursorPosition.X, lemmingPosition.X);
        var dy = LevelScreen.VerticalBoundaryBehaviour.GetDelta(CursorPosition.Y, lemmingPosition.Y);

        return Math.Abs(dx) < EngineConstants.CursorRadius && Math.Abs(dy) < EngineConstants.CursorRadius;
    }

    private bool LemmingIsAbleToBeSelected(Lemming lemming)
    {
        // Directional select
        if (_facingDirection.HasValue &&
            lemming.FacingDirection != _facingDirection &&
            !(false))//and(not(IsHighlight or IsReplay))
            return false;

        // Select only walkers
        if (_selectOnlyWalkers && lemming.CurrentAction != WalkerAction.Instance &&
            !(false)) //and(not(IsHighlight or IsReplay))
            return false;

        // Select only unassigned
        return !_selectOnlyUnassigned || !lemming.State.HasPermanentSkill;
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

        var dx = LevelScreen.HorizontalBoundaryBehaviour.GetDelta(CursorPosition.X, lemmingPosition.X);
        var dy = LevelScreen.VerticalBoundaryBehaviour.GetDelta(CursorPosition.Y, lemmingPosition.Y);

        return dx * dx + dy * dy;
    }
}