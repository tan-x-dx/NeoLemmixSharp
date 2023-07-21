using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public abstract class LemmingAction : IEquatable<LemmingAction>
{
    protected static TerrainManager Terrain { get; private set; }

    public static ReadOnlyDictionaryWrapper<string, LemmingAction> AllActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionaryWrapper<string, LemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<string, LemmingAction>();

        // NOTE: DO NOT REGISTER THE NONE ACTION

        RegisterLemmingAction(WalkerAction.Instance);
        RegisterLemmingAction(ClimberAction.Instance);
        RegisterLemmingAction(FloaterAction.Instance);
        RegisterLemmingAction(BlockerAction.Instance);
        RegisterLemmingAction(BuilderAction.Instance);
        RegisterLemmingAction(BasherAction.Instance);
        RegisterLemmingAction(MinerAction.Instance);
        RegisterLemmingAction(DiggerAction.Instance);

        RegisterLemmingAction(PlatformerAction.Instance);
        RegisterLemmingAction(StackerAction.Instance);
        RegisterLemmingAction(FencerAction.Instance);
        RegisterLemmingAction(GliderAction.Instance);
        RegisterLemmingAction(JumperAction.Instance);
        RegisterLemmingAction(SwimmerAction.Instance);
        RegisterLemmingAction(ShimmierAction.Instance);
        RegisterLemmingAction(LasererAction.Instance);
        RegisterLemmingAction(SliderAction.Instance);

        RegisterLemmingAction(FallerAction.Instance);
        RegisterLemmingAction(AscenderAction.Instance);
        RegisterLemmingAction(ShruggerAction.Instance);
        RegisterLemmingAction(DrownerAction.Instance);
        RegisterLemmingAction(HoisterAction.Instance);
        RegisterLemmingAction(DehoisterAction.Instance);
        RegisterLemmingAction(ReacherAction.Instance);
        RegisterLemmingAction(DisarmerAction.Instance);

        RegisterLemmingAction(ExiterAction.Instance);
        RegisterLemmingAction(ExploderAction.Instance);
        RegisterLemmingAction(OhNoerAction.Instance);
        RegisterLemmingAction(SplatterAction.Instance);
        RegisterLemmingAction(StonerAction.Instance);
        RegisterLemmingAction(VaporiserAction.Instance);

        ValidateLemmingActionIds();

        return new ReadOnlyDictionaryWrapper<string, LemmingAction>(result);

        void RegisterLemmingAction(LemmingAction lemmingAction)
        {
            if (lemmingAction == NoneAction.Instance)
                return;

            result.Add(lemmingAction.LemmingActionName, lemmingAction);
        }

        void ValidateLemmingActionIds()
        {
            var ids = result
                .Values
                .Select(la => la.Id)
                .ToList();

            var numberOfUniqueIds = ids.Distinct().Count();

            if (numberOfUniqueIds != result.Count)
            {
                var idsString = string.Join(',', ids.OrderBy(i => i));

                throw new Exception($"Duplicated action ID: {idsString}");
            }

            var minActionId = ids.Min();
            var maxActionId = ids.Max();

            if (minActionId != 0 || maxActionId != result.Count - 1)
                throw new Exception($"Action ids do not span a full set of values from 0 - {result.Count - 1}");
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

    /// <summary>
    /// Find the new ground pixel. 
    /// If result = 4, then at least 4 pixels are air below levelPosition. 
    /// If result = -7, then at least 7 pixels are terrain above levelPosition
    /// </summary>
    protected static int FindGroundPixel(
        Lemming lemming,
        Orientation orientation,
        LevelPosition levelPosition)
    {
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