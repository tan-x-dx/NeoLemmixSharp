using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NeoLemmixSharp.Engine.LevelPixels;

namespace NeoLemmixSharp.Engine.LemmingSkills;

public abstract class LemmingSkill : IEquatable<LemmingSkill>
{
    protected static PixelManager Terrain { get; private set; }

    public static ReadOnlyDictionary<string, LemmingSkill> LemmingSkills { get; } = RegisterAllLemmingSkills();

    private static ReadOnlyDictionary<string, LemmingSkill> RegisterAllLemmingSkills()
    {
        var result = new Dictionary<string, LemmingSkill>();

        RegisterLemmingSkill(NoneSkill.Instance);

        RegisterLemmingSkill(new BasherSkill(0));
        RegisterLemmingSkill(new BlockerSkill(0));
        RegisterLemmingSkill(new BomberSkill(0));
        RegisterLemmingSkill(new BuilderSkill(0));
        RegisterLemmingSkill(new ClimberSkill(0));
        RegisterLemmingSkill(new ClonerSkill(0));
        RegisterLemmingSkill(new DiggerSkill(0));
        RegisterLemmingSkill(new DisarmerSkill(0));
        RegisterLemmingSkill(new FencerSkill(0));
        RegisterLemmingSkill(new FloaterSkill(0));
        RegisterLemmingSkill(new GliderSkill(0));
        RegisterLemmingSkill(new JumperSkill(0));
        RegisterLemmingSkill(new LasererSkill(0));
        RegisterLemmingSkill(new MinerSkill(0));
        RegisterLemmingSkill(new PlatformerSkill(0));
        RegisterLemmingSkill(new ShimmierSkill(0));
        RegisterLemmingSkill(new SliderSkill(0));
        RegisterLemmingSkill(new StackerSkill(0));
        RegisterLemmingSkill(new StonerSkill(0));
        RegisterLemmingSkill(new SwimmerSkill(0));
        RegisterLemmingSkill(new WalkerSkill(0));

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

    public static void SetTerrain(PixelManager terrain)
    {
        Terrain = terrain;
    }

    public abstract int LemmingSkillId { get; }
    public abstract string LemmingSkillName { get; }
    public abstract bool IsPermanentSkill { get; }

    public int OriginalNumberOfSkillsAvailable { get; }
    public int CurrentNumberOfSkillsAvailable { get; private set; }

    protected LemmingSkill(int originalNumberOfSkillsAvailable)
    {
        OriginalNumberOfSkillsAvailable = originalNumberOfSkillsAvailable;
        CurrentNumberOfSkillsAvailable = originalNumberOfSkillsAvailable;
    }

    public abstract bool CanAssignToLemming(Lemming lemming);
    public abstract bool AssignToLemming(Lemming lemming);

    public bool Equals(LemmingSkill? other) => LemmingSkillId == (other?.LemmingSkillId ?? -1);
    public sealed override bool Equals(object? obj) => obj is LemmingSkill other && LemmingSkillId == other.LemmingSkillId;
    public sealed override int GetHashCode() => LemmingSkillId;
    public sealed override string ToString() => LemmingSkillName;

    public static bool operator ==(LemmingSkill left, LemmingSkill right) => left.LemmingSkillId == right.LemmingSkillId;
    public static bool operator !=(LemmingSkill left, LemmingSkill right) => left.LemmingSkillId != right.LemmingSkillId;
}