using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Actions;
using NeoLemmixSharp.Engine.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Engine.Gadgets;
using NeoLemmixSharp.Engine.Engine.Gadgets.Collections;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Skills;
using NeoLemmixSharp.Engine.Engine.Terrain;
using NeoLemmixSharp.Engine.Engine.Terrain.Masks;
using NeoLemmixSharp.Engine.Engine.Timer;
using NeoLemmixSharp.Engine.Engine.Updates;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LevelScreen : IBaseScreen
{
    public static LevelScreen Current { get; private set; }

    private readonly IGadget[] _gadgets;

    private readonly UpdateScheduler _updateScheduler;
    private readonly LevelInputController _inputController;
    private readonly LevelTimer _levelTimer;
    private readonly SkillSetManager _skillSetManager;
    private readonly ILevelControlPanel _controlPanel;
    private readonly LevelCursor _levelCursor;
    private readonly Viewport _viewport;
    private readonly LemmingManager _lemmingManager;
    private readonly TerrainManager _terrain;
    private readonly LevelRenderer _screenRenderer;

    private bool _stopMotion = true;
    private bool _doTick;

    public bool IsDisposed { get; set; }
    public IGameWindow GameWindow { get; set; }
    public string ScreenTitle { get; init; }

    public bool IsFastForwards { get; private set; }

    public bool DoneAssignmentThisFrame { get; set; }
    public LemmingSkill QueuedSkill { get; private set; } = NoneSkill.Instance;
    public Lemming? QueuedSkillLemming { get; private set; }
    public int QueuedSkillFrame { get; private set; }

    public LevelTimer LevelTimer => _levelTimer;

    public LevelScreen(
        LevelData levelData,
        IGadget[] gadgets,
        UpdateScheduler updateScheduler,
        LevelInputController levelInputController,
        LevelTimer levelTimer,
        SkillSetManager skillSetManager,
        ILevelControlPanel controlPanel,
        LevelCursor cursor,
        Viewport viewport,
        LemmingManager lemmingManager,
        TerrainManager terrain,
        LevelRenderer levelRenderer)
    {
        ScreenTitle = levelData.LevelTitle;

        _gadgets = gadgets;

        _updateScheduler = updateScheduler;
        _inputController = levelInputController;
        _levelTimer = levelTimer;
        _skillSetManager = skillSetManager;
        _controlPanel = controlPanel;
        _levelCursor = cursor;
        _viewport = viewport;
        _lemmingManager = lemmingManager;
        _terrain = terrain;
        _screenRenderer = levelRenderer;

        Orientation.SetTerrain(terrain);
        LemmingAction.SetTerrain(terrain);
        LemmingSkill.SetTerrain(terrain);
        TerrainEraseMask.SetTerrain(terrain);
        TerrainAddMask.SetTerrain(terrain);
        LevelCursor.LevelScreen = this;
        Current = this;
    }

    IScreenRenderer IBaseScreen.ScreenRenderer => _screenRenderer;

    public void Tick()
    {
        DoneAssignmentThisFrame = false;
        _levelCursor.OnNewFrame();
        _lemmingManager.CheckLemmingsUnderCursor();

        _inputController.Update();
        CheckForQueuedAction();
        HandleKeyboardInput();

        var shouldTickLemmings = HandleMouseInput();

        if (!shouldTickLemmings)
            return;

        LevelTimer.Tick();

        for (var i = 0; i < _gadgets.Length; i++)
        {
            _gadgets[i].Tick();
        }

        _lemmingManager.UpdateLemmings();

        _updateScheduler.Tick();
    }

    private void HandleKeyboardInput()
    {
        if (!GameWindow.IsActive)
            return;

        if (Pause)
        {
            _stopMotion = !_stopMotion;
        }

        if (Quit)
        {
            GameWindow.Escape();
        }

        if (ToggleFastForwards)
        {
            if (IsFastForwards)
            {
                _updateScheduler.SetNormalSpeed();
                IsFastForwards = false;
            }
            else
            {
                _updateScheduler.SetFastSpeed();
                IsFastForwards = true;
            }
        }

        if (ToggleFullScreen)
        {
            GameWindow.ToggleBorderless();
        }

        Foo();
    }

    private bool HandleMouseInput()
    {
        if (!GameWindow.IsActive)
            return false;

        if (_viewport.HandleMouseInput())
        {
            if (_inputController.LeftMouseButtonAction.IsPressed)
            {
                _doTick = true;
            }
        }
        else
        {
            _controlPanel.HandleMouseInput();
        }

        _levelCursor.HandleMouseInput();

        if (!_stopMotion)
            return true;

        if (!_doTick)
            return false;

        _doTick = false;
        return true;
    }

    public void ClearQueuedSkill()
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

    private void CheckForQueuedAction()
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

        if (QueuedSkill.CanAssignToLemming(QueuedSkillLemming) && _skillSetManager.NumberOfSkillsAvailable(QueuedSkill) > 0)
        {
            // Record skill assignment, so that we apply it in CheckForReplayAction
            // RecordSkillAssignment(L, NewSkill)
        }
        else
        {
            QueuedSkillFrame++;

            // Delete queued action after 16 frames
            if (QueuedSkillFrame > 15)
            {
                ClearQueuedSkill();
            }
        }
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        _controlPanel.SetWindowDimensions(windowWidth, windowHeight);
        _viewport.SetWindowDimensions(windowWidth, windowHeight, ((LevelControlPanel)_controlPanel).ControlPanelScreenHeight);
        _screenRenderer.OnWindowSizeChanged(windowWidth, windowHeight);
    }

    public void Dispose()
    {
#pragma warning disable CS8625
        Orientation.SetTerrain(null);
        LemmingAction.SetTerrain(null);
        LemmingSkill.SetTerrain(null);
        TerrainEraseMask.SetTerrain(null);
        TerrainAddMask.SetTerrain(null);
        LevelCursor.LevelScreen = null;

        _screenRenderer.Dispose();
        GadgetCollections.ClearGadgets();
        Current = null;
#pragma warning restore CS8625
    }

    private bool Pause => _inputController.Pause.IsPressed;
    private bool Quit => _inputController.Quit.IsPressed;
    private bool ToggleFullScreen => _inputController.ToggleFullScreen.IsPressed;
    private bool ToggleFastForwards => _inputController.ToggleFastForwards.IsPressed;


    private void Foo()
    {
        if (_gadgets.Length == 0)
            return;

        /* var gadget = (ResizeableGadget)_gadgets[0];

         if (_inputController.W.IsKeyDown)
         {
             gadget.SetDeltaHeight(-1);
         }
         else if (_inputController.S.IsKeyDown)
         {
             gadget.SetDeltaHeight(1);
         }
         else
         {
             gadget.SetDeltaHeight(0);
         }

         if (_inputController.A.IsKeyDown)
         {
             gadget.SetDeltaWidth(-1);
         }
         else if (_inputController.D.IsKeyDown)
         {
             gadget.SetDeltaWidth(1);
         }
         else
         {
             gadget.SetDeltaWidth(0);
         }

         if (_inputController.UpArrow.IsKeyDown)
         {
             gadget.SetDeltaY(-1);
         }
         else if (_inputController.DownArrow.IsKeyDown)
         {
             gadget.SetDeltaY(1);
         }
         else
         {
             gadget.SetDeltaY(0);
         }

         if (_inputController.LeftArrow.IsKeyDown)
         {
             gadget.SetDeltaX(-1);
         }
         else if (_inputController.RightArrow.IsKeyDown)
         {
             gadget.SetDeltaX(1);
         }
         else
         {
             gadget.SetDeltaX(0);
         }*/
    }
}