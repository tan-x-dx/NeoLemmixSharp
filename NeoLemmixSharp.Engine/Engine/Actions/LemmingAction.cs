using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Terrain;

namespace NeoLemmixSharp.Engine.Engine.Actions;

public abstract class LemmingAction : IUniqueIdItem<LemmingAction>
{
    private static readonly LemmingAction[] LemmingActions = RegisterAllLemmingActions();
    protected static TerrainManager Terrain { get; private set; }

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
        Array.Sort(result, UniqueIdItemComparer<LemmingAction>.Instance);

        return result;
    }

    public static void SetTerrain(TerrainManager terrain)
    {
        Terrain = terrain;
    }

    public abstract int Id { get; }
    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }
    public abstract int CursorSelectionPriorityValue { get; }

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
        Lemming lemming,
        LevelPosition levelPosition)
    {
        var result = 0;
        if (Terrain.PixelIsSolidToLemming(lemming, levelPosition))
        {
            while (Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveUp(levelPosition, 1 - result)) &&
                   result > -7)
            {
                result--;
            }

            return result;
        }

        result = 1;
        while (!Terrain.PixelIsSolidToLemming(lemming, lemming.Orientation.MoveDown(levelPosition, result)) &&
               result < 4)
        {
            result++;
        }

        return result;
    }
}