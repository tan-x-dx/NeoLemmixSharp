using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.Updates;

public sealed class UpdateScheduler : IInitialisable
{
    private LemmingSkill _queuedSkill = NoneSkill.Instance;
    private int _queuedSkillTeamId;
    private Lemming? _queuedSkillLemming;

    private UpdateState _updateState;
    private int _queuedSkillFrame;

    private int _elapsedTicks;
    private int _elapsedTicksModuloFastForwardSpeed;

    public int ElapsedTicks => _elapsedTicks;

    public void Initialise()
    {
        SetUpdateState(UpdateState.Paused);
    }

    /*

procedure TLemmingGame.UpdateLemmings;
{-------------------------------------------------------------------------------
  The main method: handling a single frame of the game.
-------------------------------------------------------------------------------}
begin
  fDoneAssignmentThisFrame := false;

  if fGameFinished then
    Exit;
  fSoundList.Clear(); // Clear list of played sound effects
  CheckForGameFinished;

  CheckAdjustSpawnInterval;

  CheckForQueuedAction; // needs to be done before CheckForReplayAction, because it writes an assignment in the replay
  CheckForReplayAction;

  // erase existing ShadowBridge
  if fExistShadow then
  begin
    fRenderer.ClearShadows;
    fExistShadow := false;
  end;

  // just as a warning: do *not* mess around with the order here
  IncrementIteration;
  CheckReleaseLemming;
  CheckLemmings;
  CheckUpdateNuking;
  UpdateGadgets;

  if (fReplayManager.ExpectedCompletionIteration = fCurrentIteration) and (not Checkpass) then
    fReplayManager.ExpectedCompletionIteration := 0;

  // Get highest priority lemming under cursor
  GetPriorityLemming(fLemSelected, SkillPanelButtonToAction[fSelectedSkill], CursorPoint);

  DrawAnimatedGadgets;

  // Check lemmings under cursor
  HitTest;
  fSoundList.Clear(); // Clear list of played sound effects - just to be safe
end;

    */

    public void Tick()
    {
        LevelScreen.LevelInputController.Tick();
        LevelScreen.LevelCursor.Tick();

        HandleMouseInput();
        var shouldContinue = HandleKeyboardInput();
        if (!shouldContinue)
            return;
        HandleCursor();
        CheckForQueuedAction();
        LevelScreen.RewindManager.CheckForReplayAction(_elapsedTicks);
        HandleSkillAssignment();
        TickLevel();
    }

    private static void HandleMouseInput()
    {
        LevelScreen.LevelViewport.HandleMouseInput(LevelScreen.LevelInputController);

        if (LevelScreen.LevelViewport.MouseIsInLevelViewPort)
        {
            LevelScreen.LevelCursor.CursorPosition = new LevelPosition(
                LevelScreen.HorizontalBoundaryBehaviour.MouseViewPortCoordinate,
                LevelScreen.VerticalBoundaryBehaviour.MouseViewPortCoordinate);
        }
        else
        {
            LevelScreen.LevelCursor.CursorPosition = new LevelPosition(-4000, -4000);
            LevelScreen.LevelControlPanel.HandleMouseInput();
        }
    }

    private bool HandleKeyboardInput()
    {
        var inputController = LevelScreen.LevelInputController;

        if (inputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleFullscreen();
        }

        if (inputController.Pause.IsPressed)
        {
            SetUpdateState(_updateState == UpdateState.Paused ? UpdateState.Normal : UpdateState.Paused);
        }
        else if (inputController.ToggleFastForwards.IsPressed)
        {
            SetUpdateState(_updateState == UpdateState.FastForward ? UpdateState.Normal : UpdateState.FastForward);
        }

        if (inputController.Quit.IsPressed)
        {
            IGameWindow.Instance.Escape();
        }

        if (inputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleBorderless();
        }

        if (inputController.Rewind50Frames.IsPressed)
        {
            GoToTick(_elapsedTicks - 50, true);
            return false;
        }

        if (inputController.Reset.IsPressed)
        {
            GoToTick(0, false);
            return false;
        }

        return true;
    }

    private void SetUpdateState(UpdateState updateState)
    {
        _updateState = updateState;
        UpdateControlPanelButtonStatus(ButtonType.Pause, updateState == UpdateState.Paused);
        UpdateControlPanelButtonStatus(ButtonType.FastForward, updateState == UpdateState.FastForward);
    }

    private static void UpdateControlPanelButtonStatus(ButtonType buttonType, bool isSelected)
    {
        var button = LevelScreen.LevelControlPanel.GetControlPanelButtonOfType(buttonType);
        if (button is null)
            return;
        button.IsSelected = isSelected;
    }

    private void TickLevel()
    {
        var numberOfTicksToPerform = (int)_updateState;

        if (LevelScreen.LevelInputController.LeftMouseButtonAction.IsPressed)
        {
            numberOfTicksToPerform++;
        }

        while (numberOfTicksToPerform-- > 0)
        {
            PerformOneTick();
        }
    }

    private void PerformOneTick()
    {
        var isMajorTick = _elapsedTicksModuloFastForwardSpeed == 0;
        LevelScreen.LemmingManager.Tick(isMajorTick);
        LevelScreen.GadgetManager.Tick(isMajorTick);
        LevelScreen.LevelTimer.SetElapsedTicks(_elapsedTicks, true);
        LevelScreen.RewindManager.Tick(_elapsedTicks);
        LevelScreen.TerrainPainter.RepaintTerrain();

        _elapsedTicks++;
        var elapsedTicksModuloFastForwardSpeed = _elapsedTicksModuloFastForwardSpeed + 1;
        elapsedTicksModuloFastForwardSpeed %= EngineConstants.FastForwardSpeedMultiplier;
        _elapsedTicksModuloFastForwardSpeed = elapsedTicksModuloFastForwardSpeed;
    }

    private static void HandleCursor()
    {
        var mouseIsInLevelViewPort = LevelScreen.LevelViewport.MouseIsInLevelViewPort;
        if (mouseIsInLevelViewPort)
        {
            LevelScreen.LevelCursor.CheckLemmingsNearCursor();
        }
        else
        {
            LevelScreen.LevelControlPanel.TextualData.ClearCursorData();
        }
    }

    private void HandleSkillAssignment()
    {
        if (LevelScreen.RewindManager.DoneSkillAssignmentForTick(_elapsedTicks))
            return;

        if (!LevelScreen.LevelInputController.LeftMouseButtonAction.IsPressed)
            return;

        var lemming = LevelScreen.LevelCursor.CurrentlyHighlightedLemming;
        if (lemming is null)
            return;

        var selectedSkillId = LevelScreen.LevelControlPanel.SelectedSkillButtonId;
        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(selectedSkillId);
        if (skillTrackingData is null || skillTrackingData.SkillCount == 0)
            return;

        if (LemmingCanBeTicked(lemming) &&
            skillTrackingData.CanAssignToLemming(lemming))
        {
            DoSkillAssignment(skillTrackingData, lemming, false);
        }
        else
        {
            SetQueuedSkill(lemming, skillTrackingData.Skill);
        }
    }

    public void DoSkillAssignment(
        SkillTrackingData skillTrackingData,
        Lemming lemming,
        bool isReplay)
    {
        skillTrackingData.Skill.AssignToLemming(lemming);
        skillTrackingData.ChangeSkillCount(-1);
        LevelScreen.LevelControlPanel.UpdateSkillCount(skillTrackingData);

        if (isReplay)
            return;

        LevelScreen.RewindManager.RecordSkillAssignment(
            _elapsedTicks,
            lemming,
            skillTrackingData);
    }

    private void ClearQueuedSkill()
    {
        _queuedSkill = NoneSkill.Instance;
        _queuedSkillLemming = null;
        _queuedSkillFrame = 0;
    }

    private void SetQueuedSkill(Lemming lemming, LemmingSkill lemmingSkill)
    {
        _queuedSkill = lemmingSkill;
        _queuedSkillLemming = lemming;
        _queuedSkillTeamId = lemming.State.TeamAffiliation.Id;
        _queuedSkillFrame = EngineConstants.FramesPerSecond - 1;
    }

    private void CheckForQueuedAction()
    {
        // First check whether there was already a skill assignment this frame
        if (LevelScreen.RewindManager.DoneSkillAssignmentForTick(_elapsedTicks))
            return;

        if (_queuedSkill == NoneSkill.Instance || _queuedSkillLemming is null)
        {
            ClearQueuedSkill();
            return;
        }

        if (!_queuedSkillLemming.State.IsActive ||
            !_queuedSkillLemming.State.CanHaveSkillsAssigned
            || false) // || lemming is teleporting
        {
            // delete queued action first
            ClearQueuedSkill();
            return;
        }

        var skillTrackingData = LevelScreen.SkillSetManager.GetSkillTrackingData(
            _queuedSkill.Id,
            _queuedSkillTeamId);

        if (skillTrackingData is not null &&
            LemmingCanBeTicked(_queuedSkillLemming) &&
            skillTrackingData.CanAssignToLemming(_queuedSkillLemming))
        {
            DoSkillAssignment(skillTrackingData, _queuedSkillLemming, false);
        }
        else
        {
            // Delete queued action after 16 frames
            if (_queuedSkillFrame-- > 0)
                return;

            ClearQueuedSkill();
        }
    }

    private bool LemmingCanBeTicked(Lemming queuedSkillLemming)
    {
        return queuedSkillLemming.IsFastForward ||
               _elapsedTicksModuloFastForwardSpeed == 0;
    }

    private void GoToTick(int requiredTick, bool pause)
    {
        if (pause)
        {
            SetUpdateState(UpdateState.Paused);
        }

        requiredTick = Math.Max(requiredTick, 0);

        _queuedSkill = NoneSkill.Instance;
        _queuedSkillLemming = null;
        _queuedSkillFrame = 0;

        var tickDelta = requiredTick - _elapsedTicks;

        if (tickDelta < 0)
        {
            var actualElapsedTicks = LevelScreen.RewindManager.RewindBackTo(requiredTick);
            tickDelta = requiredTick - actualElapsedTicks;

            _elapsedTicks = actualElapsedTicks;
            _elapsedTicksModuloFastForwardSpeed = _elapsedTicks % EngineConstants.FastForwardSpeedMultiplier;
        }

        while (tickDelta-- > 0)
        {
            PerformOneTick();
        }

        Debug.Assert(_elapsedTicks == requiredTick);
    }
}