using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;

namespace NeoLemmixSharp.Engine.Level.Updates;

public sealed class UpdateScheduler
{
    private readonly LevelControlPanel _levelControlPanel;
    private readonly LevelInputController _inputController;
    private readonly Viewport _viewport;
    private readonly LevelCursor _levelCursor;
    private readonly LevelTimer _levelTimer;
    private readonly LemmingManager _lemmingManager;
    private readonly GadgetManager _gadgetManager;
    private readonly SkillSetManager _skillSetManager;

    private LemmingSkill _queuedSkill = NoneSkill.Instance;
    private Lemming? _queuedSkillLemming;

    private UpdateState _updateState;
    private int _queuedSkillFrame;

    private int _elapsedTicks;
    private int _elapsedTicksModuloFastForwardSpeed;
    private int _elapsedTicksModuloFramesPerSecond;

    public bool DoneAssignmentThisFrame { get; set; }

    public UpdateScheduler(
        LevelControlPanel levelControlPanel,
        Viewport viewport,
        LevelCursor levelCursor,
        LevelInputController inputController,
        LevelTimer levelTimer,
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        SkillSetManager skillSetManager)
    {
        _levelControlPanel = levelControlPanel;
        _levelCursor = levelCursor;
        _viewport = viewport;
        _inputController = inputController;
        _levelTimer = levelTimer;
        _lemmingManager = lemmingManager;
        _gadgetManager = gadgetManager;
        _skillSetManager = skillSetManager;
    }

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
        _inputController.Tick();
        _levelCursor.Tick();

        HandleMouseInput();
        EvaluateGameState();
        HandleCursor();
        TickLevel();
        HandleSkillAssignment();
    }

    private void TickLevel()
    {
        if (_updateState == UpdateState.Paused)
            return;

        var isMajorTick = _elapsedTicksModuloFastForwardSpeed == 0;
        _lemmingManager.Tick(_updateState, isMajorTick);
        _gadgetManager.Tick(_updateState, isMajorTick);

        _elapsedTicks++;
        var elapsedTicksModuloFastForwardSpeed = _elapsedTicksModuloFastForwardSpeed + 1;
        if (elapsedTicksModuloFastForwardSpeed == EngineConstants.FastForwardSpeedMultiplier)
        {
            elapsedTicksModuloFastForwardSpeed = 0;
        }
        _elapsedTicksModuloFastForwardSpeed = elapsedTicksModuloFastForwardSpeed;

        var elapsedTicksModuloFramesPerSecond = _elapsedTicksModuloFramesPerSecond + 1;
        if (elapsedTicksModuloFramesPerSecond == EngineConstants.FramesPerSecond)
        {
            _levelTimer.Tick();
            elapsedTicksModuloFramesPerSecond = 0;
        }
        _elapsedTicksModuloFramesPerSecond = elapsedTicksModuloFramesPerSecond;
    }

    private void HandleMouseInput()
    {
        _viewport.HandleMouseInput(_inputController);

        if (_viewport.MouseIsInLevelViewPort)
        {
            _levelCursor.CursorPosition = new LevelPosition(_viewport.ViewportMouseX, _viewport.ViewportMouseY);
        }
        else
        {
            _levelCursor.CursorPosition = new LevelPosition(-4000, -4000);
            _levelControlPanel.HandleMouseInput();
        }
    }

    private void EvaluateGameState()
    {
        if (_inputController.Pause.IsPressed)
        {
            SetUpdateState(_updateState == UpdateState.Paused ? UpdateState.Normal : UpdateState.Paused);
        }
        else if (_inputController.ToggleFastForwards.IsPressed)
        {
            SetUpdateState(_updateState == UpdateState.FastForward ? UpdateState.Normal : UpdateState.FastForward);
        }

        if (_inputController.Quit.IsPressed)
        {
            IGameWindow.Instance.Escape();
        }

        if (_inputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleBorderless();
        }
    }

    private void SetUpdateState(UpdateState updateState)
    {
        _updateState = updateState;
        var isPaused = updateState == UpdateState.Paused;
        UpdateControlPanelButtonStatus(ButtonType.Pause, isPaused);
        var isFastForward = updateState == UpdateState.FastForward;
        UpdateControlPanelButtonStatus(ButtonType.FastForward, isFastForward);
    }

    private void HandleCursor()
    {
        var mouseIsInLevelViewPort = _viewport.MouseIsInLevelViewPort;
        if (mouseIsInLevelViewPort)
        {
            var lemmingsNearCursor = _levelCursor.LemmingsNearCursorPosition();
            foreach (var lemming in lemmingsNearCursor)
            {
                _levelCursor.CheckLemming(lemming);
            }

            if (_levelCursor.CurrentlyHighlightedLemming is null)
            {
                _levelControlPanel.TextualData.ClearCursorData();
            }
            else
            {
                var action = _levelCursor.CurrentlyHighlightedLemming.CurrentAction;
                _levelControlPanel.TextualData.SetCursorData(action.LemmingActionName, _levelCursor.NumberOfLemmingsUnderCursor);
            }
        }
        else
        {
            _levelControlPanel.TextualData.ClearCursorData();
        }
    }

    private void UpdateControlPanelButtonStatus(ButtonType buttonType, bool isSelected)
    {
        var button = _levelControlPanel.GetControlPanelButtonOfType(buttonType);
        if (button is null)
            return;
        button.IsSelected = isSelected;
    }

    private void HandleSkillAssignment()
    {
        if (!_inputController.LeftMouseButtonAction.IsPressed)
            return;

        var lemming = _levelCursor.CurrentlyHighlightedLemming;
        if (lemming is null)
            return;

        var selectedSkillId = _levelControlPanel.SelectedSkillButtonId;
        var skillTrackingData = _skillSetManager.GetSkillTrackingData(selectedSkillId);
        if (skillTrackingData is null || skillTrackingData.SkillCount == 0)
            return;

        if (skillTrackingData.CanAssignToLemming(lemming))
        {
            skillTrackingData.Skill.AssignToLemming(lemming);
            skillTrackingData.ChangeSkillCount(-1);
            _levelControlPanel.UpdateSkillCount(_levelControlPanel.SelectedSkillAssignButton, skillTrackingData.SkillCount);
        }
        else
        {
            SetQueuedSkill(lemming, skillTrackingData.Skill);
        }
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
}