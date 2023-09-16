﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public abstract class LemmingAction : IExtendedEnumType<LemmingAction>
{
    private static readonly LemmingAction[] LemmingActions = RegisterAllLemmingActions();
    protected static TerrainManager TerrainManager { get; private set; } = null!;
    protected static LemmingManager LemmingManager { get; private set; } = null!;
    protected static GadgetManager GadgetManager { get; private set; } = null!;

    public static int NumberOfItems => LemmingActions.Length;
    public static ReadOnlySpan<LemmingAction> AllItems => new(LemmingActions);

    private static LemmingAction[] RegisterAllLemmingActions()
    {
        // NOTE: DO NOT ADD THE NONE ACTION
        var result = new LemmingAction[]
        {
            WalkerAction.Instance,
            ClimberAction.Instance,
            FloaterAction.Instance,
            BlockerAction.Instance,
            BuilderAction.Instance,
            BasherAction.Instance,
            MinerAction.Instance,
            DiggerAction.Instance,

            PlatformerAction.Instance,
            StackerAction.Instance,
            FencerAction.Instance,
            GliderAction.Instance,
            JumperAction.Instance,
            SwimmerAction.Instance,
            ShimmierAction.Instance,
            LasererAction.Instance,
            SliderAction.Instance,

            FallerAction.Instance,
            AscenderAction.Instance,
            ShruggerAction.Instance,
            DrownerAction.Instance,
            HoisterAction.Instance,
            DehoisterAction.Instance,
            ReacherAction.Instance,
            DisarmerAction.Instance,

            ExiterAction.Instance,
            ExploderAction.Instance,
            OhNoerAction.Instance,
            SplatterAction.Instance,
            StonerAction.Instance,
            VaporiserAction.Instance
        };

        result.ValidateUniqueIds();
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    public static void SetTerrainManager(TerrainManager terrainManager)
    {
        TerrainManager = terrainManager;
    }

    public static void SetLemmingManager(LemmingManager lemmingManager)
    {
        LemmingManager = lemmingManager;
    }

    public static void SetGadgetManager(GadgetManager gadgetManager)
    {
        GadgetManager = gadgetManager;
    }

    public abstract int Id { get; }
    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }
    public abstract int CursorSelectionPriorityValue { get; }

    public abstract bool UpdateLemming(Lemming lemming);

    public LevelPositionPair GetLemmingBounds(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dxCorrection = 1 - lemming.FacingDirection.Id; // Fixes off-by-one errors with left/right positions
        var lemmingPosition = lemming.LevelPosition;
        var animationFrame = lemming.AnimationFrame;

        var topLeftDx = TopLeftBoundsDeltaX(animationFrame);
        var topLeftDy = TopLeftBoundsDeltaY(animationFrame);

        var bottomRightDx = BottomRightBoundsDeltaX(animationFrame);
        var bottomRightDy = BottomRightBoundsDeltaY(animationFrame);

        var p1 = orientation.MoveWithoutNormalization(lemmingPosition, dxCorrection + dx * topLeftDx, topLeftDy);
        var p2 = orientation.MoveWithoutNormalization(lemmingPosition, dxCorrection + dx * bottomRightDx, bottomRightDy);

        return new LevelPositionPair(p1, p2);
    }

    protected abstract int TopLeftBoundsDeltaX(int animationFrame);
    protected abstract int TopLeftBoundsDeltaY(int animationFrame);

    protected abstract int BottomRightBoundsDeltaX(int animationFrame);
    protected virtual int BottomRightBoundsDeltaY(int animationFrame) => -1;

    public bool Equals(LemmingAction? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => LemmingActionName;

    public static bool operator ==(LemmingAction left, LemmingAction right) => left.Id == right.Id;
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.Id != right.Id;

    public virtual void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
        if (turnAround)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection());
        }

        if (lemming.CurrentAction == this)
            return;

        lemming.SetCurrentAction(this);
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;
    }

    /// <summary>
    /// Find the new ground pixel. 
    /// If result = 4, then at least 4 pixels are air below levelPosition. 
    /// If result = -7, then at least 7 pixels are terrain above levelPosition
    /// </summary>
    protected static int FindGroundPixel(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        var result = 0;
        if (TerrainManager.PixelIsSolidToLemming(lemming, levelPosition))
        {
            while (TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(levelPosition, 1 - result)) &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!TerrainManager.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveDown(levelPosition, result)) &&
               result < 4)
        {
            result++;
        }

        return result;
    }
}