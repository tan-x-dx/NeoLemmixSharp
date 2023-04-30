using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.Engine.LemmingSkills;
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

    private readonly PixelManager _terrain;
    private readonly LevelController _controller;
    private readonly LevelViewport _viewport;

    public ITickable[] LevelObjects { private get; init; }
    public ISprite[] LevelSprites { private get; init; }
    public SpriteBank SpriteBank { get; }
    public FontBank FontBank { get; }
    public LevelControlPanel ControlPanel { get; } = new();
    public LevelCursor LevelCursor { get; } = new();

    public LevelScreen(
        LevelData levelData,
        PixelManager terrain,
        SpriteBank spriteBank,
        FontBank fontBank)
        : base(levelData.LevelTitle)
    {
        _terrain = terrain;
        SpriteBank = spriteBank;
        FontBank = fontBank;
        _viewport = new LevelViewport(terrain);

        _controller = new LevelController();

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
        _mouseCoords = $"({_viewport.ScreenMouseX},{_viewport.ScreenMouseY}) - ({_viewport.ViewportMouseX},{_viewport.ViewportMouseY})";
        LevelCursor.CursorX = _viewport.ViewPortX;
        LevelCursor.CursorY = _viewport.ViewPortY;
        LevelCursor.Tick();

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _doTick = true;
        }

        var mouseCoords = new LevelPosition(_viewport.ViewportMouseX, _viewport.ViewportMouseY);
        if (mouseState.RightButton == ButtonState.Pressed &&
            !_terrain.PositionOutOfBounds(mouseCoords))
        {
            _terrain.ErasePixel(mouseCoords);
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

        SpriteBank.LevelCursorSprite.RenderAtPosition(spriteBatch, _viewport.ScreenMouseX, _viewport.ScreenMouseY, _viewport.ScaleMultiplier);

        FontBank.MenuFont.RenderText(spriteBatch, _mouseCoords, 20, 20);
    }

    private void RenderSprites(SpriteBatch spriteBatch)
    {
        var w = _terrain.Width * _viewport.ScaleMultiplier;
        var h = _terrain.Height * _viewport.ScaleMultiplier;
        var maxX = _viewport.NumberOfHorizontalRenderIntervals;
        var maxY = _viewport.NumberOfVerticalRenderIntervals;

        for (var t = 0; t < LevelSprites.Length; t++)
        {
            var sprite = LevelSprites[t];
            var spriteLocation = sprite.GetLocationRectangle();

            var x0 = (spriteLocation.X - _viewport.ViewPortX) * _viewport.ScaleMultiplier + _viewport.ScreenX;
            var y0 = (spriteLocation.Y - _viewport.ViewPortY) * _viewport.ScaleMultiplier + _viewport.ScreenY;

            var y1 = y0;
            for (var i = 0; i < maxX; i++)
            {
                var hInterval = _viewport.GetHorizontalRenderInterval(i);
                if (hInterval.Overlaps(spriteLocation.X, spriteLocation.Width))
                {
                    for (var j = 0; j < maxY; j++)
                    {
                        var vInterval = _viewport.GetVerticalRenderInterval(j);
                        if (vInterval.Overlaps(spriteLocation.Y, spriteLocation.Height))
                        {
                            sprite.RenderAtPosition(spriteBatch, x0, y1, _viewport.ScaleMultiplier);
                        }

                        y1 += h;
                    }
                }

                x0 += w;
                y1 = y0;
            }
        }
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

    private bool Pause => _controller.CheckKeyDown(_controller.Pause) == KeyStatus.KeyPressed;
    private bool Quit => _controller.CheckKeyDown(_controller.Quit) == KeyStatus.KeyPressed;
    private bool ToggleFullScreen => _controller.CheckKeyDown(_controller.ToggleFullScreen) == KeyStatus.KeyPressed;
    private bool ToggleFastForwards => _controller.CheckKeyDown(_controller.ToggleFastForwards) == KeyStatus.KeyPressed;
}