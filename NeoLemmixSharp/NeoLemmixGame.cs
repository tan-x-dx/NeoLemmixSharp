using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
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

    private int _baseWindowWidth = 2560;
    private int _baseWindowHeight = 1440;

    private int _windowWidth;
    private int _windowHeight;
    private WindowMode _windowMode = WindowMode.Windowed;
    public bool IsFullscreen => _windowMode == WindowMode.Fullscreen;
    public bool IsBorderless => _windowMode == WindowMode.Borderless;

    public Size WindowSize => new(Window.ClientBounds.Width, Window.ClientBounds.Height);

    public SpriteBatch SpriteBatch => _spriteBatch;

    public NeoLemmixGame()
    {
        _windowWidth = _baseWindowWidth;
        _windowHeight = _baseWindowHeight;

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
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = _windowWidth;
        _graphics.PreferredBackBufferHeight = _windowHeight;
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

        StyleCache.Initialise();
        TextureCache.Initialise(Content, GraphicsDevice);

        var menuScreen = new MenuScreen(Content, GraphicsDevice);
        SetScreen(menuScreen);
        menuScreen.Initialise();
    }

    /// <summary>
    /// Validation to ensure the expected number of LemmingActions/LemmingSkills
    /// is kept track of, in case new entries are created for these types.
    /// </summary>
    private static void ValidateGameConstants()
    {
        var numberOfActions = LemmingAction.AllItems.Length;
        var numberOfSkills = LemmingSkill.AllItems.Length;

        if (numberOfActions != LemmingActionConstants.NumberOfLemmingActions)
            throw new Exception($"Number of LemmingActions is actually {numberOfActions}! Update {nameof(LemmingActionConstants.NumberOfLemmingActions)}!");

        if (numberOfSkills != LemmingSkillConstants.NumberOfLemmingSkills)
            throw new Exception($"Number of LemmingSkills is actually {numberOfSkills}! Update {nameof(LemmingSkillConstants.NumberOfLemmingSkills)}!");
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
        if (_windowMode == WindowMode.Windowed)
        {
            _windowMode = WindowMode.Fullscreen;
        }
        else
        {
            _windowMode = WindowMode.Windowed;
        }

        ApplyFullscreenChange();
    }

    public void ToggleBorderless()
    {
        if (_windowMode == WindowMode.Windowed)
        {
            _windowMode = WindowMode.Borderless;
        }
        else
        {
            _windowMode = WindowMode.Windowed;
        }

        ApplyFullscreenChange();
    }

    private void ApplyFullscreenChange()
    {
        switch (_windowMode)
        {
            case WindowMode.Windowed:
                UnsetFullscreen();
                break;
            case WindowMode.Borderless:
                ApplyHardwareMode();
                break;
            case WindowMode.Fullscreen:
                SetFullscreen();
                break;
        }
    }

    private void ApplyHardwareMode()
    {
        _windowWidth = Window.ClientBounds.Width;
        _windowHeight = Window.ClientBounds.Height;

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.HardwareModeSwitch = false;
        _graphics.ApplyChanges();

        Window.AllowUserResizing = false;
        Window.IsBorderless = true;

        _screen?.OnWindowSizeChanged();
    }

    private void SetFullscreen()
    {
        _windowWidth = Window.ClientBounds.Width;
        _windowHeight = Window.ClientBounds.Height;

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.HardwareModeSwitch = true;

        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        Window.AllowUserResizing = false;
        Window.IsBorderless = true;

        _screen?.OnWindowSizeChanged();
    }

    private void UnsetFullscreen()
    {
        _graphics.PreferredBackBufferWidth = _baseWindowWidth;
        _graphics.PreferredBackBufferHeight = _baseWindowHeight;
        _windowWidth = _baseWindowWidth;
        _windowHeight = _baseWindowHeight;
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

    protected override void Dispose(bool disposing)
    {
        MenuScreen.Instance.Dispose();

        base.Dispose(disposing);
    }
}
