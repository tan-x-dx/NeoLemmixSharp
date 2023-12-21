using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class ReleaseRatePlusButton : ControlPanelButton
{
    public ReleaseRatePlusButton(int skillPanelFrame)
        : base(skillPanelFrame)
    {
    }

    public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
    {
        return new ReleaseRatePlusButtonRenderer(spriteBank, this);
    }
}