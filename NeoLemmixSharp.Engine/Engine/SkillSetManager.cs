using NeoLemmixSharp.Engine.Engine.Skills;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class SkillSetManager
{
    private readonly LemmingSkill[] _skillList;

    public SkillSetManager(ICollection<SkillSetData> skillSetData)
    {
        _skillList = CreateSkillList(skillSetData);
    }

    public IEnumerable<LemmingSkill> AllSkills => _skillList;

    private static LemmingSkill[] CreateSkillList(ICollection<SkillSetData> skillSetData)
    {
        var tempList = new List<LemmingSkill>();
        /*
        if (skillSetData.NumberOfBashers.HasValue) { tempList.Add(new BasherSkill(skillSetData.NumberOfBashers.Value)); }
        if (skillSetData.NumberOfBlockers.HasValue) { tempList.Add(new BlockerSkill(skillSetData.NumberOfBlockers.Value)); }
        if (skillSetData.NumberOfBombers.HasValue) { tempList.Add(new BomberSkill(skillSetData.NumberOfBombers.Value)); }
        if (skillSetData.NumberOfBuilders.HasValue) { tempList.Add(new BuilderSkill(skillSetData.NumberOfBuilders.Value)); }
        if (skillSetData.NumberOfClimbers.HasValue) { tempList.Add(new ClimberSkill(skillSetData.NumberOfClimbers.Value)); }
        if (skillSetData.NumberOfCloners.HasValue) { tempList.Add(new ClonerSkill(skillSetData.NumberOfCloners.Value)); }
        if (skillSetData.NumberOfDiggers.HasValue) { tempList.Add(new DiggerSkill(skillSetData.NumberOfDiggers.Value)); }
        if (skillSetData.NumberOfDisarmers.HasValue) { tempList.Add(new DisarmerSkill(skillSetData.NumberOfDisarmers.Value)); }
        if (skillSetData.NumberOfFencers.HasValue) { tempList.Add(new FencerSkill(skillSetData.NumberOfFencers.Value)); }
        if (skillSetData.NumberOfFloaters.HasValue) { tempList.Add(new FloaterSkill(skillSetData.NumberOfFloaters.Value)); }
        if (skillSetData.NumberOfGliders.HasValue) { tempList.Add(new GliderSkill(skillSetData.NumberOfGliders.Value)); }
        if (skillSetData.NumberOfJumpers.HasValue) { tempList.Add(new JumperSkill(skillSetData.NumberOfJumpers.Value)); }
        if (skillSetData.NumberOfLaserers.HasValue) { tempList.Add(new LasererSkill(skillSetData.NumberOfLaserers.Value)); }
        if (skillSetData.NumberOfMiners.HasValue) { tempList.Add(new MinerSkill(skillSetData.NumberOfMiners.Value)); }
        if (skillSetData.NumberOfPlatformers.HasValue) { tempList.Add(new PlatformerSkill(skillSetData.NumberOfPlatformers.Value)); }
        if (skillSetData.NumberOfShimmiers.HasValue) { tempList.Add(new ShimmierSkill(skillSetData.NumberOfShimmiers.Value)); }
        if (skillSetData.NumberOfSliders.HasValue) { tempList.Add(new SliderSkill(skillSetData.NumberOfSliders.Value)); }
        if (skillSetData.NumberOfStackers.HasValue) { tempList.Add(new StackerSkill(skillSetData.NumberOfStackers.Value)); }
        if (skillSetData.NumberOfStoners.HasValue) { tempList.Add(new StonerSkill(skillSetData.NumberOfStoners.Value)); }
        if (skillSetData.NumberOfSwimmers.HasValue) { tempList.Add(new SwimmerSkill(skillSetData.NumberOfSwimmers.Value)); }
        if (skillSetData.NumberOfWalkers.HasValue) { tempList.Add(new WalkerSkill(skillSetData.NumberOfWalkers.Value)); }
        */
        return tempList.ToArray();
    }

    public bool SkillIsAvailable(LemmingSkill lemmingSkill)
    {
        return true;
    }

    public int NumberOfSkillsAvailable(LemmingSkill queuedSkill)
    {
        return 1;
    }
}