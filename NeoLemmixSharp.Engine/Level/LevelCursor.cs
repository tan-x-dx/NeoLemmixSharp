using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using System.Diagnostics.Contracts;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelCursor
{
    private FacingDirection? _facingDirection;
    private bool _selectOnlyWalkers;
    private bool _selectOnlyUnassigned;

    private int _currentlyHighlightedLemmingDistanceSquaredFromCursorCentre;

    public uint NumberOfLemmingsUnderCursor { get; private set; }
    public Point CursorPosition { private get; set; }

    public Lemming? CurrentlyHighlightedLemming { get; private set; }

    public Color Color1 { get; private set; }
    public Color Color2 { get; private set; }
    public Color Color3 { get; private set; }

    public LevelCursor()
    {
        SetSelectedTribe(null);
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

    public void CheckLemmingsNearCursor()
    {
        GetLemmingsNearCursorPosition(out var lemmingsNearCursor);
        foreach (var lemming in lemmingsNearCursor)
        {
            CheckLemming(lemming);
        }

        if (CurrentlyHighlightedLemming is not null)
        {
            LevelScreen.LevelControlPanel.TextualData.SetCursorData(
                CurrentlyHighlightedLemming,
                NumberOfLemmingsUnderCursor);
        }
    }

    private void GetLemmingsNearCursorPosition(out LemmingEnumerable result)
    {
        var c = CursorPosition;

        // Cursor position is the bottom right pixel of the middle square in the cursor sprite.
        // Hence, for the top left position, add one extra pixel to fix offset.
        var topLeftCursorPixel = new Point(c.X - (EngineConstants.CursorRadius + 1), c.Y - (EngineConstants.CursorRadius + 1));
        var bottomRightCursorPixel = new Point(c.X + EngineConstants.CursorRadius, c.Y + EngineConstants.CursorRadius);
        var levelRegion = new RectangularRegion(topLeftCursorPixel, bottomRightCursorPixel);

        LevelScreen.LemmingManager.GetAllLemmingsNearRegion(levelRegion, out result);
    }

    public void SetSelectedTribe(Tribe? tribe)
    {
        if (tribe is null)
        {
            Color1 = EngineConstants.CursorColor1;
            Color2 = EngineConstants.CursorColor2;
            Color3 = EngineConstants.CursorColor3;
        }
        else
        {
            Color1 = tribe.ColorData.HairColor;
            Color2 = tribe.ColorData.BodyColor;
            Color3 = tribe.ColorData.SkinColor;
        }
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

    [Pure]
    private bool LemmingIsUnderCursor(Lemming lemming)
    {
        var lemmingPosition = lemming.CenterPosition;

        var dx = LevelScreen.HorizontalBoundaryBehaviour.GetDelta(CursorPosition.X, lemmingPosition.X);
        var dy = LevelScreen.VerticalBoundaryBehaviour.GetDelta(CursorPosition.Y, lemmingPosition.Y);

        return Math.Abs(dx) < EngineConstants.CursorRadius && Math.Abs(dy) < EngineConstants.CursorRadius;
    }

    [Pure]
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
               NewCandidateHasMoreRelevantTribe(previousCandidate, newCandidate) ||
               NewCandidateHasHigherActionPriority(previousCandidate, newCandidate) ||
               NewCandidateIsCloserToCursorCentre(newCandidate);
    }

    [Pure]
    private static bool NewCandidateHasMoreRelevantState(Lemming previousCandidate, Lemming newCandidate)
    {
        return (previousCandidate.State.IsNeutral && !newCandidate.State.IsNeutral) ||
               (previousCandidate.State.IsZombie && !newCandidate.State.IsZombie);
    }

    [Pure]
    private static bool NewCandidateHasMoreRelevantTribe(Lemming previousCandidate, Lemming newCandidate)
    {
        var skillTrackingDataId = LevelScreen.LevelControlPanel.SelectedSkillAssignButton?.SkillTrackingDataId ?? -1;

        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(skillTrackingDataId);
        if (skillTrackingData is null)
            return false;

        var previousCandidateMatchesTribe = previousCandidate.State.TribeAffiliation.Equals(skillTrackingData.Tribe);
        var newCandidateMatchesTribe = newCandidate.State.TribeAffiliation.Equals(skillTrackingData.Tribe);

        return newCandidateMatchesTribe && !previousCandidateMatchesTribe;
    }

    [Pure]
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

    [Pure]
    private int GetDistanceSquaredFromCursorCentre(Lemming lemming)
    {
        var lemmingPosition = lemming.CenterPosition;

        var dx = LevelScreen.HorizontalBoundaryBehaviour.GetDelta(CursorPosition.X, lemmingPosition.X);
        var dy = LevelScreen.VerticalBoundaryBehaviour.GetDelta(CursorPosition.Y, lemmingPosition.Y);

        return dx * dx + dy * dy;
    }
}

public class SomeData
{
    public readonly int Id;

    public SomeData(int id) => Id = id;

    public bool EqualsInstance1(SomeData? other) => Id == (other?.Id ?? -1);

    public bool EqualsInstance2(SomeData? other)
    {
        var valA = Id;
        var valB = -1;
        if (other is not null) valB = other.Id;
        return valA == valB;
    }

    public bool EqualsInstance3(SomeData? other)
    {
        var otherValue = -1;
        if (other is not null) otherValue = other.Id;
        return Id == otherValue;
    }

    public static bool Equals1(SomeData? a, SomeData? b) => a?.Id == b?.Id;

    public static bool Equals2(SomeData? a, SomeData? b)
    {
        var valA = -1;
        if (a is not null) valA = a.Id;
        var valB = -1;
        if (b is not null) valB = b.Id;
        return valA == valB;
    }

    public static bool Equals3(SomeData? a, SomeData? b)
    {
        var valA = a?.Id ?? -1;
        var valB = b?.Id ?? -1;
        return valA == valB;
    }

    public static bool EqualsNotNull1(SomeData a, SomeData b) => a.Id == b.Id;

    public static bool EqualsNotNull2(SomeData a, SomeData b)
    {
        var valA = a.Id;
        var valB = b.Id;
        return valA == valB;
    }
}
