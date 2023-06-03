using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Engine;

public sealed class LevelCursor
{
#pragma warning disable CS8618
    public static LevelScreen LevelScreen { private get; set; }
#pragma warning restore CS8618

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelInputController _controller;
    private readonly Lemming[] _lemmings;

    private bool _hitTestAutoFail;
    private bool _doneAssignmentThisFrame;

    public bool HighlightLemming { get; private set; }
    public bool LemmingsUnderCursor => _numberOfLemmingsUnderCursor > 0;

    public LevelPosition CursorPosition { get; set; }

    //private Lemming? _lemmingUnderCursor;
    private int _numberOfLemmingsUnderCursor;

    public LevelCursor(
        IHorizontalBoundaryBehaviour horizontalBoundaryBehaviour,
        IVerticalBoundaryBehaviour verticalBoundaryBehaviour,
        ILevelControlPanel controlPanel,
        LevelInputController controller,
        Lemming[] lemmings)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _controlPanel = controlPanel;
        _controller = controller;
        _lemmings = lemmings;
    }

    public void HandleMouseInput()
    {
        //   CheckLemmingsUnderCursor();

        //   if (_controller.LeftMouseButtonStatus == MouseButtonStatusConsts.MouseButtonPressed)
        //  {
        HandleSkillAssignment();
        //  }
    }

    private bool HandleSkillAssignment(bool isReplayAssignment = false)
    {
        if (_controlPanel.SelectedSkillAssignButton == null)
            return false;

        var skill = _controlPanel.SelectedSkill;
        var oldHitTestAutoFail = _hitTestAutoFail;
        _hitTestAutoFail = false;

        // Just to be safe, though this should always return in fLemSelected
        var priorityLemming = GetPriorityLemming(skill, isReplayAssignment);
        // Get lemming to queue the skill assignment
        var queuedLemming = GetPriorityLemming(NoneSkill.Instance, false);

        _hitTestAutoFail = oldHitTestAutoFail;

        if (priorityLemming == null || !SkillIsAvailable(skill))
        {
            if (queuedLemming == null || skill.IsPermanentSkill)
                return false;

            LevelScreen.SetQueuedSkill(queuedLemming, skill);

            return false;
        }

        if (isReplayAssignment)// If the assignment is written in the replay, change lemming state
        {
            if (AssignSkill(priorityLemming, skill))
            {
                // CueSoundEffect(SFX_ASSIGN_SKILL, L.Position);

                return true;
            }

            return false;
        }

        if (SkillIsAvailable(skill))
        {
            RegainControl();
            RecordSkillAssignment(priorityLemming, skill);

            return true;
        }

        return false;
    }

    private Lemming? GetPriorityLemming(LemmingSkill lemmingSkill, bool isReplayAssignment)
    {
        Lemming? lemmingUnderCursor = null;
        _numberOfLemmingsUnderCursor = 0;
        int curValue = 10;

        for (var i = 0; i < _lemmings.Length; i++)
        {
            var lemming = _lemmings[i];

            if (HighlightLemming && !false/*(L = GetHighlitLemming)*/)
            {
                continue;
            }

            /* if (lemming.IsRemoved || lemming.IsTeleporting)
             {
                 continue;
             }*/
            /*
            // Check if we only look for highlighted Lems
            if (IsHighlight and not(L = GetHighlitLemming))
            or(IsReplay and not(L = GetTargetLemming)) then Continue;
            // Does Lemming exist
            if L.LemRemoved or L.LemTeleporting then Continue;
            // Is the Lemming unable to receive skills, because zombie, neutral, or was-ohnoer? (remove unless we haven't yet had any lem under the cursor)
            if L.CannotReceiveSkills and Assigned(PriorityLem) then Continue;
            */

            // Is Lemming inside cursor (only check if we are not using Hightlightning!)
            if (!LemmingIsUnderCursor(lemming))// and(not(IsHighlight or IsReplay))
            {
                continue;
            }
            // Directional select

            /*
            if (fSelectDx <> 0) and(fSelectDx <> L.LemDx) and(not(IsHighlight or IsReplay)) then Continue;
            // Select only walkers
            if IsSelectWalkerHotkey and(L.LemAction <> baWalking) and(not(IsHighlight or IsReplay)) then Continue;
            */

            // Increase number of lemmings in cursor (if not a zombie or neutral)
            //if(lemming.CanReceiveSkills)
            //{
            _numberOfLemmingsUnderCursor++;
            //}

            // Determine priority class of current lemming
            var curPriorityBox = 0;
            if (false) //IsSelectUnassignedHotkey or IsSelectWalkerHotkey then
            {
                curPriorityBox = 1;
            }
            else
            {
                curPriorityBox = 0;
                var lemIsInBox = false;
                do
                {
                    lemIsInBox = false;
                    curPriorityBox++;
                } while (curPriorityBox > Math.Min(curValue, 4) || lemIsInBox);
            }

            // Can this lemming actually receive the skill?
            if (!lemmingSkill.CanAssignToLemming(lemming))
            {
                curPriorityBox = 8;
            }

            // Deprioritize zombie even when just counting lemmings
            if (false)//(!lemming.CanReceiveSkills)
            {
                curPriorityBox = 9;
            }

            if (curPriorityBox < curValue || (curPriorityBox == curValue && IsCloserToCursorCentre(lemmingUnderCursor, lemming)))
            {
                // New top priority lemming found
                lemmingUnderCursor = lemming;
                curValue = curPriorityBox;
            }
        }

        //  Delete PriorityLem if too low-priority and we wish to assign a skill
        //  if (CurValue > 6) && !(NewSkillOrig = baNone) then PriorityLem := nil;

        return lemmingUnderCursor;
    }

    private bool LemmingIsUnderCursor(Lemming lemming)
    {
        if (lemming.Debug)
            return true;

        var lemmingPosition = lemming.Orientation.Move(lemming.LevelPosition, lemming.FacingDirection.DeltaX, 5);

        var dx = _horizontalBoundaryBehaviour.GetHorizontalDistanceSquared(CursorPosition.X, lemmingPosition.X);
        var dy = _verticalBoundaryBehaviour.GetVerticalDistanceSquared(CursorPosition.Y, lemmingPosition.Y);

        return dx < 8 && dy < 11;
    }

    private bool IsCloserToCursorCentre(Lemming? previousCandidate, Lemming newCandidate)
    {
        if (previousCandidate is null)
            return true;

        var previousLemmingPosition = previousCandidate.Orientation.Move(previousCandidate.LevelPosition, previousCandidate.FacingDirection.DeltaX, 5);
        var newLemmingPosition = newCandidate.Orientation.Move(newCandidate.LevelPosition, newCandidate.FacingDirection.DeltaX, 5);

        var dx1 = _horizontalBoundaryBehaviour.GetHorizontalDistanceSquared(CursorPosition.X, previousLemmingPosition.X);
        var dy1 = _horizontalBoundaryBehaviour.GetHorizontalDistanceSquared(CursorPosition.Y, previousLemmingPosition.Y);

        var dx2 = _verticalBoundaryBehaviour.GetVerticalDistanceSquared(CursorPosition.X, newLemmingPosition.X);
        var dy2 = _verticalBoundaryBehaviour.GetVerticalDistanceSquared(CursorPosition.Y, newLemmingPosition.Y);

        return dx2 + dy2 < dx1 + dy1;
    }

    private bool SkillIsAvailable(LemmingSkill lemmingSkill)
    {
        return lemmingSkill.CurrentNumberOfSkillsAvailable > 0;
    }

    private bool AssignSkill(Lemming lemming, LemmingSkill lemmingSkill)
    {
        if (!SkillIsAvailable(lemmingSkill) || _doneAssignmentThisFrame)
            return false;

        //UpdateSkillCount(lemmingSkill);

        LevelScreen.ClearQueuedSkill();

        var result = lemmingSkill.AssignToLemming(lemming);

        _doneAssignmentThisFrame = true;

        return result;
    }

    private void RegainControl()
    {
    }

    private void RecordSkillAssignment(Lemming queuedLemming, LemmingSkill skill)
    {
    }
}