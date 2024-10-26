using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Ui;
using MLEM.Ui.Style;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using NeoLemmixSharp.Menu;
using System;
using System.Runtime.InteropServices;
using MLEM.Ui.Elements;

namespace NeoLemmixSharp;

public sealed partial class NeoLemmixGame : Game, IGameWindow
{
    private const string UiRootElementKey = nameof(UiRootElementKey);

    private readonly GraphicsDeviceManager _graphics;

    private UiSystem _uiSystem;
    private SpriteBatch _spriteBatch;
    private IBaseScreen? _screen;
    private IScreenRenderer? _screenRenderer;

    private int _width;
    private int _height;
    private bool _isBorderless;
    private bool _isFullscreen;
    public bool IsFullscreen => _isFullscreen;

    public int WindowWidth => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
    public int WindowHeight => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

    public UiSystem UiSystem => _uiSystem;
    public Element UiRoot => _uiSystem.Get(UiRootElementKey).Element;

    public NeoLemmixGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            SynchronizeWithVerticalRetrace = false
        };

        Content.RootDirectory = "Content";
        Window.AllowUserResizing = false;
        Window.IsBorderless = true;
        IsMouseVisible = true;
        _isBorderless = true;

        Window.ClientSizeChanged += WindowOnClientSizeChanged;

        IsFixedTimeStep = true;
        TargetElapsedTime = EngineConstants.FramesPerSecondTimeSpan;

        IGameWindow.Instance = this;

        ToggleBorderless();
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

    protected override void Initialize()
    {
        // make the window fullscreen (but still with border and top control bar)
        var screenWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
        var screenHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = screenWidth;
        _graphics.PreferredBackBufferHeight = screenHeight;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        InitialiseGameConstants();
        ValidateMaxActionNameLength();
        LoadContent();
    }

    protected override void LoadContent()
    {
        LoadResources();

        RootDirectoryManager.Initialise();
        FontBank.Initialise(Content);
        MenuSpriteBank.Initialise(Content);
        CommonSprites.Initialise(Content, GraphicsDevice);

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _uiSystem = new UiSystem(this, new UntexturedStyle(_spriteBatch));

        var ui = new Group(Anchor.TopLeft, Vector2.One, false);
        _uiSystem.Add(UiRootElementKey, ui);

        TerrainMasks.InitialiseTerrainMasks(Content, GraphicsDevice);
        DefaultLemmingSpriteBank.CreateDefaultLemmingSpriteBank(Content, GraphicsDevice);

        var menuScreen = new MenuScreen(
            Content,
            GraphicsDevice);
        SetScreen(menuScreen);
    }

    private static void InitialiseGameConstants()
    {
        var numberOfFacingDirections = FacingDirection.NumberOfItems;
        var numberOfOrientations = Orientation.NumberOfItems;
        var numberOfActions = LemmingAction.NumberOfItems;
        var numberOfSkills = LemmingSkill.NumberOfItems;
        var numberOfTeams = Team.NumberOfItems;
        var numberOfGadgetBehaviours = GadgetBehaviour.NumberOfItems;

        Console.WriteLine(
            "Loaded {0} facing directions. Loaded {1} orientations. Loaded {2} skills. Loaded {3} actions. " +
            "Loaded {4} teams. Loaded {5} gadget types",
            numberOfFacingDirections,
            numberOfOrientations,
            numberOfSkills,
            numberOfActions,
            numberOfTeams,
            numberOfGadgetBehaviours);
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
            LengthMax<char>(action.LemmingActionName);
        }

        LengthMax<char>(LevelConstants.NeutralControlPanelString);
        LengthMax<char>(LevelConstants.ZombieControlPanelString);
        LengthMax<char>(LevelConstants.NeutralZombieControlPanelString);
        LengthMax<char>(LevelConstants.AthleteControlPanelString2Skills);
        LengthMax<char>(LevelConstants.AthleteControlPanelString3Skills);
        LengthMax<char>(LevelConstants.AthleteControlPanelString4Skills);
        LengthMax<char>(LevelConstants.AthleteControlPanelString5Skills);

        if (actualMaxActionNameLength != LevelConstants.LongestActionNameLength)
            throw new Exception($"Longest action name length is actually {actualMaxActionNameLength}! Update {nameof(LevelConstants.LongestActionNameLength)}!");

        return;

        void LengthMax<T>(ReadOnlySpan<T> span)
        {
            actualMaxActionNameLength = Math.Max(actualMaxActionNameLength, span.Length);
        }
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
        var oldIsFullscreen = _isFullscreen;

        if (_isBorderless)
        {
            _isBorderless = false;
        }
        else
        {
            _isFullscreen = !_isFullscreen;
        }

        ApplyFullscreenChange(oldIsFullscreen);
    }

    public void ToggleBorderless()
    {
        var oldIsFullscreen = _isFullscreen;

        _isBorderless = !_isBorderless;
        _isFullscreen = _isBorderless;

        ApplyFullscreenChange(oldIsFullscreen);
    }

    private void ApplyFullscreenChange(bool oldIsFullscreen)
    {
        if (_isFullscreen)
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
        _graphics.HardwareModeSwitch = !_isBorderless;
        _graphics.ApplyChanges();

        _screen?.OnWindowSizeChanged();
    }

    private void SetFullscreen()
    {
        _width = Window.ClientBounds.Width;
        _height = Window.ClientBounds.Height;

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.HardwareModeSwitch = !_isBorderless;

        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        _screen?.OnWindowSizeChanged();
    }

    private void UnsetFullscreen()
    {
        _graphics.PreferredBackBufferWidth = _width;
        _graphics.PreferredBackBufferHeight = _height;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();

        _screen?.OnWindowSizeChanged();
    }

    public void Escape()
    {
        Exit();
    }
}