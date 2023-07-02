using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.LevelBuilding;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Level.Viewport.Lemming;
using NeoLemmixSharp.Rendering.Text;
using NeoLemmixSharp.Screen;
using System;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp;

public sealed class NeoLemmixGame : Game, IGameWindow
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly TimeSpan _standardGameUps = TimeSpan.FromSeconds(1d / 51d);

    private bool _isBorderless;

    private FontBank _fontBank;
    private Point _gameResolution = new(960, 720);
    private SpriteBatch _spriteBatch;

    public int WindowWidth => _graphics.PreferredBackBufferWidth;
    public int WindowHeight => _graphics.PreferredBackBufferHeight;

    public bool IsFullScreen { get; private set; }
    public BaseScreen Screen { get; set; }
    public ScreenRenderer ScreenRenderer { get; set; }

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

        DefaultLemmingSpriteBank.CreateDefaultLemmingSpriteBank(Content, GraphicsDevice);

        var path =
        //    "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\tanxdx_TheTreacheryOfLemmings_R3V1.nxlv";
        //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\rotation test.nxlv";
        //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\render test.nxlv";
        //"C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\movement test.nxlv";
        "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\object test.nxlv";
        // "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\Amiga Lemmings\\Oh No! More Lemmings\\Tame\\02_Rent-a-Lemming.nxlv";
        //   "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\Amiga Lemmings\\Oh No! More Lemmings\\Tame\\05_Snuggle_up_to_a_Lemming.nxlv";
        //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\Amiga Lemmings\\Lemmings\\Tricky\\05_Careless_clicking_costs_lives.nxlv";
        //   "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\LemRunner\\Industry\\TheNightShift.nxlv";
        //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\Amiga Lemmings\\Lemmings\\Tricky\\04_Here's_one_I_prepared_earlier.nxlv";
        //"C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\IntegralLemmingsV5\\Alpha\\TheseLemmingsAndThoseLemmings.nxlv";
        //"C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\CuttingItClose.nxlv";
        //    "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\scrollTest.nxlv";
        //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\LemRunner\\Mona\\ACaeloUsqueAdCentrum.nxlv";
        // "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\groupTest.nxlv";
        //    "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\eraseTest.nxlv";
        //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\Amiga Lemmings\\Lemmings\\Fun\\19_Take_good_care_of_my_Lemmings.nxlv";

        using (var levelBuilder = new LevelBuilder(Content, GraphicsDevice, _spriteBatch, _fontBank))
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
