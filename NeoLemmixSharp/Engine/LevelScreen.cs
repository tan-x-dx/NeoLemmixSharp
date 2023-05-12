using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.Engine.LevelUpdates;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Rendering.Text;
using NeoLemmixSharp.Screen;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    public static LevelScreen CurrentLevel { get; private set; }

    private readonly PixelManager _terrain;
    private readonly SpriteBank _spriteBank;

    private readonly LevelCursor _levelCursor;
    private readonly LevelViewport _viewport;
    private readonly LevelInputController _inputController;
    private readonly LevelControlPanel _controlPanel;
    private readonly ILevelUpdater _levelUpdater;

    private readonly Lemming[] _lemmings;
    // private readonly ITickable[] _gadgets;

    private bool _stopMotion = true;
    private bool _doTick;

    public LevelScreen(
        LevelData levelData,
        Lemming[] lemmings,
        //  ITickable[] gadgets,
        PixelManager terrain,
        SpriteBank spriteBank)
        : base(levelData.LevelTitle)
    {
        _lemmings = lemmings;
        //  _gadgets = gadgets;

        _terrain = terrain;
        _inputController = new LevelInputController();

        _levelUpdater = new StandardLevelUpdater();
        _controlPanel = new LevelControlPanel(levelData.SkillSet, _inputController);
        _levelCursor = new LevelCursor(_controlPanel, _inputController, _lemmings);
        _viewport = new LevelViewport(terrain, _levelCursor, _inputController);

        CurrentLevel = this;
        Orientation.SetTerrain(terrain);
        LemmingAction.SetTerrain(terrain);
        LemmingSkill.SetTerrain(terrain);

        _spriteBank = spriteBank;
        _spriteBank.TerrainSprite.SetViewport(_viewport);
        _spriteBank.LevelCursorSprite.SetLevelCursor(_levelCursor);
    }

    public override void Tick()
    {
        _inputController.Update();

        HandleKeyboardInput();

        var shouldTickLevel = HandleMouseInput();

        if (!shouldTickLevel)
            return;

        for (var i = 0; i < _lemmings.Length; i++)
        {
            var lemming = _lemmings[i];
            _levelUpdater.UpdateLemming(lemming);
        }

        _levelUpdater.Update();
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
            _levelUpdater.ToggleFastForwards();
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
            if (_inputController.LeftMouseButtonStatus == MouseButtonStatusConsts.MouseButtonPressed)
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

    public override void OnWindowSizeChanged()
    {
        _controlPanel.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight);
        _viewport.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight, _controlPanel.ControlPanelScreenHeight);
    }

    public override void Dispose()
    {
        _spriteBank.Dispose();
#pragma warning disable CS8625
        CurrentLevel = null;
        Orientation.SetTerrain(null);
        LemmingAction.SetTerrain(null);
        LemmingSkill.SetTerrain(null);
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

    private bool Pause => _inputController.CheckKeyDown(_inputController.Pause) == KeyStatusConsts.KeyPressed;
    private bool Quit => _inputController.CheckKeyDown(_inputController.Quit) == KeyStatusConsts.KeyPressed;
    private bool ToggleFullScreen => _inputController.CheckKeyDown(_inputController.ToggleFullScreen) == KeyStatusConsts.KeyPressed;
    private bool ToggleFastForwards => _inputController.CheckKeyDown(_inputController.ToggleFastForwards) == KeyStatusConsts.KeyPressed;
}