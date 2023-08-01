using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Orientations;
using NeoLemmixSharp.Engine.Engine.Terrain;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public abstract class LemmingAction : IEquatable<LemmingAction>, IUniqueIdItem
{
    private static readonly LemmingAction[] LemmingActions = RegisterAllLemmingActions();
    protected static TerrainManager Terrain { get; private set; }

    public static ReadOnlySpan<LemmingAction> AllLemmingActions => new(LemmingActions);

    private static LemmingAction[] RegisterAllLemmingActions()
    {
        var list = new List<LemmingAction>();

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

        ListValidatorMethods.ValidateUniqueIds(list);

        list.Sort((x, y) => x.Id.CompareTo(y.Id));

        return list.ToArray();

        void RegisterLemmingAction(LemmingAction lemmingAction)
        {
            if (lemmingAction == NoneAction.Instance)
                return;

            list.Add(lemmingAction);
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
        Orientation orientation,
        LevelPosition levelPosition)
    {
        var result = 0;
        if (Terrain.PixelIsSolidToLemming(orientation, levelPosition))
        {
            while (Terrain.PixelIsSolidToLemming(orientation, orientation.MoveUp(levelPosition, 1 - result)) &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!Terrain.PixelIsSolidToLemming(orientation, orientation.MoveDown(levelPosition, result)) &&
               result < 4)
        {
            result++;
        }

        return result;
    }
}