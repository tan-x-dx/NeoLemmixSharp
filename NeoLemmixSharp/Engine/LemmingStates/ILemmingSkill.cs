using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LemmingStates;

public interface ILemmingSkill
{
    private static Dictionary<int, ILemmingSkill> LemmingStates { get; } = RegisterAllLemmingStates();

    private static Dictionary<int, ILemmingSkill> RegisterAllLemmingStates()
    {
        var result = new Dictionary<int, ILemmingSkill>();

        RegisterLemmingState(result, WalkerSkill.Instance);
        RegisterLemmingState(result, FallerSkill.Instance);
        RegisterLemmingState(result, AscenderSkill.Instance);

        return result;
    }

    private static void RegisterLemmingState(IDictionary<int, ILemmingSkill> dict, ILemmingSkill lemmingSkill)
    {
        dict.Add(lemmingSkill.LemmingStateId, lemmingSkill);
    }

    public static ICollection<ILemmingSkill> AllLemmingStates => LemmingStates.Values;
    
    string LemmingStateName { get; }
    int LemmingStateId { get; }

    void UpdateLemming(Lemming lemming);
}