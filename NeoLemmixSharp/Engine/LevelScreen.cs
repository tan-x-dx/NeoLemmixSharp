using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.Engine.LemmingSkills;
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

    private readonly Lemming[] _lemmings;
    private readonly ITickable[] _gadgets;

    private readonly PixelManager _terrain;
    private readonly SpriteBank _spriteBank;

    private readonly LevelCursor _levelCursor;
    private readonly LevelViewport _viewport;
    private readonly LevelKeyController _keyController;
    private readonly LevelControlPanel _controlPanel;

    private bool _stopMotion = true;
    private bool _doTick;

    public LevelScreen(
        LevelData levelData,
        Lemming[] lemmings,
        ITickable[] gadgets,
        PixelManager terrain,
        SpriteBank spriteBank)
        : base(levelData.LevelTitle)
    {
        _lemmings = lemmings;
        _gadgets = gadgets;

        _terrain = terrain;
        _keyController = new LevelKeyController();

        _controlPanel = new LevelControlPanel(levelData.SkillSet);
        _levelCursor = new LevelCursor(_controlPanel, _lemmings);
        _viewport = new LevelViewport(terrain, _levelCursor);

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
        _keyController.Tick();

        HandleKeyboardInput();

        var shouldTickLevel = HandleMouseInput();

        if (!shouldTickLevel)
            return;

        for (var i = 0; i < _lemmings.Length; i++)
        {
            if (_lemmings[i].ShouldTick)
            {
                _lemmings[i].Tick();
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (!GameWindow.IsActive)
            return;

        var keyboardState = Keyboard.GetState();
        _keyController.ControllerKeysDown(keyboardState.GetPressedKeys());

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
            GameWindow.SetFastForwards(!GameWindow.IsFastForwards);
        }

        if (ToggleFullScreen)
        {
            GameWindow.ToggleFullScreen();
        }
    }

    private bool HandleMouseInput()
    {
        if (!GameWindow.IsActive)
            return false;

        var mouseState = Mouse.GetState();

        if (_viewport.HandleMouseInput(mouseState))
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _doTick = true;
            }
        }
        else
        {
            _controlPanel.HandleMouseInput(mouseState);
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

    private bool Pause => _keyController.CheckKeyDown(_keyController.Pause) == KeyStatus.KeyPressed;
    private bool Quit => _keyController.CheckKeyDown(_keyController.Quit) == KeyStatus.KeyPressed;
    private bool ToggleFullScreen => _keyController.CheckKeyDown(_keyController.ToggleFullScreen) == KeyStatus.KeyPressed;
    private bool ToggleFastForwards => _keyController.CheckKeyDown(_keyController.ToggleFastForwards) == KeyStatus.KeyPressed;
}