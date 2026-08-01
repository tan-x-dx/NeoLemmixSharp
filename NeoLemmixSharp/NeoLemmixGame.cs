using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Config;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Shaders;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.Menu;
using NeoLemmixSharp.Ui.Data;
using System;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp;

public sealed partial class NeoLemmixGame : Game, IGameWindow
{
    private readonly GraphicsDeviceManager _graphics;

    private SpriteBatch _spriteBatch = null!;
    private IBaseScreen? _screen;
    private IScreenRenderer? _screenRenderer;

    private RectangularRegion _windowedBounds = new(new Common.Point(200, 400), new Size(1920, 1080));

    private WindowMode _windowMode = WindowMode.Windowed;
    private WindowMode _fullscreenWindowMode = WindowMode.Borderless;

    public bool IsFullscreen => _windowMode == WindowMode.Fullscreen;
    public bool IsBorderless => _windowMode == WindowMode.Borderless;

    public Common.Point WindowPosition => new(Window.ClientBounds.Left, Window.ClientBounds.Top);
    public Size WindowSize => new(Window.ClientBounds.Width, Window.ClientBounds.Height);

    public UserSettings UserSettings { get; set; } = new();
    public SpriteBatch SpriteBatch => _spriteBatch;

    public NeoLemmixGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            SynchronizeWithVerticalRetrace = false,
            GraphicsProfile = GraphicsProfile.HiDef
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.ClientSizeChanged += WindowOnClientSizeChanged;

        IsFixedTimeStep = true;
        TargetElapsedTime = EngineConstants.MenuFramesPerSecondTimeSpan;

        IGameWindow.Instance = this;
    }

    protected override void Initialize()
    {
        ValidateMaxActionNameLength();
        LoadContent();

        UnsetFullscreen();
        // ToggleBorderless();
    }

    private void WindowOnClientSizeChanged(object? sender, EventArgs e)
    {
        if (_windowMode == WindowMode.Windowed)
        {
            var windowBounds = Window.ClientBounds;

            var position = new Common.Point(windowBounds.Left, windowBounds.Top);
            var size = new Size(
                Math.Max(windowBounds.Width, 64),
                Math.Max(windowBounds.Height, 64));
            _windowedBounds = new RectangularRegion(position, size);
        }

        _screen?.OnWindowSizeChanged();
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
        base.OnActivated(sender, args);

        _screen?.OnActivated();
    }

    public void CaptureCursor()
    {
        var rect = Window.ClientBounds;
        rect.Width += rect.X;
        rect.Height += rect.Y;

        ClipCursor(ref rect);
    }

    [LibraryImport("user32.dll")]
    private static partial void ClipCursor(ref Rectangle rect);

    protected override void LoadContent()
    {
        LoadResources();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        RootDirectoryManager.Initialise();
        ShaderBank.Initialise(Content);
        FontBank.Initialise(Content);
        MenuSpriteBank.Initialise(Content);
        CommonSprites.Initialise(Content, GraphicsDevice);
        UiSprites.Initialise(Content);

        StyleCache.Initialise();
        TextureCache.Initialise(Content, GraphicsDevice);

        var menuScreen = new MenuScreen(Content, GraphicsDevice);
        SetScreen(menuScreen);
        menuScreen.Initialise();
    }

    /// <summary>
    /// Validation to ensure there is enough space to write
    /// lemming action text in the buffer in <see cref="ControlPanelTextualData"/>
    /// </summary>
    private static void ValidateMaxActionNameLength()
    {
        var actualMaxActionNameLength = 0;

        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.NeutralControlPanelString.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.ZombieControlPanelString.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.NeutralZombieControlPanelString.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString2Skills.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString3Skills.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString4Skills.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString5Skills.Length);

        foreach (var action in LemmingAction.AllItems)
        {
            actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, action.LemmingActionName.Length);
        }

        if (actualMaxActionNameLength != LemmingActionConstants.LongestActionNameLength)
            throw new Exception($"Longest action name length is actually {actualMaxActionNameLength}! Update {nameof(LemmingActionConstants.LongestActionNameLength)}!");
    }

    public void SetScreen(IBaseScreen screen)
    {
        _screen = screen;
        _screen.OnWindowSizeChanged();
        _screenRenderer = _screen.ScreenRenderer;

        Window.Title = _screen.ScreenTitle;
        TargetElapsedTime = _screen.GetTargetElapsedTime();
        _screen.OnActivated();
    }

    public void ExitLevel()
    {
        var levelScreen = LevelScreen.Instance ?? throw new InvalidOperationException("No level to exit!");

        var levelEndPage = MenuScreen.Instance.MenuPageCreator.CreateLevelEndPage(levelScreen.LevelData, levelScreen.GenerateSuccessOutcome());
        MenuScreen.Instance.SetNextPage(levelEndPage);

        SetScreen(MenuScreen.Instance);
    }

    protected override void Update(GameTime gameTime)
    {
        if (!IsActive)
            return;

        _screen!.Tick(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // if (gameTime.IsRunningSlowly)
        //     return;

        _screenRenderer!.RenderScreen(gameTime, _spriteBatch);
    }

    public void ToggleFullscreenSetting()
    {
        if (_windowMode != WindowMode.Windowed)
        {
            UnsetFullscreen();
            return;
        }

        if (_fullscreenWindowMode == WindowMode.Fullscreen)
        {
            SetFullscreen();
        }
        else
        {
            SetBorderless();
        }
    }

    public void ToggleFullscreen()
    {
        if (_windowMode == WindowMode.Windowed)
        {
            SetFullscreen();
        }
        else
        {
            UnsetFullscreen();
        }
    }

    public void ToggleBorderless()
    {
        if (_windowMode == WindowMode.Windowed)
        {
            SetBorderless();
        }
        else
        {
            UnsetFullscreen();
        }
    }

    private void SetBorderless()
    {
        _windowMode = WindowMode.Borderless;

        Window.AllowUserResizing = false;
        Window.IsBorderless = true;
        Window.Position = Microsoft.Xna.Framework.Point.Zero;

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        //    _graphics.HardwareModeSwitch = false;
        //    _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        _screen?.OnWindowSizeChanged();
    }

    private void SetFullscreen()
    {
        _windowMode = WindowMode.Fullscreen;

        Window.AllowUserResizing = false;
        Window.IsBorderless = true;
        Window.Position = Microsoft.Xna.Framework.Point.Zero;

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        //   _graphics.HardwareModeSwitch = true;
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        _screen?.OnWindowSizeChanged();
    }

    private void UnsetFullscreen()
    {
        _windowMode = WindowMode.Windowed;

        Window.AllowUserResizing = true;
        Window.IsBorderless = false;
        Window.Position = new(_windowedBounds.X, _windowedBounds.Y);

        _graphics.PreferredBackBufferWidth = _windowedBounds.W;
        _graphics.PreferredBackBufferHeight = _windowedBounds.H;
        //   _graphics.HardwareModeSwitch = false;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        _screen?.OnWindowSizeChanged();
    }

    public void Escape()
    {
        Exit();
    }

    protected override void Dispose(bool disposing)
    {
        MenuScreen.Instance?.Dispose();

        base.Dispose(disposing);
    }
}
