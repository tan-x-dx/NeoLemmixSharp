using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Terrain;
using NeoLemmixSharp.Screen;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    private bool _stopMotion = true;
    private bool _doTick;
    public static LevelScreen? CurrentLevel { get; private set; }

    public ITickable[] LevelObjects { private get; init; }
    public IRenderable[] LevelSprites { private get; init; }
    public PixelManager Terrain { get; }
    public SpriteBank SpriteBank { get; }

    public LevelController Controller { get; }
    public LevelViewPort Viewport { get; }
    public TerrainSprite TerrainSprite { get; init; }

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
        Viewport = new LevelViewPort(Width, Height);

        Controller = new LevelController();

        CurrentLevel = this;
    }

    public override void Tick()
    {
        Controller.Tick();

        HandleMouseInput();
        HandleKeyboardInput();

        if (_stopMotion)
        {
            if (_doTick)
            {
                _doTick = false;
            }
            else
            {
                return;
            }
        }

        for (var i = 0; i < LevelObjects.Length; i++)
        {
            if (LevelObjects[i].ShouldTick)
            {
                LevelObjects[i].Tick();
            }
        }
    }

    private void HandleMouseInput()
    {
        if (!GameWindow.IsActive)
            return;

        var mouseState = Mouse.GetState();

        _mouseX = mouseState.X;
        _mouseY = mouseState.Y;

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _doTick = true;
            return;
        }

        Viewport.HandleMouseInput(mouseState);
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

    public override void Render(SpriteBatch spriteBatch)
    {
        TerrainSprite.Render(spriteBatch);

        for (var i = 0; i < LevelSprites.Length; i++)
        {
            if (LevelSprites[i].ShouldRender)
            {
                LevelSprites[i].Render(spriteBatch);
            }
        }

        /* int r = _mouseX == 0
             ? 255
             : 0;

         int b = _mouseY == 0
             ? 255
             : 0;

         var colour = new Color(r, 255, b);

         spriteBatch.Draw(SpriteBank.GetBox(), new Rectangle(_mouseX, _mouseY, 20, 20), colour);*/

    }

    public override void OnWindowSizeChanged()
    {
        Viewport.SetWindowDimensions(GameWindow.WindowWidth, GameWindow.WindowHeight);
    }

    public override void Dispose()
    {
    }

    private bool Pause => Controller.CheckKeyDown(Controller.Pause) == KeyStatus.KeyPressed;
    private bool Quit => Controller.CheckKeyDown(Controller.Quit) == KeyStatus.KeyPressed;
    private bool ToggleFullScreen => Controller.CheckKeyDown(Controller.ToggleFullScreen) == KeyStatus.KeyPressed;
    private bool ToggleFastForwards => Controller.CheckKeyDown(Controller.ToggleFastForwards) == KeyStatus.KeyPressed;
}