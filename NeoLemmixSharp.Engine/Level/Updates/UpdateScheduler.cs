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
        HandleKeyboardInput();
        TickLevel();
        HandleCursor();
        HandleSkillAssignment();
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

    private void HandleKeyboardInput()
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
            GoToTick(_elapsedTicks - 50);
        }

        if (inputController.Reset.IsPressed)
        {
            GoToTick(0);
        }
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
        LevelScreen.LevelTimer.SetElapsedTicks(_elapsedTicks);
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
            var lemmingsNearCursor = LevelScreen.LevelCursor.LemmingsNearCursorPosition();
            foreach (var lemming in lemmingsNearCursor)
            {
                LevelScreen.LevelCursor.CheckLemming(lemming);
            }

            LevelScreen.LevelControlPanel.TextualData.SetCursorData(
                LevelScreen.LevelCursor.CurrentlyHighlightedLemming,
                LevelScreen.LevelCursor.NumberOfLemmingsUnderCursor);
        }
        else
        {
            LevelScreen.LevelControlPanel.TextualData.ClearCursorData();
        }
    }

    private void HandleSkillAssignment()
    {
        if (LevelScreen.RewindManager.DoneSkillAssignmentThisTick)
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

        if (skillTrackingData.CanAssignToLemming(lemming))
        {
            DoSkillAssignment(skillTrackingData, lemming);
        }
        else
        {
            SetQueuedSkill(lemming, skillTrackingData.Skill);
        }
    }

    public void DoSkillAssignment(
        SkillTrackingData skillTrackingData,
        Lemming lemming)
    {
        skillTrackingData.Skill.AssignToLemming(lemming);
        skillTrackingData.ChangeSkillCount(-1);
        LevelScreen.LevelControlPanel.UpdateSkillCount(skillTrackingData);

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

    public void SetQueuedSkill(Lemming lemming, LemmingSkill lemmingSkill)
    {
        _queuedSkill = lemmingSkill;
        _queuedSkillLemming = lemming;
        _queuedSkillFrame = 0;
    }

    public void CheckForQueuedAction()
    {
        // First check whether there was already a skill assignment this frame
        //    if Assigned(fReplayManager.Assignment[fCurrentIteration, 0]) then Exit;

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

        //   if (QueuedSkill.CanAssignToLemming(QueuedSkillLemming) && _skillSetManager.NumberOfSkillsAvailable(QueuedSkill) > 0)
        {
            // Record skill assignment, so that we apply it in CheckForReplayAction
            // RecordSkillAssignment(L, NewSkill)
        }
        //  else
        {
            _queuedSkillFrame++;

            // Delete queued action after 16 frames
            if (_queuedSkillFrame > 15)
            {
                ClearQueuedSkill();
            }
        }
    }

    private void GoToTick(int requiredTick)
    {
        SetUpdateState(UpdateState.Paused);

        requiredTick = Math.Max(requiredTick, 0);

        _queuedSkill = NoneSkill.Instance;
        _queuedSkillLemming = null;
        _queuedSkillFrame = 0;

        var tickDelta = requiredTick - _elapsedTicks;
        int remainingTicks;

        if (tickDelta < 0)
        {
            var actualElapsedTicks = LevelScreen.RewindManager.RewindBackTo(requiredTick);
            remainingTicks = requiredTick - actualElapsedTicks;

            _elapsedTicks = actualElapsedTicks;
            _elapsedTicksModuloFastForwardSpeed = _elapsedTicks % EngineConstants.FastForwardSpeedMultiplier;
        }
        else
        {
            remainingTicks = tickDelta;
        }

        while (remainingTicks-- > 0)
        {
            PerformOneTick();
        }

        Debug.Assert(_elapsedTicks == requiredTick);
    }
}