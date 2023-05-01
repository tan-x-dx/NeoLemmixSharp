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

    private bool _stopMotion = true;
    private bool _doTick;

    private readonly PixelManager _terrain;
    private readonly LevelController _controller;
    private readonly LevelViewport _viewport;

    public ITickable[] LevelObjects { private get; init; }
    public ISprite[] LevelSprites { private get; init; }
    public SpriteBank SpriteBank { get; }
    public FontBank FontBank { get; }
    public LevelControlPanel ControlPanel { get; }
    public LevelCursor LevelCursor { get; } = new();

    public LevelScreen(
        LevelData levelData,
        PixelManager terrain,
        SpriteBank spriteBank,
        FontBank fontBank)
        : base(levelData.LevelTitle)
    {
        _terrain = terrain;
        _controller = new LevelController();
        _viewport = new LevelViewport(terrain);

        SpriteBank = spriteBank;
        FontBank = fontBank;
        ControlPanel = new LevelControlPanel(levelData.SkillSet);

        CurrentLevel = this;
        Orientation.SetTerrain(terrain);
        LemmingAction.SetTerrain(terrain);
        LemmingSkill.SetTerrain(terrain);
        spriteBank.TerrainSprite.SetViewport(_viewport);
        spriteBank.LevelCursorSprite.SetLevelCursor(LevelCursor);
    }

    public override void Tick()
    {
        _controller.Tick();

        HandleKeyboardInput();

        var shouldTickLevel = HandleMouseInput();

        //CheckLemmingsUnderCursor();

        if (!shouldTickLevel)
            return;

        for (var i = 0; i < LevelObjects.Length; i++)
        {
            if (LevelObjects[i].ShouldTick)
            {
                LevelObjects[i].Tick();
            }
        }
    }

    private void HandleKeyboardInput()
    {
        if (!GameWindow.IsActive)
            return;

        var keyboardState = Keyboard.GetState();
        _controller.ControllerKeysDown(keyboardState.GetPressedKeys());

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

        _viewport.HandleMouseInput(mouseState);
        LevelCursor.CursorX = _viewport.ViewPortX;
        LevelCursor.CursorY = _viewport.ViewPortY;
        LevelCursor.Tick();

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _doTick = true;
        }

        if (!_stopMotion)
            return true;

        if (!_doTick)
            return false;

        _doTick = false;
        return true;
    }

    public override void OnWindowSizeChanged()
    {
        _viewport.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight);
    }

    public override void Dispose()
    {
        SpriteBank.Dispose();
#pragma warning disable CS8625
        CurrentLevel = null;
        Orientation.SetTerrain(null);
        LemmingAction.SetTerrain(null);
        LemmingSkill.SetTerrain(null);
        SpriteBank.TerrainSprite.SetViewport(null);
        SpriteBank.LevelCursorSprite.SetLevelCursor(null);
#pragma warning restore CS8625
    }

    public override ScreenRenderer CreateScreenRenderer()
    {
        return new LevelRenderer(
            _terrain,
            _viewport,
            LevelSprites,
            SpriteBank,
            FontBank,
            ControlPanel);
    }

    private bool Pause => _controller.CheckKeyDown(_controller.Pause) == KeyStatus.KeyPressed;
    private bool Quit => _controller.CheckKeyDown(_controller.Quit) == KeyStatus.KeyPressed;
    private bool ToggleFullScreen => _controller.CheckKeyDown(_controller.ToggleFullScreen) == KeyStatus.KeyPressed;
    private bool ToggleFastForwards => _controller.CheckKeyDown(_controller.ToggleFastForwards) == KeyStatus.KeyPressed;
}