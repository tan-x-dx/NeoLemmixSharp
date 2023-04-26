using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Text;
using NeoLemmixSharp.Screen;
using NeoLemmixSharp.Util;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    public static LevelScreen CurrentLevel { get; private set; }

    private bool _stopMotion = true;
    private bool _doTick;
    private string _mouseCoords = string.Empty;

    public ITickable[] LevelObjects { private get; init; }
    public ISprite[] LevelSprites { private get; init; }
    public PixelManager Terrain { get; }
    public SpriteBank SpriteBank { get; }
    public FontBank FontBank { get; }
    public LevelControlPanel ControlPanel { get; } = new();

    public LevelController Controller { get; }
    public LevelViewPort Viewport { get; }

    public LevelScreen(
        LevelData levelData,
        PixelManager terrain,
        SpriteBank spriteBank,
        FontBank fontBank)
        : base(levelData.LevelTitle)
    {
        Terrain = terrain;
        SpriteBank = spriteBank;
        FontBank = fontBank;
        Viewport = new LevelViewPort(terrain);

        Controller = new LevelController();

        CurrentLevel = this;
    }

    public override void Tick()
    {
        Controller.Tick();

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

        Viewport.HandleMouseInput(mouseState);
        _mouseCoords = $"({Viewport.ScreenMouseX},{Viewport.ScreenMouseY}) - ({Viewport.ViewportMouseX},{Viewport.ViewportMouseY})";

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _doTick = true;
        }

        var mouseCoords = new LevelPosition(Viewport.ViewportMouseX, Viewport.ViewportMouseY);
        if (mouseState.RightButton == ButtonState.Pressed &&
            !Terrain.PositionOutOfBounds(mouseCoords))
        {
            Terrain.ErasePixel(mouseCoords);
        }

        if (!_stopMotion)
            return true;

        if (!_doTick)
            return false;

        _doTick = false;
        return true;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        SpriteBank.Render(spriteBatch);

        RenderSprites(spriteBatch);

        RenderCursor(spriteBatch, SpriteBank.CursorSprite);

        FontBank.MenuFont.RenderText(spriteBatch, _mouseCoords, 20, 20);
    }

    private void RenderSprites(SpriteBatch spriteBatch)
    {
        var w = Terrain.Width * Viewport.ScaleMultiplier;
        var h = Terrain.Height * Viewport.ScaleMultiplier;
        var maxX = Viewport.NumberOfHorizontalRenderIntervals;
        var maxY = Viewport.NumberOfVerticalRenderIntervals;

        for (var t = 0; t < LevelSprites.Length; t++)
        {
            var sprite = LevelSprites[t];
            var spriteLocation = sprite.GetLocationRectangle();

            var x0 = (spriteLocation.X - Viewport.ViewPortX) * Viewport.ScaleMultiplier + Viewport.ScreenX;
            var y0 = (spriteLocation.Y - Viewport.ViewPortY) * Viewport.ScaleMultiplier + Viewport.ScreenY;

            var y1 = y0;
            for (var i = 0; i < maxX; i++)
            {
                var hInterval = Viewport.GetHorizontalRenderInterval(i);
                if (hInterval.Overlaps(spriteLocation.X, spriteLocation.Width))
                {
                    for (var j = 0; j < maxY; j++)
                    {
                        var vInterval = Viewport.GetVerticalRenderInterval(j);
                        if (vInterval.Overlaps(spriteLocation.Y, spriteLocation.Height))
                        {
                            sprite.RenderAtPosition(spriteBatch, x0, y1, Viewport.ScaleMultiplier);
                        }

                        y1 += h;
                    }
                }

                x0 += w;
                y1 = y0;
            }
        }
    }

    private void RenderCursor(SpriteBatch spriteBatch, ISprite cursorSprite)
    {
        cursorSprite.RenderAtPosition(spriteBatch, Viewport.ScreenMouseX, Viewport.ScreenMouseY, Viewport.ScaleMultiplier);
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
    public bool LemmingsUnderCursor { get; set; }
}