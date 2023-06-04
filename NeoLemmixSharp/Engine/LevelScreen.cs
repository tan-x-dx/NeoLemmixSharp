using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Horizontal;
using NeoLemmixSharp.Engine.LevelBoundaryBehaviours.Vertical;
using NeoLemmixSharp.Engine.LevelUpdates;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Rendering.Text;
using NeoLemmixSharp.Screen;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    private readonly PixelManager _terrain;
    private readonly SpriteBank _spriteBank;

    private readonly SkillSetManager _skillSetManager;
    private readonly LevelCursor _levelCursor;
    private readonly LevelViewport _viewport;
    private readonly LevelInputController _inputController;
    private readonly LevelControlPanel _controlPanel;

    private readonly IHorizontalBoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly IVerticalBoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly IHorizontalViewPortBehaviour _horizontalViewPortBehaviour;
    private readonly IVerticalViewPortBehaviour _verticalViewPortBehaviour;

    private readonly Lemming[] _lemmings;
    // private readonly ITickable[] _gadgets;

    private readonly IFrameUpdater _standardFrameUpdater;
    private readonly IFrameUpdater _fastForwardFrameUpdater;

    private IFrameUpdater _currentlySelectedFrameUpdater;

    private bool _stopMotion = true;
    private bool _doTick;

    public bool IsFastForwards { get; private set; }

    public bool DoneAssignmentThisFrame { get; set; }
    public LemmingSkill QueuedSkill { get; private set; } = NoneSkill.Instance;
    public Lemming? QueuedSkillLemming { get; private set; }
    public int QueuedSkillFrame { get; private set; }

    public LevelScreen(
        LevelData levelData,
        Lemming[] lemmings,
        //  ITickable[] gadgets,
        PixelManager terrain,
        SpriteBank spriteBank)
        : base(levelData.LevelTitle)
    {
        _horizontalBoundaryBehaviour = levelData.HorizontalBoundaryBehaviour ?? new HorizontalWrapBoundaryBehaviour(levelData.LevelWidth);
        _verticalBoundaryBehaviour = levelData.VerticalBoundaryBehaviour ?? new VerticalWrapBoundaryBehaviour(levelData.LevelHeight);

        _horizontalViewPortBehaviour = levelData.HorizontalViewPortBehaviour ?? new HorizontalWrapBehaviour(levelData.LevelWidth);
        _verticalViewPortBehaviour = levelData.VerticalViewPortBehaviour ?? new VerticalWrapViewPortBehaviour(levelData.LevelHeight);

        _lemmings = lemmings;
        //  _gadgets = gadgets;

        _terrain = terrain;
        _inputController = new LevelInputController();
        _skillSetManager = new SkillSetManager(levelData.SkillSetData);

        var isSuperLemmingMode = false;

        _fastForwardFrameUpdater = new FastForwardsFrameUpdater();
        _standardFrameUpdater = isSuperLemmingMode
            ? _fastForwardFrameUpdater
            : new StandardFrameUpdater();

        _currentlySelectedFrameUpdater = _standardFrameUpdater;

        _controlPanel = new LevelControlPanel(_skillSetManager, _inputController);
        _levelCursor = new LevelCursor(_horizontalBoundaryBehaviour, _verticalBoundaryBehaviour, _controlPanel, _inputController, _lemmings);
        _viewport = new LevelViewport(_levelCursor, _inputController, _horizontalViewPortBehaviour, _verticalViewPortBehaviour, _horizontalBoundaryBehaviour, _verticalBoundaryBehaviour);

        Orientation.SetTerrain(terrain);
        LemmingAction.SetTerrain(terrain);
        LemmingSkill.SetTerrain(terrain);
        LevelCursor.LevelScreen = this;

        _spriteBank = spriteBank;
        _spriteBank.TerrainSprite.SetViewport(_viewport);
        _spriteBank.LevelCursorSprite.SetLevelCursor(_levelCursor);
    }

    public override void Tick()
    {
        DoneAssignmentThisFrame = false;

        _inputController.Update();
        CheckForQueuedAction();
        HandleKeyboardInput();

        var shouldTickLevel = HandleMouseInput();

        if (!shouldTickLevel)
            return;

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
        _controlPanel.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight);
        _viewport.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight, _controlPanel.ControlPanelScreenHeight);
    }

    public override void Dispose()
    {
        _spriteBank.Dispose();
#pragma warning disable CS8625
        Orientation.SetTerrain(null);
        LemmingAction.SetTerrain(null);
        LemmingSkill.SetTerrain(null);
        LevelCursor.LevelScreen = null;
        _spriteBank.TerrainSprite.SetViewport(null);
        _spriteBank.LevelCursorSprite.SetLevelCursor(null);
#pragma warning restore CS8625
    }

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
    }

    private bool Pause => _inputController.Pause.IsPressed;
    private bool Quit => _inputController.Quit.IsPressed;
    private bool ToggleFullScreen => _inputController.ToggleFullScreen.IsPressed;
    private bool ToggleFastForwards => _inputController.ToggleFastForwards.IsPressed;
}