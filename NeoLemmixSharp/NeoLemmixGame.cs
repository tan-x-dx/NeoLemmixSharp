using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.LevelBuilding;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp;

public sealed class NeoLemmixGame : Game, IGameWindow
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly TimeSpan _standardGameUps;

    private bool _isBorderless;

    private FontBank _fontBank;
    private Point _gameResolution = new(960, 720);
    private SpriteBatch _spriteBatch;
    private RootDirectoryManager _rootDirectoryManager;

    public int WindowWidth => _graphics.PreferredBackBufferWidth;
    public int WindowHeight => _graphics.PreferredBackBufferHeight;

    public bool IsFullScreen { get; private set; }
    public IBaseScreen Screen { get; set; }
    public IScreenRenderer ScreenRenderer { get; set; }

    public NeoLemmixGame()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.AllowUserResizing = false;
        Window.IsBorderless = false;

        Window.ClientSizeChanged += WindowOnClientSizeChanged;

        _standardGameUps = TimeSpan.FromSeconds(1d / GameConstants.FramesPerSecond);

        IsFixedTimeStep = true;
        TargetElapsedTime = _standardGameUps;
    }

    private void WindowOnClientSizeChanged(object? sender, EventArgs e)
    {
        CaptureCursor();
        Screen.OnWindowSizeChanged();
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
        base.OnActivated(sender, args);

        CaptureCursor();
    }

    private void CaptureCursor()
    {
        var rect = Window.ClientBounds;
        rect.Width += rect.X;
        rect.Height += rect.Y;

        ClipCursor(ref rect);
    }

    [DllImport("user32.dll")]
    private static extern void ClipCursor(ref Rectangle rect);

    protected override void LoadContent()
    {
        _fontBank = new FontBank(Content);

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _graphics.PreferredBackBufferWidth = _gameResolution.X;
        _graphics.PreferredBackBufferHeight = _gameResolution.Y;

        _graphics.ApplyChanges();

        InitialiseGameConstants();

        var file =
        //    "levels\\tanxdx_TheTreacheryOfLemmings_R3V1.nxlv";
        //  "levels\\rotation test.nxlv";
        //  "levels\\render test.nxlv";
         "levels\\movement test.nxlv";
        // "levels\\object test.nxlv";
        // "levels\\Amiga Lemmings\\Oh No! More Lemmings\\Tame\\02_Rent-a-Lemming.nxlv";
        //   "levels\\Amiga Lemmings\\Oh No! More Lemmings\\Tame\\05_Snuggle_up_to_a_Lemming.nxlv";
        //  "levels\\Amiga Lemmings\\Lemmings\\Tricky\\05_Careless_clicking_costs_lives.nxlv";
        //   "levels\\LemRunner\\Industry\\TheNightShift.nxlv";
        //  "levels\\Amiga Lemmings\\Lemmings\\Tricky\\04_Here's_one_I_prepared_earlier.nxlv";
        //"levels\\IntegralLemmingsV5\\Alpha\\TheseLemmingsAndThoseLemmings.nxlv";
        //"levels\\CuttingItClose.nxlv";
        //    "levels\\scrollTest.nxlv";
        //  "levels\\LemRunner\\Mona\\ACaeloUsqueAdCentrum.nxlv";
        // "levels\\groupTest.nxlv";
        //    "levels\\eraseTest.nxlv";
        //  "levels\\Amiga Lemmings\\Lemmings\\Fun\\19_Take_good_care_of_my_Lemmings.nxlv";

        var path = Path.Combine(_rootDirectoryManager.RootDirectory, file);

        using (var levelBuilder = new LevelBuilder(Content, GraphicsDevice, _spriteBatch, _fontBank, _rootDirectoryManager))
        {
            Screen = levelBuilder.BuildLevel(path);
            Screen.GameWindow = this;
            Screen.OnWindowSizeChanged();
            ScreenRenderer = Screen.ScreenRenderer;
            ScreenRenderer.GameWindow = this;
        }

        Window.Title = Screen.ScreenTitle;

        CaptureCursor();
    }

    private void InitialiseGameConstants()
    {
        var numberOfFacingDirections = FacingDirection.NumberOfItems;
        var numberOfOrientations = Orientation.NumberOfItems;
        var numberOfActions = LemmingAction.NumberOfItems;
        var numberOfSkills = LemmingSkill.NumberOfItems;
        var numberOfTeams = Team.NumberOfItems;

        Console.WriteLine(
            "Loaded {0} facing directions. Loaded {1} orientations. Loaded {2} skills. Loaded {3} actions. " +
            "Loaded {4} teams.",
            numberOfFacingDirections,
            numberOfOrientations,
            numberOfSkills,
            numberOfActions,
            numberOfTeams);

        TerrainMasks.InitialiseTerrainMasks(Content, GraphicsDevice);
        DefaultLemmingSpriteBank.CreateDefaultLemmingSpriteBank(Content, GraphicsDevice);

        _rootDirectoryManager = new RootDirectoryManager();
    }

    protected override void Update(GameTime gameTime)
    {
        Screen.Tick();
    }

    protected override void Draw(GameTime gameTime)
    {
        if (gameTime.IsRunningSlowly)
            return;

        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

        ScreenRenderer.RenderScreen(_spriteBatch);

        _spriteBatch.End();
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
        CaptureCursor();
    }

    private void SetFullscreen()
    {
        _gameResolution = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);

        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.HardwareModeSwitch = !_isBorderless;

        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
        CaptureCursor();
    }

    private void UnsetFullscreen()
    {
        _graphics.PreferredBackBufferWidth = _gameResolution.X;
        _graphics.PreferredBackBufferHeight = _gameResolution.Y;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        CaptureCursor();
    }

    public void Escape()
    {
        Exit();
    }
}
