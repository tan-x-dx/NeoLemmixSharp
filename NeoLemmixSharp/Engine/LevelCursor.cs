using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.Engine.LevelUpdates;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Engine;

public sealed class LevelCursor
{
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelUpdater _levelUpdater;
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
        ILevelControlPanel controlPanel,
        LevelUpdater levelUpdater,
        LevelInputController controller,
        Lemming[] lemmings)
    {
        _controlPanel = controlPanel;
        _controller = controller;
        _lemmings = lemmings;
        _levelUpdater = levelUpdater;
    }

    public void HandleMouseInput()
    {
        //   CheckLemmingsUnderCursor();

        if (_controller.LeftMouseButtonStatus == MouseButtonStatusConsts.MouseButtonPressed)
        {
            HandleSkillAssignment();
        }
    }

    private bool HandleSkillAssignment(bool isReplayAssignment = false)
    {
        if (_controlPanel.SelectedSkillAssignButton != null)
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

            _levelUpdater.SetQueuedSkill(queuedLemming, skill);

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
        var p0 = lemming.Orientation.Move(lemming.LevelPosition, -8, 10);
        var p1 = lemming.Orientation.Move(lemming.LevelPosition, 5, -3);

        return lemming.Orientation.FirstIsAboveSecond(p0, CursorPosition) &&
               lemming.Orientation.FirstIsToLeftOfSecond(p0, CursorPosition) &&
               lemming.Orientation.FirstIsBelowSecond(p1, CursorPosition) &&
               lemming.Orientation.FirstIsToRightOfSecond(p1, CursorPosition);
    }

    private bool IsCloserToCursorCentre(Lemming? previousCandidate, Lemming newCandidate)
    {
        if (previousCandidate is null)
            return true;

        var l1 = previousCandidate.Orientation.MoveUp(previousCandidate.LevelPosition, 6);
        var l2 = newCandidate.Orientation.MoveUp(newCandidate.LevelPosition, 6);

        var x1 = l1.X - CursorPosition.X;
        var y1 = l1.Y - CursorPosition.Y;

        var x2 = l2.X - CursorPosition.X;
        var y2 = l2.Y - CursorPosition.Y;

        var d1 = x1 * x1 + y1 * y1;
        var d2 = x2 * x2 + y2 * y2;

        return d2 < d1;
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

        _levelUpdater.ClearQueuedSkill();

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