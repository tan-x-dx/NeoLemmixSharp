using NeoLemmixSharp.Engine.Orientations;
using NeoLemmixSharp.Engine.Terrain;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeoLemmixSharp.Engine.Actions;

public abstract class LemmingAction : IEquatable<LemmingAction>
{
    protected static TerrainManager Terrain { get; private set; }

    public static ReadOnlyDictionary<string, LemmingAction> AllActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, LemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<string, LemmingAction>();

        // NOTE: DO NOT REGISTER THE NONE ACTION

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
        RegisterLemmingAction(FallerAction.Instance);
        RegisterLemmingAction(FencerAction.Instance);
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
            .Select(la => la.Id)
            .Distinct()
            .Count();

        if (numberOfUniqueIds != result.Count)
        {
            var ids = string.Join(',', result
                .Values
                .Select(la => la.Id)
                .OrderBy(i => i));

            throw new Exception($"Duplicated action ID: {ids}");
        }

        return new ReadOnlyDictionary<string, LemmingAction>(result);

        void RegisterLemmingAction(LemmingAction lemmingAction)
        {
            result.Add(lemmingAction.LemmingActionName, lemmingAction);
        }
    }

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    public abstract int Id { get; }
    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }
    public abstract bool CanBeAssignedPermanentSkill { get; }

    public abstract bool UpdateLemming(Lemming lemming);

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
            lemming.SetFacingDirection(lemming.FacingDirection.OppositeDirection);
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

    protected static void LayBrick(Lemming lemming)
    {
        var orientation = lemming.Orientation;
        var dx = lemming.FacingDirection.DeltaX;
        var dy = lemming.CurrentAction == BuilderAction.Instance
            ? 1
            : 0;

        var brickPosition = lemming.LevelPosition;
        brickPosition = orientation.MoveUp(brickPosition, dy);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);

        brickPosition = orientation.MoveRight(brickPosition, dx);
        Terrain.SetSolidPixel(brickPosition, uint.MaxValue);
    }

    protected static int FindGroundPixel(
        Lemming lemming,
        Orientation orientation,
        LevelPosition levelPosition)
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
}