using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.LemmingActions;

public interface ILemmingAction : IEquatable<ILemmingAction>
{
    public static ReadOnlyDictionary<string, ILemmingAction> LemmingActions { get; } = RegisterAllLemmingActions();

    private static ReadOnlyDictionary<string, ILemmingAction> RegisterAllLemmingActions()
    {
        var result = new Dictionary<string, ILemmingAction>();

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

        return new ReadOnlyDictionary<string, ILemmingAction>(result);
    }

    private static void RegisterLemmingAction(IDictionary<string, ILemmingAction> dict, ILemmingAction lemmingAction)
    {
        dict.Add(lemmingAction.LemmingActionName, lemmingAction);
    }

    public static ICollection<ILemmingAction> AllLemmingActions => LemmingActions.Values;

    protected static PixelManager Terrain => LevelScreen.CurrentLevel!.Terrain;

    LemmingActionSpriteBundle ActionSpriteBundle { get; set; }

    string LemmingActionName { get; }
    int NumberOfAnimationFrames { get; }

    void UpdateLemming(Lemming lemming);
    void OnTransitionToAction(Lemming lemming, bool previouslyStartingAction);
}