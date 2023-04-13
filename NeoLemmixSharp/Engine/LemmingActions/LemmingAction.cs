using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.LemmingActions;

public abstract class LemmingAction : IEquatable<LemmingAction>
{
    public static ReadOnlyDictionary<string, LemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, LemmingAction> RegisterAllLemmingActions()
    {
        var ids = new ArrayBasedBitArray(50);

        var result = new Dictionary<string, LemmingAction>();

        RegisterLemmingAction(ids, result, AscenderAction.Instance);
        RegisterLemmingAction(ids, result, BasherAction.Instance);
        RegisterLemmingAction(ids, result, BlockerAction.Instance);
        RegisterLemmingAction(ids, result, BuilderAction.Instance);
        RegisterLemmingAction(ids, result, ClimberAction.Instance);
        RegisterLemmingAction(ids, result, DehoisterAction.Instance);
        RegisterLemmingAction(ids, result, DiggerAction.Instance);
        RegisterLemmingAction(ids, result, DisarmerAction.Instance);
        RegisterLemmingAction(ids, result, DrownerAction.Instance);
        RegisterLemmingAction(ids, result, ExiterAction.Instance);
        RegisterLemmingAction(ids, result, ExploderAction.Instance);
        RegisterLemmingAction(ids, result, FencerAction.Instance);
        RegisterLemmingAction(ids, result, FallerAction.Instance);
        RegisterLemmingAction(ids, result, FloaterAction.Instance);
        RegisterLemmingAction(ids, result, GliderAction.Instance);
        RegisterLemmingAction(ids, result, HoisterAction.Instance);
        RegisterLemmingAction(ids, result, JumperAction.Instance);
        RegisterLemmingAction(ids, result, LasererAction.Instance);
        RegisterLemmingAction(ids, result, MinerAction.Instance);
        RegisterLemmingAction(ids, result, OhNoerAction.Instance);
        RegisterLemmingAction(ids, result, PlatformerAction.Instance);
        RegisterLemmingAction(ids, result, ReacherAction.Instance);
        RegisterLemmingAction(ids, result, ShimmierAction.Instance);
        RegisterLemmingAction(ids, result, ShruggerAction.Instance);
        RegisterLemmingAction(ids, result, SliderAction.Instance);
        RegisterLemmingAction(ids, result, SplatterAction.Instance);
        RegisterLemmingAction(ids, result, StackerAction.Instance);
        RegisterLemmingAction(ids, result, StonerAction.Instance);
        RegisterLemmingAction(ids, result, SwimmerAction.Instance);
        RegisterLemmingAction(ids, result, VaporiserAction.Instance);
        RegisterLemmingAction(ids, result, WalkerAction.Instance);

        return new ReadOnlyDictionary<string, LemmingAction>(result);
    }

    private static void RegisterLemmingAction(IBitArray ids, IDictionary<string, LemmingAction> dict, LemmingAction lemmingAction)
    {
        dict.Add(lemmingAction.LemmingActionName, lemmingAction);

        if (!ids.SetBit(lemmingAction.ActionId))
        {
            throw new Exception($"Duplicated ID: {lemmingAction.LemmingActionName}");
        }
    }

    public static ICollection<LemmingAction> AllLemmingActions => LemmingActions.Values;

    protected static PixelManager Terrain => LevelScreen.CurrentLevel.Terrain;

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    protected abstract int ActionId { get; }
    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }

    public abstract bool UpdateLemming(Lemming lemming);

    public bool Equals(LemmingAction? other) => ActionId == (other?.ActionId ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && Equals(other);
    public sealed override int GetHashCode() => ActionId;

    public static bool operator ==(LemmingAction left, LemmingAction right) => left.ActionId == right.ActionId;
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.ActionId != right.ActionId;

    public virtual void TransitionLemmingToAction(
        Lemming lemming,
        bool turnAround)
    {
        if (turnAround)
        {
            lemming.FacingDirection = lemming.FacingDirection.OppositeDirection;
        }

        if (lemming.CurrentAction == this)
            return;

        lemming.CurrentAction = this;
        lemming.AnimationFrame = 0;
        lemming.EndOfAnimation = false;
        lemming.NumberOfBricksLeft = 0;
        lemming.IsStartingAction = true;
        lemming.InitialFall = false;
    }

    protected static void LayBrick(Lemming lemming)
    {
        var dx = lemming.FacingDirection.DeltaX;
        var brickDelta = lemming.CurrentAction == BuilderAction.Instance
            ? 1
            : 0;

        var brickPosition = lemming.Orientation.MoveUp(lemming.LevelPosition, brickDelta);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);
    }

    protected static int FindGroundPixel(
        IOrientation orientation,
        Point levelPosition)
    {
        // Find the new ground pixel
        // If Result = 4, then at least 4 pixels are air below (X, Y)
        // If Result = -7, then at least 7 pixels are terrain above (X, Y)
        var result = 0;
        if (Terrain.GetPixelData(levelPosition).IsSolid)
        {
            while (Terrain.GetPixelData(orientation.MoveUp(levelPosition, 1 - result)).IsSolid &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!Terrain.GetPixelData(orientation.MoveDown(levelPosition, result)).IsSolid &&
               result < 4)
        {
            result++;
        }

        return result;
    }

    protected static bool LemmingCanDehoist(Lemming lemming, bool alreadyMoved)
    {
        var currentPosition = lemming.LevelPosition;
        var nextPosition = currentPosition;
        var dx = lemming.FacingDirection.DeltaX;
        if (alreadyMoved)
        {
            currentPosition = lemming.Orientation.MoveLeft(currentPosition, dx);
        }
        else
        {
            nextPosition = lemming.Orientation.MoveRight(nextPosition, dx);
        }

        if (Terrain.PositionOutOfBounds(nextPosition) ||
            (!Terrain.GetPixelData(currentPosition).IsSolid ||
             Terrain.GetPixelData(nextPosition).IsSolid))
            return false;

        if (Terrain.GetPixelData(lemming.Orientation.MoveDown(nextPosition, 1)).IsSolid)
            return false;
        if (!Terrain.GetPixelData(lemming.Orientation.MoveDown(currentPosition, 1)).IsSolid)
            return true;

        if (Terrain.GetPixelData(lemming.Orientation.MoveDown(nextPosition, 2)).IsSolid)
            return false;
        if (!Terrain.GetPixelData(lemming.Orientation.MoveDown(currentPosition, 2)).IsSolid)
            return true;

        if (Terrain.GetPixelData(lemming.Orientation.MoveDown(nextPosition, 3)).IsSolid)
            return false;
        if (!Terrain.GetPixelData(lemming.Orientation.MoveDown(currentPosition, 3)).IsSolid)
            return true;

        if (Terrain.GetPixelData(lemming.Orientation.MoveDown(nextPosition, 4)).IsSolid)
            return false;
        if (!Terrain.GetPixelData(lemming.Orientation.MoveDown(currentPosition, 4)).IsSolid)
            return true;

        return true;
    }
}