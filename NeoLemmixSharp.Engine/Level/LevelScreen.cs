using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Rewind;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Terrain;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.Level.Updates;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level;

public sealed class LevelScreen : IBaseScreen
{
    private static BoundaryBehaviour _horizontalBoundaryBehaviour = null!;
    private static BoundaryBehaviour _verticalBoundaryBehaviour = null!;

    private static LevelParameterSet _levelParameters = null!;
    private static TerrainManager _terrainManager = null!;
    private static TerrainPainter _terrainPainter = null!;
    private static LemmingManager _lemmingManager = null!;
    private static GadgetManager _gadgetManager = null!;
    private static SkillSetManager _skillSetManager = null!;
    private static LevelControlPanel _levelControlPanel = null!;
    private static UpdateScheduler _updateScheduler = null!;
    private static LevelCursor _levelCursor = null!;
    private static LevelTimer _levelTimer = null!;
    private static LevelInputController _levelInputController = null!;
    private static Viewport _levelViewport = null!;
    private static RewindManager _rewindManager = null!;
    private static LevelScreenRenderer _levelScreenRenderer = null!;

    public static BoundaryBehaviour HorizontalBoundaryBehaviour => _horizontalBoundaryBehaviour;
    public static BoundaryBehaviour VerticalBoundaryBehaviour => _verticalBoundaryBehaviour;

    public static LevelParameterSet LevelParameters => _levelParameters;
    public static TerrainManager TerrainManager => _terrainManager;
    public static TerrainPainter TerrainPainter => _terrainPainter;
    public static LemmingManager LemmingManager => _lemmingManager;
    public static GadgetManager GadgetManager => _gadgetManager;
    public static SkillSetManager SkillSetManager => _skillSetManager;
    public static LevelControlPanel LevelControlPanel => _levelControlPanel;
    public static UpdateScheduler UpdateScheduler => _updateScheduler;
    public static LevelCursor LevelCursor => _levelCursor;
    public static LevelTimer LevelTimer => _levelTimer;
    public static LevelInputController LevelInputController => _levelInputController;
    public static RewindManager RewindManager => _rewindManager;
    public static Viewport LevelViewport => _levelViewport;

    public static void SetHorizontalBoundaryBehaviour(BoundaryBehaviour horizontalBoundaryBehaviour)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
    }

    public static void SetVerticalBoundaryBehaviour(BoundaryBehaviour verticalBoundaryBehaviour)
    {
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
    }

    public static void SetLevelParameters(LevelParameterSet levelParameters)
    {
        _levelParameters = levelParameters;
    }

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        _terrainManager = terrainManager;
    }

    public static void SetTerrainPainter(TerrainPainter terrainPainter)
    {
        _terrainPainter = terrainPainter;
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        _lemmingManager = lemmingManager;
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
        _gadgetManager = gadgetManager;
    }

    public static void SetSkillSetManager(SkillSetManager skillSetManager)
    {
        _skillSetManager = skillSetManager;
    }

    public static void SetLevelControlPanel(LevelControlPanel levelControlPanel)
    {
        _levelControlPanel = levelControlPanel;
    }

    public static void SetUpdateScheduler(UpdateScheduler updateScheduler)
    {
        _updateScheduler = updateScheduler;
    }

    public static void SetLevelCursor(LevelCursor levelCursor)
    {
        _levelCursor = levelCursor;
    }

    public static void SetLevelTimer(LevelTimer levelTimer)
    {
        _levelTimer = levelTimer;
    }

    public static void SetLevelInputController(LevelInputController levelInputController)
    {
        _levelInputController = levelInputController;
    }

    public static void SetRewindManager(RewindManager rewindManager)
    {
        _rewindManager = rewindManager;
    }

    public static void SetViewport(Viewport levelViewport)
    {
        _levelViewport = levelViewport;
    }

    public static void SetLevelScreenRenderer(LevelScreenRenderer levelScreenRenderer)
    {
        _levelScreenRenderer = levelScreenRenderer;
    }

    public static int LevelWidth => HorizontalBoundaryBehaviour.LevelLength;
    public static int LevelHeight => VerticalBoundaryBehaviour.LevelLength;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static LevelPosition NormalisePosition(LevelPosition levelPosition)
    {
        return new LevelPosition(
            HorizontalBoundaryBehaviour.Normalise(levelPosition.X),
            VerticalBoundaryBehaviour.Normalise(levelPosition.Y));
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool PositionOutOfBounds(LevelPosition levelPosition)
    {
        return levelPosition.X < 0 ||
               levelPosition.X >= HorizontalBoundaryBehaviour.LevelLength ||
               levelPosition.Y < 0 ||
               levelPosition.Y >= VerticalBoundaryBehaviour.LevelLength;
    }

    public IScreenRenderer ScreenRenderer => _levelScreenRenderer;
    public string ScreenTitle { get; }
    public bool IsDisposed { get; private set; }

    public LevelScreen(LevelData levelData)
    {
        ScreenTitle = levelData.LevelTitle;

        ValidateAndInitialise();
    }

    private static void ValidateAndInitialise()
    {
        var staticFields = typeof(LevelScreen)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var staticField in staticFields)
        {
            var actualObject = staticField.GetValue(null);

            if (actualObject is null)
                throw new InvalidOperationException($"Static field has not been initialised: {staticField.Name}");

            if (actualObject is IInitialisable objectToInitialise)
            {
                objectToInitialise.Initialise();
            }
        }
    }

    public void Tick(GameTime gameTime)
    {
        if (!IGameWindow.Instance.IsActive)
            return;

        UpdateScheduler.Tick();
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        LevelControlPanel.SetWindowDimensions(windowWidth, windowHeight);
        LevelViewport.SetWindowDimensions(windowWidth, windowHeight, LevelControlPanel.ScreenHeight);
        ScreenRenderer.OnWindowSizeChanged();

        IGameWindow.Instance.CaptureCursor();
    }

    public void OnActivated()
    {
        IGameWindow.Instance.CaptureCursor();
    }

    public void OnSetScreen()
    {
        IGameWindow.Instance.CaptureCursor();
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        var staticFields = typeof(LevelScreen)
            .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var staticField in staticFields)
        {
            var actualObject = staticField.GetValue(null);

            if (actualObject is IDisposable disposable)
            {
                disposable.Dispose();
            }

            staticField.SetValue(null, null);
        }

        IsDisposed = true;
    }
}