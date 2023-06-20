using NeoLemmixSharp.Engine.Directions.Orientations;
using NeoLemmixSharp.Engine.LevelPixels;
using NeoLemmixSharp.Rendering.LevelRendering;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeoLemmixSharp.Engine.LemmingActions;

public abstract class LemmingAction : IEquatable<LemmingAction>
{
    protected static TerrainManager Terrain { get; private set; }

    public static ReadOnlyDictionary<string, LemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, LemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<string, LemmingAction>();

        RegisterLemmingAction(NoneAction.Instance);

        RegisterLemmingAction(AscenderAction.Instance);
        RegisterLemmingAction(BasherAction.Instance);
        RegisterLemmingAction(BlockerAction.Instance);
        RegisterLemmingAction(BuilderAction.Instance);
        RegisterLemmingAction(ClimberAction.Instance);
        RegisterLemmingAction(DehoisterAction.Instance);
        RegisterLemmingAction(DiggerAction.Instance);
        RegisterLemmingAction(DisarmerAction.Instance);
        RegisterLemmingAction(DrownerAction.Instance);
        RegisterLemmingAction(ExiterAction.Instance);
        RegisterLemmingAction(ExploderAction.Instance);
        RegisterLemmingAction(FencerAction.Instance);
        RegisterLemmingAction(FallerAction.Instance);
        RegisterLemmingAction(FloaterAction.Instance);
        RegisterLemmingAction(GliderAction.Instance);
        RegisterLemmingAction(HoisterAction.Instance);
        RegisterLemmingAction(JumperAction.Instance);
        RegisterLemmingAction(LasererAction.Instance);
        RegisterLemmingAction(MinerAction.Instance);
        RegisterLemmingAction(OhNoerAction.Instance);
        RegisterLemmingAction(PlatformerAction.Instance);
        RegisterLemmingAction(ReacherAction.Instance);
        RegisterLemmingAction(ShimmierAction.Instance);
        RegisterLemmingAction(ShruggerAction.Instance);
        RegisterLemmingAction(SliderAction.Instance);
        RegisterLemmingAction(SplatterAction.Instance);
        RegisterLemmingAction(StackerAction.Instance);
        RegisterLemmingAction(StonerAction.Instance);
        RegisterLemmingAction(SwimmerAction.Instance);
        RegisterLemmingAction(VaporiserAction.Instance);
        RegisterLemmingAction(WalkerAction.Instance);

        var numberOfUniqueIds = result
            .Values
            .Select(la => la.ActionId)
            .Distinct()
            .Count();

        if (numberOfUniqueIds != result.Count)
        {
            var ids = string.Join(',', result
                .Values
                .Select(la => la.ActionId)
                .OrderBy(i => i));

            throw new Exception($"Duplicated action ID: {ids}");
        }

        return new ReadOnlyDictionary<string, LemmingAction>(result);

        void RegisterLemmingAction(LemmingAction lemmingAction)
        {
            result.Add(lemmingAction.LemmingActionName, lemmingAction);
        }
    }

    public static ICollection<LemmingAction> AllLemmingActions => LemmingActions.Values;

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    public abstract int ActionId { get; }
    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }
    public abstract bool CanBeAssignedPermanentSkill { get; }

    public abstract bool UpdateLemming(Lemming lemming);

    public bool Equals(LemmingAction? other) => ActionId == (other?.ActionId ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && ActionId == other.ActionId;
    public sealed override int GetHashCode() => ActionId;
    public sealed override string ToString() => LemmingActionName;

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
        var dy = lemming.CurrentAction == BuilderAction.Instance
            ? 1
            : 0;

        var brickPosition = lemming.LevelPosition;
        brickPosition = lemming.Orientation.MoveUp(brickPosition, dy);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = lemming.Orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);
    }

    protected static int FindGroundPixel(
        Lemming lemming,
        Orientation orientation,
        in LevelPosition levelPosition)
    {
        // Find the new ground pixel
        // If Result = 4, then at least 4 pixels are air below (X, Y)
        // If Result = -7, then at least 7 pixels are terrain above (X, Y)
        var result = 0;
        if (Terrain.PixelIsSolidToLemming(levelPosition, lemming))
        {
            while (Terrain.PixelIsSolidToLemming(orientation.MoveUp(levelPosition, 1 - result), lemming) &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!Terrain.PixelIsSolidToLemming(orientation.MoveDown(levelPosition, result), lemming) &&
               result < 4)
        {
            result++;
        }

        return result;
    }

    protected static bool LemmingCanDehoist(Lemming lemming, bool alreadyMoved)
    {
        var dx = lemming.FacingDirection.DeltaX;
        LevelPosition currentPosition;
        LevelPosition nextPosition;
        if (alreadyMoved)
        {
            nextPosition = lemming.LevelPosition;
            currentPosition = lemming.Orientation.MoveLeft(nextPosition, dx);
        }
        else
        {
            currentPosition = lemming.LevelPosition;
            nextPosition = lemming.Orientation.MoveRight(currentPosition, dx);
        }

        if (Terrain.PositionOutOfBounds(nextPosition) ||
            (!Terrain.PixelIsSolidToLemming(currentPosition, lemming) ||
             Terrain.PixelIsSolidToLemming(nextPosition, lemming)))
            return false;

        if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(nextPosition, 1), lemming))
            return false;
        if (!Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(currentPosition, 1), lemming))
            return true;

        if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(nextPosition, 2), lemming))
            return false;
        if (!Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(currentPosition, 2), lemming))
            return true;

        if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(nextPosition, 3), lemming))
            return false;
        if (!Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(currentPosition, 3), lemming))
            return true;

        if (Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(nextPosition, 4), lemming))
            return false;
        return !Terrain.PixelIsSolidToLemming(lemming.Orientation.MoveDown(currentPosition, 4), lemming);
    }
}