using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.LevelBuilding;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Rendering.Text;
using NeoLemmixSharp.Screen;
using NeoLemmixSharp.Util;
using System;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp;

public sealed class NeoLemmixGame : Game, IGameWindow
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly TimeSpan _standardGameUps = TimeSpan.FromSeconds(1d / 17d);
    private readonly TimeSpan _fastForwardsGameUps = TimeSpan.FromSeconds(1d / 68d);

    private FontBank _fontBank;
    private Point _gameResolution = new(960, 720);
    private SpriteBatch _spriteBatch;
    private MenuFont _menuFont;

    public int WindowWidth => _graphics.PreferredBackBufferWidth;
    public int WindowHeight => _graphics.PreferredBackBufferHeight;
    public bool IsFullScreen => _graphics.IsFullScreen;
    public bool IsFastForwards { get; private set; }

    public BaseScreen Screen { get; set; }
    public ScreenRenderer ScreenRenderer { get; set; }

    [DllImport("user32.dll")]
    private static extern void ClipCursor(ref Rectangle rect);

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
        ScreenRenderer.OnWindowSizeChanged();
    }

    protected override void OnActivated(object sender, EventArgs args)
    {
        CaptureCursor();

        base.OnActivated(sender, args);
    }

    private void CaptureCursor()
    {
        var rect = Window.ClientBounds;
        rect.Width += rect.X;
        rect.Height += rect.Y;

        ClipCursor(ref rect);
    }

    protected override void LoadContent()
    {
        _fontBank = new FontBank(Content);

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _graphics.PreferredBackBufferWidth = _gameResolution.X;
        _graphics.PreferredBackBufferHeight = _gameResolution.Y;

        _graphics.ApplyChanges();

        var path =
                    //    "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\tanxdx_TheTreacheryOfLemmings_R3V1.nxlv";
                    //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\rotation test.nxlv";
                    //  "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\render test.nxlv";
                    "C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\movement test.nxlv";
        //"C:\\Users\\andre\\Documents\\NeoLemmix_v12.12.5\\levels\\object test.nxlv";
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
            ScreenRenderer = Screen.CreateScreenRenderer();
            ScreenRenderer.GameWindow = this;
            ScreenRenderer.OnWindowSizeChanged();
        }

        Window.Title = Screen.ScreenTitle;

        CaptureCursor();
    }

    protected override void Update(GameTime gameTime)
    {
        Screen.Tick();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(24, 24, 60));

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        ScreenRenderer.RenderScreen(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void ToggleFullScreen()
    {
        if (_graphics.IsFullScreen)
        {
            _graphics.PreferredBackBufferWidth = _gameResolution.X;
            _graphics.PreferredBackBufferHeight = _gameResolution.Y;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        _graphics.IsFullScreen = !_graphics.IsFullScreen;
        _graphics.ApplyChanges();
        CaptureCursor();
    }

    public void SetFastForwards(bool fastForwards)
    {
        if (fastForwards)
        {
            TargetElapsedTime = _fastForwardsGameUps;
            IsFastForwards = true;
        }
        else
        {
            TargetElapsedTime = _standardGameUps;
            IsFastForwards = false;
        }
    }

    public void Escape()
    {
        Exit();
    }
}
