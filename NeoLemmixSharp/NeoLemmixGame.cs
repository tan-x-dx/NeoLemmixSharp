using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
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

    private int _width = 1920;
    private int _height = 1080;
    private WindowMode _windowMode;
    public bool IsFullscreen => _windowMode == WindowMode.Fullscreen;
    public bool IsBorderless => _windowMode == WindowMode.Borderless;

    public Size WindowSize => new(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

    public SpriteBatch SpriteBatch => _spriteBatch;

    public NeoLemmixGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            SynchronizeWithVerticalRetrace = false
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.ClientSizeChanged += WindowOnClientSizeChanged;

        IsFixedTimeStep = true;
        TargetElapsedTime = EngineConstants.FramesPerSecondTimeSpan;

        IGameWindow.Instance = this;

        ToggleBorderless();
    }

    protected override void Initialize()
    {
        // make the window fullscreen (but still with border and top control bar)
        var screenWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        var screenHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = screenWidth;
        _graphics.PreferredBackBufferHeight = screenHeight;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        ValidateGameConstants();
        ValidateMaxActionNameLength();
        LoadContent();
    }

    private void WindowOnClientSizeChanged(object? sender, EventArgs e)
    {
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
        FontBank.Initialise(Content);
        MenuSpriteBank.Initialise(Content);
        CommonSprites.Initialise(Content, GraphicsDevice);
        UiSprites.Initialise(Content);

        DefaultLemmingSpriteBank.CreateDefaultLemmingSpriteBank(Content, GraphicsDevice);

        var menuScreen = new MenuScreen(
            Content,
            GraphicsDevice);
        SetScreen(menuScreen);
    }

    /// <summary>
    /// Validation to ensure the expected number of LemmingActions/LemmingSkills
    /// is kept track of, in case new entries are created for these types.
    /// </summary>
    private static void ValidateGameConstants()
    {
        var numberOfActions = LemmingAction.AllItems.Length;
        var numberOfSkills = LemmingSkill.AllItems.Length;

        if (numberOfActions != EngineConstants.NumberOfLemmingActions)
            throw new Exception($"Number of LemmingActions is actually {numberOfActions}! Update {nameof(EngineConstants.NumberOfLemmingActions)}!");

        if (numberOfSkills != EngineConstants.NumberOfLemmingSkills)
            throw new Exception($"Number of LemmingSkills is actually {numberOfSkills}! Update {nameof(EngineConstants.NumberOfLemmingSkills)}!");
    }

    /// <summary>
    /// Validation to ensure there is enough space to write
    /// lemming action text in the buffer in <see cref="ControlPanelTextualData"/>
    /// </summary>
    private static void ValidateMaxActionNameLength()
    {
        var actualMaxActionNameLength = 0;
        foreach (var action in LemmingAction.AllItems)
        {
            actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, action.LemmingActionName.Length);
        }

        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.NeutralControlPanelString.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.ZombieControlPanelString.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.NeutralZombieControlPanelString.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString2Skills.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString3Skills.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString4Skills.Length);
        actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, EngineConstants.AthleteControlPanelString5Skills.Length);

        if (actualMaxActionNameLength != EngineConstants.LongestActionNameLength)
            throw new Exception($"Longest action name length is actually {actualMaxActionNameLength}! Update {nameof(EngineConstants.LongestActionNameLength)}!");
    }

    public void SetScreen(IBaseScreen screen)
    {
        _screen?.Dispose();
        _screenRenderer?.Dispose();

        _screen = screen;
        _screen.OnWindowSizeChanged();
        _screenRenderer = _screen.ScreenRenderer;

        Window.Title = _screen.ScreenTitle;
        _screen.OnSetScreen();

        _screen.Initialise();
    }

    protected override void Update(GameTime gameTime)
    {
        _screen!.Tick(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // if (gameTime.IsRunningSlowly)
        //     return;

        _screenRenderer!.RenderScreen(gameTime, _spriteBatch);
    }

    public void ToggleFullscreen()
    {
        var oldIsFullscreen = _windowMode == WindowMode.Fullscreen;

        if (_windowMode == WindowMode.Windowed)
        {
            _windowMode = WindowMode.Fullscreen;
        }
        else
        {
            _windowMode = WindowMode.Windowed;
        }

        ApplyFullscreenChange(oldIsFullscreen);
    }

    public void ToggleBorderless()
    {
        var oldIsFullscreen = _windowMode == WindowMode.Fullscreen;

        if (_windowMode == WindowMode.Windowed)
        {
            _windowMode = WindowMode.Borderless;
        }
        else
        {
            _windowMode = WindowMode.Windowed;
        }

        ApplyFullscreenChange(oldIsFullscreen);
    }

    private void ApplyFullscreenChange(bool oldIsFullscreen)
    {
        if (_windowMode != WindowMode.Windowed)
        {
            if (oldIsFullscreen)
            {
                ApplyHardwareMode();
            }
            else
            {
                SetFullscreen();
            }
        }
        else
        {
            UnsetFullscreen();
        }
    }

    private void ApplyHardwareMode()
    {
        _graphics.HardwareModeSwitch = _windowMode == WindowMode.Fullscreen;
        _graphics.ApplyChanges();

        Window.AllowUserResizing = false;
        Window.IsBorderless = true;

        _screen?.OnWindowSizeChanged();
    }

    private void SetFullscreen()
    {
        _width = Window.ClientBounds.Width;
        _height = Window.ClientBounds.Height;

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.HardwareModeSwitch = _windowMode == WindowMode.Fullscreen;

        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        Window.AllowUserResizing = false;
        Window.IsBorderless = true;

        _screen?.OnWindowSizeChanged();
    }

    private void UnsetFullscreen()
    {
        _graphics.PreferredBackBufferWidth = _width;
        _graphics.PreferredBackBufferHeight = _height;
        _graphics.IsFullScreen = _windowMode == WindowMode.Fullscreen;
        _graphics.ApplyChanges();

        Window.AllowUserResizing = true;
        Window.IsBorderless = false;

        _screen?.OnWindowSizeChanged();
    }

    public void Escape()
    {
        Exit();
    }
}