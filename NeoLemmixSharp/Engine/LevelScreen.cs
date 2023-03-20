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
    //   private readonly NonRepeatingActionPerformer _pauseAction = new();
    //  private readonly NonRepeatingActionPerformer _pauseAction = new();

    private bool _stopMotion = true;
    private bool _doTick;
    public static LevelScreen? CurrentLevel { get; private set; }

    public ITickable[] LevelObjects { private get; init; }
    public IRenderable[] LevelSprites { private get; init; }
    public LevelTerrain Terrain { get; }
    public SpriteBank SpriteBank { get; }

    public LevelController Controller { get; }
    public NeoLemmixViewPort Viewport { get; init; }
    public TerrainSprite TerrainSprite { get; init; }

    public int Width => Terrain.Width;
    public int Height => Terrain.Height;

    public LevelScreen(
        LevelData levelData,
        LevelTerrain terrain,
        SpriteBank spriteBank)
        : base(levelData.LevelTitle)
    {
        Terrain = terrain;
        SpriteBank = spriteBank;

        Controller = new LevelController();

        CurrentLevel = this;
    }

    public override void Tick(MouseState mouseState)
    {
        Viewport.Tick(mouseState);
        Controller.Tick(mouseState);

        HandleKeyboardInput();

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _doTick = true;
            return;
        }

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
                LevelObjects[i].Tick(mouseState);
            }
        }
    }

    private void HandleKeyboardInput()
    {
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
    }

    public override void Dispose()
    {
    }

    public override void KeyInput(KeyboardState keyboardState)
    {
        Controller.ControllerKeysDown(keyboardState.GetPressedKeys());
    }

    private bool Pause => Controller.CheckKeyDown(Controller.Pause) == KeyStatus.KeyPressed;
    private bool Quit => Controller.CheckKeyDown(Controller.Quit) == KeyStatus.KeyPressed;
    private bool ToggleFullScreen => Controller.CheckKeyDown(Controller.ToggleFullScreen) == KeyStatus.KeyPressed;
    private bool ToggleFastForwards => Controller.CheckKeyDown(Controller.ToggleFastForwards) == KeyStatus.KeyPressed;
}