﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Terrain.Masks;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public abstract class LemmingAction : IExtendedEnumType<LemmingAction>
{
    private static readonly LemmingAction[] LemmingActions = RegisterAllLemmingActions();

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

        IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<LemmingAction>(result));
        Array.Sort(result, IdEquatableItemHelperMethods.Compare);

        return result;
    }

    public readonly int Id;
    public readonly string LemmingActionName;
    public readonly int NumberOfAnimationFrames;
    public readonly int MaxPhysicsFrames;
    public readonly int CursorSelectionPriorityValue;
    public readonly bool IsOneTimeAction;
    public readonly bool IsAirborneAction;

    protected LemmingAction(
        int id,
        string lemmingActionName,
        int numberOfAnimationFrames,
        int maxPhysicsFrames,
        int cursorSelectionPriorityValue,
        bool isOneTimeAction,
        bool isAirborneAction)
    {
        Id = id;
        LemmingActionName = lemmingActionName;
        NumberOfAnimationFrames = numberOfAnimationFrames;
        MaxPhysicsFrames = maxPhysicsFrames;
        CursorSelectionPriorityValue = cursorSelectionPriorityValue;
        IsOneTimeAction = isOneTimeAction;
        IsAirborneAction = isAirborneAction;
    }

    public abstract bool UpdateLemming(Lemming lemming);

    public LevelPositionPair GetLemmingBounds(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dxCorrection = 1 - lemming.FacingDirection.Id; // Fixes off-by-one errors with left/right positions
        var lemmingPosition = lemming.LevelPosition;
        var physicsFrame = lemming.PhysicsFrame;

        var topLeftDx = TopLeftBoundsDeltaX(physicsFrame);
        var topLeftDy = TopLeftBoundsDeltaY(physicsFrame);

        var bottomRightDx = BottomRightBoundsDeltaX(physicsFrame);
        var bottomRightDy = BottomRightBoundsDeltaY(physicsFrame);

        var p1 = orientation.MoveWithoutNormalization(lemmingPosition, dxCorrection + dx * topLeftDx, topLeftDy);
        var p2 = orientation.MoveWithoutNormalization(lemmingPosition, dxCorrection + dx * bottomRightDx, bottomRightDy);

        return new LevelPositionPair(p1, p2);
    }

    protected abstract int TopLeftBoundsDeltaX(int animationFrame);
    protected abstract int TopLeftBoundsDeltaY(int animationFrame);

    protected abstract int BottomRightBoundsDeltaX(int animationFrame);
    protected virtual int BottomRightBoundsDeltaY(int animationFrame) => -1;

    public virtual LevelPosition GetFootPosition(
        Lemming lemming,
        LevelPosition anchorPosition)
    {
        return lemming.Orientation.MoveUp(anchorPosition, 1);
    }

    public virtual void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
        if (turnAround)
        {
            lemming.SetFacingDirection(lemming.FacingDirection.GetOpposite());
        }

        if (lemming.CurrentAction == this)
            return;

        lemming.SetCurrentAction(this);
        lemming.PhysicsFrame = 0;
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;
    }

    /// <summary>
    /// Find the new ground pixel. 
    /// If result = -4, then at least 4 pixels are air below levelPosition. 
    /// If result = 7, then at least 7 pixels are terrain above levelPosition
    /// </summary>
    [Pure]
    [SkipLocalsInit]
    protected static int FindGroundPixel(
        Lemming lemming,
        LevelPosition levelPosition)
    {
        var orientation = lemming.Orientation;

        // Subroutine of other LevelAction methods.
        // Use a dummy scratch space span to prevent data from being overridden.
        // Prevents weird bugs!
        Span<uint> scratchSpace = stackalloc uint[LevelScreen.GadgetManager.ScratchSpaceSize];

        LevelScreen.GadgetManager.Foo = true;

        var gadgetTestRegion = new LevelPositionPair(
            orientation.MoveUp(levelPosition, LevelConstants.MaxStepUp + 1),
            orientation.MoveDown(levelPosition, LevelConstants.DefaultFallStep + 1));
        var gadgetsNearRegion = LevelScreen.GadgetManager.GetAllItemsNearRegion(scratchSpace, gadgetTestRegion);

        LevelScreen.GadgetManager.Foo = false;

        int result;
        if (PositionIsSolidToLemming(gadgetsNearRegion, lemming, levelPosition))
        {
            result = 0;
            while (PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(levelPosition, 1 + result)) &&
                   result < LevelConstants.MaxStepUp + 1)
            {
                result++;
            }

            return result;
        }

        result = -1;
        // MoveUp, but step is negative, therefore moves down
        while (!PositionIsSolidToLemming(gadgetsNearRegion, lemming, orientation.MoveUp(levelPosition, result)) &&
               result > -(LevelConstants.DefaultFallStep + 1))
        {
            result--;
        }

        return result;
    }

    public static bool PositionIsSolidToLemming(
        in GadgetSet gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, levelPosition) ||
               (gadgets.Count > 0 && HasSolidGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    public static bool PositionIsIndestructibleToLemming(
        in GadgetSet gadgets,
        Lemming lemming,
        IDestructionMask destructionMask,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsIndestructibleToLemming(lemming, destructionMask, levelPosition) ||
               (gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    protected static bool PositionIsSteelToLemming(
        in GadgetSet gadgets,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        return LevelScreen.TerrainManager.PixelIsSteel(levelPosition) ||
               (gadgets.Count > 0 && HasSteelGadgetAtPosition(in gadgets, lemming, levelPosition));
    }

    [Pure]
    private static bool HasSolidGadgetAtPosition(
        in GadgetSet enumerable,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        foreach (var gadget in enumerable)
        {
            if (gadget.IsSolidToLemmingAtPosition(lemming, levelPosition))
                return true;
        }

        return false;
    }

    [Pure]
    private static bool HasSteelGadgetAtPosition(
        in GadgetSet enumerable,
        Lemming lemming,
        LevelPosition levelPosition)
    {
        foreach (var gadget in enumerable)
        {
            if (gadget.IsSteelToLemmingAtPosition(lemming, levelPosition))
                return true;
        }

        return false;
    }

    int IIdEquatable<LemmingAction>.Id => Id;
    public bool Equals(LemmingAction? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && Id == other.Id;
    public sealed override int GetHashCode() => Id;
    public sealed override string ToString() => LemmingActionName;

    public static bool operator ==(LemmingAction left, LemmingAction right) => left.Id == right.Id;
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.Id != right.Id;
}