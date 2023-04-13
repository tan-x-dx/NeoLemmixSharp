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
    public abstract void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction);

    public bool Equals(LemmingAction? other) => ActionId == (other?.ActionId ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingAction other && Equals(other);
    public sealed override int GetHashCode() => ActionId;

    public static bool operator ==(LemmingAction left, LemmingAction right) => left.ActionId == right.ActionId;
    public static bool operator !=(LemmingAction left, LemmingAction right) => left.ActionId != right.ActionId;
}