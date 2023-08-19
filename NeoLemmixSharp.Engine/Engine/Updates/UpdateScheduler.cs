﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Skills;
using NeoLemmixSharp.Engine.Engine.Timer;

namespace NeoLemmixSharp.Engine.Engine.Updates;

public sealed class UpdateScheduler
{
    private readonly ItemWrapper<int> _elapsedTicks;

    private readonly PauseUpdater _pauseUpdater;
    private readonly StandardFrameUpdater _standardFrameUpdater;
    private readonly FastForwardsFrameUpdater _fastForwardFrameUpdater;

    private readonly bool _superLemmingMode;

    private readonly ILevelControlPanel _levelControlPanel;
    private readonly LevelInputController _inputController;
    private readonly Viewport _viewport;
    private readonly LevelCursor _levelCursor;
    private readonly LevelTimer _levelTimer;
    private readonly LemmingManager _lemmingManager;

    public bool DoneAssignmentThisFrame { get; set; }
    public LemmingSkill QueuedSkill { get; private set; } = NoneSkill.Instance;
    public Lemming? QueuedSkillLemming { get; private set; }
    public int QueuedSkillFrame { get; private set; }

    public IFrameUpdater CurrentlySelectedFrameUpdater { get; private set; }

    public UpdateScheduler(
        bool superLemmingMode,
        ILevelControlPanel levelControlPanel,
        Viewport viewport,
        LevelCursor levelCursor,
        LevelInputController inputController,
        LevelTimer levelTimer,
        LemmingManager lemmingManager)
    {
        _superLemmingMode = superLemmingMode;
        _levelControlPanel = levelControlPanel;
        _levelCursor = levelCursor;
        _viewport = viewport;
        _inputController = inputController;
        _levelTimer = levelTimer;
        _lemmingManager = lemmingManager;

        _elapsedTicks = new ItemWrapper<int>();
        _pauseUpdater = new PauseUpdater(_elapsedTicks);
        _fastForwardFrameUpdater = new FastForwardsFrameUpdater(_elapsedTicks);
        _standardFrameUpdater = new StandardFrameUpdater(_elapsedTicks);

        CurrentlySelectedFrameUpdater = _standardFrameUpdater;
    }

    public void Tick()
    {
        _inputController.Tick();

        HandleKeyboardInput();
        HandleMouseInput();

        var mouseIsInLevelViewPort = _viewport.MouseIsInLevelViewPort;
        _levelCursor.OnNewFrame();

        foreach (var lemming in _lemmingManager.AllLemmings)
        {
            if (mouseIsInLevelViewPort)
            {
                _levelCursor.CheckLemming(lemming);
            }

            CurrentlySelectedFrameUpdater.UpdateLemming(lemming);
        }

        var didTick = CurrentlySelectedFrameUpdater.Tick();

        if (didTick)
        {
            if (_elapsedTicks.Item % GameConstants.FramesPerSecond == 0)
            {
                _levelTimer.Tick();
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (_inputController.Pause.IsPressed)
        {
            if (CurrentlySelectedFrameUpdater.UpdateState == UpdateState.Paused)
            {
                SetNormalSpeed();
            }
            else
            {
                SetPaused();
            }

            return;
        }

        if (_inputController.ToggleFastForwards.IsPressed)
        {
            if (CurrentlySelectedFrameUpdater.UpdateState == UpdateState.FastForward)
            {
                SetNormalSpeed();
            }
            else
            {
                SetFastSpeed();
            }
        }
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

    public void UpdateLemming(Lemming lemming)
    {
        CurrentlySelectedFrameUpdater.UpdateLemming(lemming);
    }

    private void SetPaused()
    {
        CurrentlySelectedFrameUpdater = _pauseUpdater;
    }

    private void SetNormalSpeed()
    {
        CurrentlySelectedFrameUpdater = _superLemmingMode
            ? _fastForwardFrameUpdater
            : _standardFrameUpdater;
    }

    private void SetFastSpeed()
    {
        CurrentlySelectedFrameUpdater = _fastForwardFrameUpdater;
    }

    private void ClearQueuedSkill()
    {
        QueuedSkill = NoneSkill.Instance;
        QueuedSkillLemming = null;
        QueuedSkillFrame = 0;
    }

    public void SetQueuedSkill(Lemming lemming, LemmingSkill lemmingSkill)
    {
        QueuedSkill = lemmingSkill;
        QueuedSkillLemming = lemming;
        QueuedSkillFrame = 0;
    }

    public void CheckForQueuedAction()
    {
        // First check whether there was already a skill assignment this frame
        //    if Assigned(fReplayManager.Assignment[fCurrentIteration, 0]) then Exit;

        if (QueuedSkill == NoneSkill.Instance || QueuedSkillLemming == null)
        {
            ClearQueuedSkill();
            return;
        }

        if (false) //(lemming.IsRemoved || !lemming.CanReceiveSkills || lemming.IsTeleporting) // CannotReceiveSkills covers neutral and zombie
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
            QueuedSkillFrame++;

            // Delete queued action after 16 frames
            if (QueuedSkillFrame > 15)
            {
                ClearQueuedSkill();
            }
        }
    }

}