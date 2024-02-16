using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
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
using NeoLemmixSharp.Menu.Rendering;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoLemmixSharp;

public sealed partial class NeoLemmixGame : Game, IGameWindow
{
    private readonly GraphicsDeviceManager _graphics;

    private SpriteBatch _spriteBatch;
    private IBaseScreen? _screen;
    private IScreenRenderer? _screenRenderer;

    private bool _isBorderless;
    public bool IsFullScreen { get; private set; }

    public int WindowWidth => _graphics.PreferredBackBufferWidth;
    public int WindowHeight => _graphics.PreferredBackBufferHeight;

    public NeoLemmixGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
        };

        Content.RootDirectory = "Content";
        Window.AllowUserResizing = false;
        Window.IsBorderless = true;
        IsMouseVisible = false;

        Window.ClientSizeChanged += WindowOnClientSizeChanged;

        IsFixedTimeStep = true;
        TargetElapsedTime = EngineConstants.FramesPerSecondTimeSpan;

        IGameWindow.Instance = this;
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
        LoadContent();
    }

    protected override void LoadContent()
    {
        LoadResources();

        // create and init the UI manager
        UserInterface.Initialize(Content, BuiltinThemes.editor);
        UserInterface.Active.UseRenderTarget = true;

        // draw cursor outside the render target
        UserInterface.Active.IncludeCursorInRenderTarget = false;

        RootDirectoryManager.Initialise();
        FontBank.Initialise(Content);
        MenuSpriteBank.Initialise(Content, GraphicsDevice);
        new CommonSpriteBankBuilder(GraphicsDevice, Content).BuildCommonSpriteBank();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        TerrainMasks.InitialiseTerrainMasks(Content, GraphicsDevice);
        DefaultLemmingSpriteBank.CreateDefaultLemmingSpriteBank(Content, GraphicsDevice);

        var menuScreen = new MenuScreen(
            Content,
            GraphicsDevice,
            _spriteBatch);
        SetScreen(menuScreen);
        menuScreen.Initialise();
    }

    private void LoadResources()
    {
        var assembly = GetType().Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        LoadParticleResources();

        return;

        void LoadParticleResources()
        {
            var particleResourceName = Array.Find(resourceNames, s => s.EndsWith("particles.dat"));

            if (string.IsNullOrWhiteSpace(particleResourceName))
                throw new InvalidOperationException("Could not load particles.dat!");

            using var stream = assembly.GetManifestResourceStream(particleResourceName)!;
            var byteBuffer = ParticleHelper.GetByteBuffer();
            using var reader = new BinaryReader(stream, Encoding.UTF8, false);
            _ = reader.Read(byteBuffer);
        }
    }

    private static void InitialiseGameConstants()
    {
        var numberOfFacingDirections = FacingDirection.NumberOfItems;
        var numberOfOrientations = Orientation.NumberOfItems;
        var numberOfActions = LemmingAction.NumberOfItems;
        var numberOfSkills = LemmingSkill.NumberOfItems;
        var numberOfTeams = Team.NumberOfItems;
        var numberOfGadgetTypes = GadgetBehaviour.NumberOfItems;

        Console.WriteLine(
            "Loaded {0} facing directions. Loaded {1} orientations. Loaded {2} skills. Loaded {3} actions. " +
            "Loaded {4} teams. Loaded {5} gadget types",
            numberOfFacingDirections,
            numberOfOrientations,
            numberOfSkills,
            numberOfActions,
            numberOfTeams,
            numberOfGadgetTypes);

        OhNoerAction.Initialise();
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
    }

    protected override void Update(GameTime gameTime)
    {
        _screen!.Tick(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // if (gameTime.IsRunningSlowly)
        //     return;

        _screenRenderer!.RenderScreen(_spriteBatch);
    }

    public void ToggleFullScreen()
    {
        var oldIsFullscreen = IsFullScreen;

        if (_isBorderless)
        {
            _isBorderless = false;
        }
        else
        {
            IsFullScreen = !IsFullScreen;
        }

        ApplyFullscreenChange(oldIsFullscreen);
    }

    public void ToggleBorderless()
    {
        var oldIsFullscreen = IsFullScreen;

        _isBorderless = !_isBorderless;
        IsFullScreen = _isBorderless;

        ApplyFullscreenChange(oldIsFullscreen);
    }

    private void ApplyFullscreenChange(bool oldIsFullscreen)
    {
        if (IsFullScreen)
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
    }

    private void SetFullscreen()
    {
        /*   _gameResolution = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

           _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
           _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
           _graphics.HardwareModeSwitch = !_isBorderless;

           _graphics.IsFullScreen = true;
           _graphics.ApplyChanges();
           CaptureCursor();*/
    }

    private void UnsetFullscreen()
    {
        /* _graphics.PreferredBackBufferWidth = _gameResolution.X;
         _graphics.PreferredBackBufferHeight = _gameResolution.Y;
         _graphics.IsFullScreen = false;
         _graphics.ApplyChanges();
         CaptureCursor();*/
    }

    public void Escape()
    {
        Exit();
    }
}