using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.LevelBuilding.Data;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class LevelControlPanel
{
    private readonly SkillAssignButton[] _skillAssignButtons;

    private SkillAssignButton? _selectedSkillAssignButton;

    public LevelControlPanel(SkillSet skillSet)
    {
        _skillAssignButtons = CreateSkillAssignButtons(skillSet);

        _selectedSkillAssignButton = _skillAssignButtons.FirstOrDefault();
    }

    private static SkillAssignButton[] CreateSkillAssignButtons(SkillSet skillSet)
    {
        var tempList = new List<SkillAssignButton>();

        if (skillSet.NumberOfBashers.HasValue) { tempList.Add(new SkillAssignButton(BasherSkill.Instance, skillSet.NumberOfBashers.Value)); }
        if (skillSet.NumberOfBlockers.HasValue) { tempList.Add(new SkillAssignButton(BlockerSkill.Instance, skillSet.NumberOfBlockers.Value)); }
        if (skillSet.NumberOfBombers.HasValue) { tempList.Add(new SkillAssignButton(BomberSkill.Instance, skillSet.NumberOfBombers.Value)); }
        if (skillSet.NumberOfBuilders.HasValue) { tempList.Add(new SkillAssignButton(BuilderSkill.Instance, skillSet.NumberOfBuilders.Value)); }
        if (skillSet.NumberOfClimbers.HasValue) { tempList.Add(new SkillAssignButton(ClimberSkill.Instance, skillSet.NumberOfClimbers.Value)); }
        if (skillSet.NumberOfCloners.HasValue) { tempList.Add(new SkillAssignButton(ClonerSkill.Instance, skillSet.NumberOfCloners.Value)); }
        if (skillSet.NumberOfDiggers.HasValue) { tempList.Add(new SkillAssignButton(DiggerSkill.Instance, skillSet.NumberOfDiggers.Value)); }
        if (skillSet.NumberOfDisarmers.HasValue) { tempList.Add(new SkillAssignButton(DisarmerSkill.Instance, skillSet.NumberOfDisarmers.Value)); }
        if (skillSet.NumberOfFencers.HasValue) { tempList.Add(new SkillAssignButton(FencerSkill.Instance, skillSet.NumberOfFencers.Value)); }
        if (skillSet.NumberOfFloaters.HasValue) { tempList.Add(new SkillAssignButton(FloaterSkill.Instance, skillSet.NumberOfFloaters.Value)); }
        if (skillSet.NumberOfGliders.HasValue) { tempList.Add(new SkillAssignButton(GliderSkill.Instance, skillSet.NumberOfGliders.Value)); }
        if (skillSet.NumberOfJumpers.HasValue) { tempList.Add(new SkillAssignButton(JumperSkill.Instance, skillSet.NumberOfJumpers.Value)); }
        if (skillSet.NumberOfLaserers.HasValue) { tempList.Add(new SkillAssignButton(LasererSkill.Instance, skillSet.NumberOfLaserers.Value)); }
        if (skillSet.NumberOfMiners.HasValue) { tempList.Add(new SkillAssignButton(MinerSkill.Instance, skillSet.NumberOfMiners.Value)); }
        if (skillSet.NumberOfPlatformers.HasValue) { tempList.Add(new SkillAssignButton(PlatformerSkill.Instance, skillSet.NumberOfPlatformers.Value)); }
        if (skillSet.NumberOfShimmiers.HasValue) { tempList.Add(new SkillAssignButton(ShimmierSkill.Instance, skillSet.NumberOfShimmiers.Value)); }
        if (skillSet.NumberOfSliders.HasValue) { tempList.Add(new SkillAssignButton(SliderSkill.Instance, skillSet.NumberOfSliders.Value)); }
        if (skillSet.NumberOfStackers.HasValue) { tempList.Add(new SkillAssignButton(StackerSkill.Instance, skillSet.NumberOfStackers.Value)); }
        if (skillSet.NumberOfStoners.HasValue) { tempList.Add(new SkillAssignButton(StonerSkill.Instance, skillSet.NumberOfStoners.Value)); }
        if (skillSet.NumberOfSwimmers.HasValue) { tempList.Add(new SkillAssignButton(SwimmerSkill.Instance, skillSet.NumberOfSwimmers.Value)); }
        if (skillSet.NumberOfWalkers.HasValue) { tempList.Add(new SkillAssignButton(WalkerSkill.Instance, skillSet.NumberOfWalkers.Value)); }

        return tempList.ToArray();
    }

    public IEnumerable<SkillAssignButton> SkillAssignButtons => _skillAssignButtons;
}