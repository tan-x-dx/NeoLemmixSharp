﻿using Microsoft.Xna.Framework;
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
    private static LevelScreen _instance = null!;

    private readonly BoundaryBehaviour _horizontalBoundaryBehaviour;
    private readonly BoundaryBehaviour _verticalBoundaryBehaviour;

    private readonly LevelParameterSet _levelParameters;
    private readonly TerrainManager _terrainManager;
    private readonly TerrainPainter _terrainPainter;
    private readonly LemmingManager _lemmingManager;
    private readonly GadgetManager _gadgetManager;
    private readonly SkillSetManager _skillSetManager;
    private readonly LevelControlPanel _levelControlPanel;
    private readonly UpdateScheduler _updateScheduler;
    private readonly LevelCursor _levelCursor;
    private readonly LevelTimer _levelTimer;
    private readonly LevelInputController _levelInputController;
    private readonly Viewport _levelViewport;
    private readonly RewindManager _rewindManager;
    private readonly LevelScreenRenderer _levelScreenRenderer;

    public static BoundaryBehaviour HorizontalBoundaryBehaviour => _instance._horizontalBoundaryBehaviour;
    public static BoundaryBehaviour VerticalBoundaryBehaviour => _instance._verticalBoundaryBehaviour;

    public static LevelParameterSet LevelParameters => _instance._levelParameters;
    public static TerrainManager TerrainManager => _instance._terrainManager;
    public static TerrainPainter TerrainPainter => _instance._terrainPainter;
    public static LemmingManager LemmingManager => _instance._lemmingManager;
    public static GadgetManager GadgetManager => _instance._gadgetManager;
    public static SkillSetManager SkillSetManager => _instance._skillSetManager;
    public static LevelControlPanel LevelControlPanel => _instance._levelControlPanel;
    public static UpdateScheduler UpdateScheduler => _instance._updateScheduler;
    public static LevelCursor LevelCursor => _instance._levelCursor;
    public static LevelTimer LevelTimer => _instance._levelTimer;
    public static LevelInputController LevelInputController => _instance._levelInputController;
    public static RewindManager RewindManager => _instance._rewindManager;
    public static Viewport LevelViewport => _instance._levelViewport;

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
        return (uint)levelPosition.X >= (uint)HorizontalBoundaryBehaviour.LevelLength ||
               (uint)levelPosition.Y >= (uint)VerticalBoundaryBehaviour.LevelLength;
    }

    public IScreenRenderer ScreenRenderer => _levelScreenRenderer;
    public string ScreenTitle { get; }

    public LevelScreen(
        LevelData levelData,
        BoundaryBehaviour horizontalBoundaryBehaviour,
        BoundaryBehaviour verticalBoundaryBehaviour,
        LevelParameterSet levelParameters,
        TerrainManager terrainManager,
        TerrainPainter terrainPainter,
        LemmingManager lemmingManager,
        GadgetManager gadgetManager,
        SkillSetManager skillSetManager,
        LevelControlPanel levelControlPanel,
        UpdateScheduler updateScheduler,
        LevelCursor levelCursor,
        LevelTimer levelTimer,
        LevelInputController levelInputController,
        Viewport levelViewport,
        RewindManager rewindManager,
        LevelScreenRenderer levelScreenRenderer)
    {
        _horizontalBoundaryBehaviour = horizontalBoundaryBehaviour;
        _verticalBoundaryBehaviour = verticalBoundaryBehaviour;
        _levelParameters = levelParameters;
        _terrainManager = terrainManager;
        _terrainPainter = terrainPainter;
        _lemmingManager = lemmingManager;
        _gadgetManager = gadgetManager;
        _skillSetManager = skillSetManager;
        _levelControlPanel = levelControlPanel;
        _updateScheduler = updateScheduler;
        _levelCursor = levelCursor;
        _levelTimer = levelTimer;
        _levelInputController = levelInputController;
        _levelViewport = levelViewport;
        _rewindManager = rewindManager;
        _levelScreenRenderer = levelScreenRenderer;
        ScreenTitle = levelData.LevelTitle;

        _instance = this;
    }

    public void Initialise()
    {
        var fields = typeof(LevelScreen)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var actualObject = field.GetValue(this);

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
        var fields = typeof(LevelScreen)
            .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var actualObject = field.GetValue(this);

            if (actualObject is IDisposable disposable)
            {
                disposable.Dispose();
            }

            field.SetValue(this, null);
        }

        _instance = null!;
    }
}