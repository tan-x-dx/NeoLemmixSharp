using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using NeoLemmixSharp.Engine.LevelBuilding;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;
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
		// create and init the UI manager
		UserInterface.Initialize(Content, BuiltinThemes.editor);
		UserInterface.Active.UseRenderTarget = true;

		// draw cursor outside the render target
		UserInterface.Active.IncludeCursorInRenderTarget = false;

		RootDirectoryManager.Initialise();
		FontBank.Initialise(Content);
		MenuSpriteBank.Initialise(Content, GraphicsDevice);

		_spriteBatch = new SpriteBatch(GraphicsDevice);

		TerrainMasks.InitialiseTerrainMasks(Content, GraphicsDevice);
		DefaultLemmingSpriteBank.CreateDefaultLemmingSpriteBank(Content, GraphicsDevice);

		using (var stream = File.Open("particles.dat", FileMode.Open))
		{
			Span<byte> byteBuffer = stackalloc byte[ParticleHelper.ByteLength];
			using var reader = new BinaryReader(stream, Encoding.UTF8, false);
			_ = reader.Read(byteBuffer);
			ParticleHelper.InitialiseParticleOffsets(byteBuffer);
		}

		//LoadLevel_Debug();
		var menuScreen = new MenuScreen(
			Content,
			GraphicsDevice,
			_spriteBatch);
		SetScreen(menuScreen);
		menuScreen.Initialise();
	}

	private static void InitialiseGameConstants()
	{
		var numberOfFacingDirections = FacingDirection.NumberOfItems;
		var numberOfOrientations = Orientation.NumberOfItems;
		var numberOfActions = LemmingAction.NumberOfItems;
		var numberOfSkills = LemmingSkill.NumberOfItems;
		var numberOfTeams = Team.NumberOfItems;
		var numberOfGadgetTypes = GadgetSubType.NumberOfItems;

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

	private void LoadLevel_Debug()
	{
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

		var path = Path.Combine(RootDirectoryManager.RootDirectory, file);

		var fileExtension = Path.GetExtension(file);
		var levelReader = LevelFileTypeHandler.GetLevelReaderForFileExtension(fileExtension);

		using var levelBuilder = new LevelBuilder(Content, GraphicsDevice, _spriteBatch, levelReader);
		SetScreen(levelBuilder.BuildLevel(path));
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

		GraphicsDevice.Clear(Color.Black);

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