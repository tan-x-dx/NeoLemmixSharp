using NeoLemmixSharp.Engine.ControlPanel;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public sealed class SkillAssignButtonRenderer : ControlPanelButtonRenderer
{
    private readonly SkillAssignButton _skillAssignButton;

    public SkillAssignButtonRenderer(SkillAssignButton skillAssignButton)
    {
        _skillAssignButton = skillAssignButton;
    }
}