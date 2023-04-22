using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Screen;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    private bool _stopMotion = true;
    private bool _doTick;
    public static LevelScreen CurrentLevel { get; private set; }

    public ITickable[] LevelObjects { private get; init; }
    public ISprite[] LevelSprites { private get; init; }
    public PixelManager Terrain { get; }
    public SpriteBank SpriteBank { get; }

    public LevelController Controller { get; }
    public LevelViewPort Viewport { get; }

    public int Width => Terrain.Width;
    public int Height => Terrain.Height;

    private int _mouseX;
    private int _mouseY;

    public LevelScreen(
        LevelData levelData,
        PixelManager terrain,
        SpriteBank spriteBank)
        : base(levelData.LevelTitle)
    {
        Terrain = terrain;
        SpriteBank = spriteBank;
        Viewport = new LevelViewPort(terrain);

        Controller = new LevelController();

        CurrentLevel = this;
    }

    public override void Tick()
    {
        Controller.Tick();

        HandleKeyboardInput();

        var shouldTickLevel = HandleMouseInput();

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
        Controller.ControllerKeysDown(keyboardState.GetPressedKeys());

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

        _mouseX = (mouseState.X - Viewport.ScreenX) / Viewport.ScaleMultiplier + Viewport.ViewPortX;
        _mouseY = (mouseState.Y - Viewport.ScreenY) / Viewport.ScaleMultiplier + Viewport.ViewPortY;

        Viewport.HandleMouseInput(mouseState);

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _doTick = true;
        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            Terrain.ErasePixel(Terrain.NormalisePosition(new LevelPosition(_mouseX, _mouseY)));
        }

        if (!_stopMotion)
            return _doTick;

        if (!_doTick)
            return false;

        _doTick = false;
        return true;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        SpriteBank.Render(spriteBatch);

        for (var i = 0; i < LevelSprites.Length; i++)
        {
            Viewport.RenderSprite(spriteBatch, LevelSprites[i]);
        }

        spriteBatch.Draw(
            SpriteBank.BoxTexture,
            new Rectangle(_mouseX * Viewport.ScaleMultiplier, _mouseY * Viewport.ScaleMultiplier, 2 * Viewport.ScaleMultiplier, 2 * Viewport.ScaleMultiplier),
            Color.White);
    }

    public override void OnWindowSizeChanged()
    {
        Viewport.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight);
    }

    public override void Dispose()
    {
        SpriteBank.Dispose();
    }

    private bool Pause => Controller.CheckKeyDown(Controller.Pause) == KeyStatus.KeyPressed;
    private bool Quit => Controller.CheckKeyDown(Controller.Quit) == KeyStatus.KeyPressed;
    private bool ToggleFullScreen => Controller.CheckKeyDown(Controller.ToggleFullScreen) == KeyStatus.KeyPressed;
    private bool ToggleFastForwards => Controller.CheckKeyDown(Controller.ToggleFastForwards) == KeyStatus.KeyPressed;
}