using NeoLemmixSharp.Engine.Actions;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Gadgets;
using NeoLemmixSharp.Engine.Input;
using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Engine.Skills;
using NeoLemmixSharp.Engine.Terrain;
using NeoLemmixSharp.Engine.Updates;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering.Level;
using NeoLemmixSharp.Screen;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    private readonly TerrainManager _terrain;

    private readonly SkillSetManager _skillSetManager;
    private readonly LevelCursor _levelCursor;
    private readonly LevelViewport _viewport;
    private readonly LevelInputController _inputController;
    private readonly ILevelControlPanel _controlPanel;

    private readonly Lemming[] _lemmings;
    private readonly IGadget[] _gadgets;

    private readonly IFrameUpdater _standardFrameUpdater;
    private readonly IFrameUpdater _fastForwardFrameUpdater;

    private IFrameUpdater _currentlySelectedFrameUpdater;

    private bool _stopMotion = true;
    private bool _doTick;

    public override LevelRenderer ScreenRenderer { get; }

    public bool IsFastForwards { get; private set; }

    public bool DoneAssignmentThisFrame { get; set; }
    public LemmingSkill QueuedSkill { get; private set; } = NoneSkill.Instance;
    public Lemming? QueuedSkillLemming { get; private set; }
    public int QueuedSkillFrame { get; private set; }

    public LevelScreen(
        LevelData levelData,
        TerrainManager terrain,
        Lemming[] lemmings,
        IGadget[] gadgets,
        LevelInputController levelInputController,
        ILevelControlPanel controlPanel,
        LevelCursor cursor,
        LevelViewport viewport,
        LevelRenderer levelRenderer)
    {
        ScreenTitle = levelData.LevelTitle;
        ScreenRenderer = levelRenderer;

        _lemmings = lemmings;
        _gadgets = gadgets;

        _terrain = terrain;
        _inputController = levelInputController; // new LevelInputController();
        _skillSetManager = new SkillSetManager(levelData.SkillSetData);

        var isSuperLemmingMode = false;

        _fastForwardFrameUpdater = new FastForwardsFrameUpdater();
        _standardFrameUpdater = isSuperLemmingMode
            ? _fastForwardFrameUpdater
            : new StandardFrameUpdater();

        _currentlySelectedFrameUpdater = _standardFrameUpdater;

        _controlPanel = controlPanel; // new LevelControlPanel(_skillSetManager, _inputController);
        _levelCursor = cursor; // new LevelCursor(_horizontalBoundaryBehaviour, _verticalBoundaryBehaviour, _controlPanel, _inputController);
        _viewport = viewport; // = new LevelViewport(_levelCursor, _inputController, _horizontalViewPortBehaviour, _verticalViewPortBehaviour, _horizontalBoundaryBehaviour, _verticalBoundaryBehaviour);

        Orientation.SetTerrain(terrain);
        LemmingAction.SetTerrain(terrain);
        LemmingSkill.SetTerrain(terrain);
        LevelCursor.LevelScreen = this;

        // terrain.TerrainRenderer.SetViewport(_viewport);
    }

    public override void Tick()
    {
        DoneAssignmentThisFrame = false;
        _levelCursor.OnNewFrame();

        for (var i = 0; i < _lemmings.Length; i++)
        {
            _levelCursor.CheckLemming(_lemmings[i]);
        }

        _inputController.Update();
        CheckForQueuedAction();
        HandleKeyboardInput();

        var shouldTickLemmings = HandleMouseInput();

        if (!shouldTickLemmings)
            return;

        for (var i = 0; i < _gadgets.Length; i++)
        {
            _gadgets[i].Tick();
        }

        for (var i = 0; i < _lemmings.Length; i++)
        {
            _currentlySelectedFrameUpdater.UpdateLemming(_lemmings[i]);
        }

        _currentlySelectedFrameUpdater.Update();
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
                _currentlySelectedFrameUpdater = _standardFrameUpdater;
                IsFastForwards = false;
            }
            else
            {
                _currentlySelectedFrameUpdater = _fastForwardFrameUpdater;
                IsFastForwards = true;
            }
        }

        if (ToggleFullScreen)
        {
            GameWindow.ToggleBorderless();
        }
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

        if (QueuedSkill.CanAssignToLemming(QueuedSkillLemming) && QueuedSkill.CurrentNumberOfSkillsAvailable > 0)
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

    public override void OnWindowSizeChanged()
    {
        var windowWidth = GameWindow.WindowWidth;
        var windowHeight = GameWindow.WindowHeight;

        _controlPanel.SetWindowDimensions(windowWidth, windowHeight);
        _viewport.SetWindowDimensions(windowWidth, windowHeight, ((LevelControlPanel)_controlPanel).ControlPanelScreenHeight);
        ScreenRenderer.OnWindowSizeChanged(windowWidth, windowHeight);
    }

    public override void Dispose()
    {
#pragma warning disable CS8625
        Orientation.SetTerrain(null);
        LemmingAction.SetTerrain(null);
        LemmingSkill.SetTerrain(null);
        LevelCursor.LevelScreen = null;

        ScreenRenderer.Dispose();
        GadgetCollections.ClearGadgets();
#pragma warning restore CS8625
    }
    /*
    public override ScreenRenderer CreateScreenRenderer(
        SpriteBank spriteBank,
        FontBank fontBank,
        ISprite[] levelSprites)
    {
        return new LevelRenderer(
            _terrain,
            _viewport,
            levelSprites,
            spriteBank,
            fontBank,
            _controlPanel);
    }*/

    private bool Pause => _inputController.Pause.IsPressed;
    private bool Quit => _inputController.Quit.IsPressed;
    private bool ToggleFullScreen => _inputController.ToggleFullScreen.IsPressed;
    private bool ToggleFastForwards => _inputController.ToggleFastForwards.IsPressed;
}