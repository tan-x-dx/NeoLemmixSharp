using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public interface ILemmingSkill
{
    private static Dictionary<int, ILemmingSkill> LemmingSkills { get; } = RegisterAllLemmingSkills();

    private static Dictionary<int, ILemmingSkill> RegisterAllLemmingSkills()
    {
        var result = new Dictionary<int, ILemmingSkill>();

        RegisterLemmingSkill(result, WalkerSkill.Instance);
        RegisterLemmingSkill(result, FallerSkill.Instance);
        RegisterLemmingSkill(result, AscenderSkill.Instance);

        return result;
    }

    private static void RegisterLemmingSkill(IDictionary<int, ILemmingSkill> dict, ILemmingSkill lemmingSkill)
    {
        dict.Add(lemmingSkill.LemmingSkillId, lemmingSkill);
    }

    public static ICollection<ILemmingSkill> AllLemmingStates => LemmingSkills.Values;
    
    string LemmingSkillName { get; }
    int LemmingSkillId { get; }

    void UpdateLemming(Lemming lemming);
}