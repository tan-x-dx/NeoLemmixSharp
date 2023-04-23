using NeoLemmixSharp.Engine.LemmingActions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public abstract class LemmingSkill : IEquatable<LemmingSkill>
{
    public static ReadOnlyDictionary<string, LemmingSkill> LemmingSkills { get; } = RegisterAllLemmingSkills();

    private static ReadOnlyDictionary<string, LemmingSkill> RegisterAllLemmingSkills()
    {
        var result = new Dictionary<string, LemmingSkill>();

        RegisterLemmingSkill(BasherSkill.Instance);
        RegisterLemmingSkill(BlockerSkill.Instance);
        RegisterLemmingSkill(BomberSkill.Instance);
        RegisterLemmingSkill(BuilderSkill.Instance);
        RegisterLemmingSkill(ClimberSkill.Instance);
        RegisterLemmingSkill(ClonerSkill.Instance);
        RegisterLemmingSkill(DiggerSkill.Instance);
        RegisterLemmingSkill(DisarmerSkill.Instance);
        RegisterLemmingSkill(FencerSkill.Instance);
        RegisterLemmingSkill(FloaterSkill.Instance);
        RegisterLemmingSkill(GliderSkill.Instance);
        RegisterLemmingSkill(JumperSkill.Instance);
        RegisterLemmingSkill(LasererSkill.Instance);
        RegisterLemmingSkill(MinerSkill.Instance);
        RegisterLemmingSkill(PlatformerSkill.Instance);
        RegisterLemmingSkill(ShimmierSkill.Instance);
        RegisterLemmingSkill(SliderSkill.Instance);
        RegisterLemmingSkill(StackerSkill.Instance);
        RegisterLemmingSkill(StonerSkill.Instance);
        RegisterLemmingSkill(SwimmerSkill.Instance);
        RegisterLemmingSkill(WalkerSkill.Instance);

        var numberOfUniqueIds = result
            .Values
            .Select(la => la.LemmingSkillId)
            .Distinct()
            .Count();

        if (numberOfUniqueIds != result.Count)
        {
            var ids = string.Join(',', result
                .Values
                .Select(la => la.LemmingSkillId)
                .OrderBy(i => i));

            throw new Exception($"Duplicated skill ID: {ids}");
        }

        return new ReadOnlyDictionary<string, LemmingSkill>(result);

        void RegisterLemmingSkill(LemmingSkill lemmingSkill)
        {
            result.Add(lemmingSkill.LemmingSkillName, lemmingSkill);
        }
    }

    public static ICollection<LemmingSkill> AllLemmingSkills => LemmingSkills.Values;

    protected static PixelManager Terrain => LevelScreen.CurrentLevel.Terrain;

    public abstract int LemmingSkillId { get; }
    public abstract string LemmingSkillName { get; }
    public abstract bool IsPermanentSkill { get; }

    protected static bool LemmingActionCanBeAssignedPermanentSkill(Lemming lemming, bool includeDrowner = true)
    {
        return lemming.CurrentAction != OhNoerAction.Instance &&
               lemming.CurrentAction != StonerAction.Instance &&
               lemming.CurrentAction != ExploderAction.Instance &&
               lemming.CurrentAction != StonerAction.Instance &&
               (!includeDrowner || lemming.CurrentAction != DrownerAction.Instance) &&
               lemming.CurrentAction != VaporiserAction.Instance &&
               lemming.CurrentAction != SplatterAction.Instance &&
               lemming.CurrentAction != ExiterAction.Instance;
    }

    /*public bool AssignNewSkill()
    {

    }*/

    public abstract bool CanAssignToLemming(Lemming lemming);
    public abstract bool AssignToLemming(Lemming lemming);

    public bool Equals(LemmingSkill? other) => LemmingSkillId == (other?.LemmingSkillId ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingSkill other && LemmingSkillId == other.LemmingSkillId;
    public sealed override int GetHashCode() => LemmingSkillId;
    public sealed override string ToString() => LemmingSkillName;

    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.LemmingSkillId == right.LemmingSkillId;
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.LemmingSkillId != right.LemmingSkillId;
}