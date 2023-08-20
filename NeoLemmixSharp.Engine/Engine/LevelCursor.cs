﻿using NeoLemmixSharp.Common.BoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Common.BoundaryBehaviours.Vertical;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.FacingDirections;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LevelCursor
{
#pragma warning disable CS8618
    public static LevelScreen LevelScreen { private get; set; }
#pragma warning restore CS8618

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelInputController _controller;
    private readonly SkillSetManager _skillSetManager;

    public int NumberOfLemmingsUnderCursor { get; private set; }
    public LevelPosition CursorPosition { private get; set; }

    private Lemming? _currentlyHighlightedLemming;
    private FacingDirection? _facingDirection;
    private bool _selectOnlyWalkers;
    private bool _selectOnlyUnassigned;

    public LevelCursor(
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour,
        ILevelControlPanel controlPanel,
        LevelInputController controller,
        SkillSetManager skillSetManager)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _controlPanel = controlPanel;
        _controller = controller;
        _skillSetManager = skillSetManager;
    }

    public void OnNewFrame()
    {
        NumberOfLemmingsUnderCursor = 0;
        _currentlyHighlightedLemming = null;

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

    public void CheckLemming(Lemming lemming)
    {
        if (!LemmingIsUnderCursor(lemming) || !LemmingIsAbleToBeSelected(lemming))
            return;

        NumberOfLemmingsUnderCursor++;
        if (NewCandidateIsHigherPriority(_currentlyHighlightedLemming, lemming))
        {
            _currentlyHighlightedLemming = lemming;
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
            return true;

        if (newCandidate.State.IsNeutral)
            return false;
        if (newCandidate.State.IsZombie)
            return false;

        if (NewCandidateHasMoreRelevantTeam(previousCandidate, newCandidate))
            return true;

        if (newCandidate.CurrentAction.CursorSelectionPriorityValue >
            previousCandidate.CurrentAction.CursorSelectionPriorityValue)
            return true;

        return IsCloserToCursorCentre(previousCandidate, newCandidate);
    }

    private bool NewCandidateHasMoreRelevantTeam(Lemming previousCandidate, Lemming newCandidate)
    {
        var teamForSelectedSkill = _skillSetManager.TeamForSelectedSkill(_controlPanel.SelectedSkillButtonId);
        if (teamForSelectedSkill is null)
            return false;

        var previousCandidateMatchesTeam = previousCandidate.State.TeamAffiliation == teamForSelectedSkill;
        var newCandidateMatchesTeam = newCandidate.State.TeamAffiliation == teamForSelectedSkill;

        return newCandidateMatchesTeam && !previousCandidateMatchesTeam;
    }

    private bool IsCloserToCursorCentre(Lemming previousCandidate, Lemming newCandidate)
    {
        var previousLemmingPosition = previousCandidate.Orientation.Move(previousCandidate.LevelPosition, previousCandidate.FacingDirection.DeltaX, 4);
        var newLemmingPosition = newCandidate.Orientation.Move(newCandidate.LevelPosition, newCandidate.FacingDirection.DeltaX, 4);

        var dx1 = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.X, previousLemmingPosition.X);
        var dy1 = _horizontalBoundaryBehaviour.GetAbsoluteHorizontalDistance(CursorPosition.Y, previousLemmingPosition.Y);

        var dx2 = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.X, newLemmingPosition.X);
        var dy2 = _verticalBoundaryBehaviour.GetAbsoluteVerticalDistance(CursorPosition.Y, newLemmingPosition.Y);

        return dx2 * dx2 + dy2 * dy2 < dx1 * dx1 + dy1 * dy1;
    }
}