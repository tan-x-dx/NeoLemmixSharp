using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.LemmingActions;

public abstract class LemmingAction : IEquatable<LemmingAction>
{
    public static ReadOnlyDictionary<string, LemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, LemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<string, LemmingAction>();

        RegisterLemmingAction(result, AscenderAction.Instance);
        RegisterLemmingAction(result, BasherAction.Instance);
        RegisterLemmingAction(result, BlockerAction.Instance);
        RegisterLemmingAction(result, BuilderAction.Instance);
        RegisterLemmingAction(result, ClimberAction.Instance);
        RegisterLemmingAction(result, DehoisterAction.Instance);
        RegisterLemmingAction(result, DiggerAction.Instance);
        RegisterLemmingAction(result, DisarmerAction.Instance);
        RegisterLemmingAction(result, DrownerAction.Instance);
        RegisterLemmingAction(result, ExiterAction.Instance);
        RegisterLemmingAction(result, ExploderAction.Instance);
        RegisterLemmingAction(result, FencerAction.Instance);
        RegisterLemmingAction(result, FallerAction.Instance);
        RegisterLemmingAction(result, FloaterAction.Instance);
        RegisterLemmingAction(result, GliderAction.Instance);
        RegisterLemmingAction(result, HoisterAction.Instance);
        RegisterLemmingAction(result, JumperAction.Instance);
        RegisterLemmingAction(result, LasererAction.Instance);
        RegisterLemmingAction(result, MinerAction.Instance);
        RegisterLemmingAction(result, OhNoerAction.Instance);
        RegisterLemmingAction(result, PlatformerAction.Instance);
        RegisterLemmingAction(result, ReacherAction.Instance);
        RegisterLemmingAction(result, ShimmierAction.Instance);
        RegisterLemmingAction(result, ShruggerAction.Instance);
        RegisterLemmingAction(result, SliderAction.Instance);
        RegisterLemmingAction(result, SplatterAction.Instance);
        RegisterLemmingAction(result, StackerAction.Instance);
        RegisterLemmingAction(result, StonerAction.Instance);
        RegisterLemmingAction(result, SwimmerAction.Instance);
        RegisterLemmingAction(result, VaporiserAction.Instance);
        RegisterLemmingAction(result, WalkerAction.Instance);

        return new ReadOnlyDictionary<string, LemmingAction>(result);
    }

    private static void RegisterLemmingAction(IDictionary<string, LemmingAction> dict, LemmingAction lemmingAction)
    {
        dict.Add(lemmingAction.LemmingActionName, lemmingAction);
    }

    public static ICollection<LemmingAction> AllLemmingActions => LemmingActions.Values;

    protected static PixelManager Terrain => LevelScreen.CurrentLevel!.Terrain;

    public LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    public abstract string LemmingActionName { get; }
    public abstract int NumberOfAnimationFrames { get; }
    public abstract bool IsOneTimeAction { get; }

    public abstract bool UpdateLemming(Lemming lemming);
    public abstract void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction);

    public bool Equals(LemmingAction? other)
    {
        throw new NotImplementedException();
    }

    public sealed override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LemmingAction)obj);
    }

    public sealed override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}