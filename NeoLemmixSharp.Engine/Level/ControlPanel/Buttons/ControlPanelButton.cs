using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public class ControlPanelButton
{
    private const int NumberOfSkillPanels = 8;
    public const int SkillPanelFrameMask = NumberOfSkillPanels - 1;

    private readonly int _iconX;
    private readonly int _iconY;

    public readonly int ButtonId;

    public readonly int SkillPanelFrame;
    public int X;
    public int Y;
    public int Width;
    public int Height;

    public bool ShouldRender = true;
    public bool IsSelected;

    public IButtonAction ButtonAction { get; protected init; }

    protected ControlPanelButton(
        int buttonId,
        int skillPanelFrame)
    {
        ButtonId = buttonId;
        SkillPanelFrame = skillPanelFrame & SkillPanelFrameMask;
    }

    public ControlPanelButton(
        int buttonId,
        int skillPanelFrame,
        IButtonAction buttonAction,
        int iconX,
        int iconY)
    {
        ButtonId = buttonId;

        _iconX = iconX;
        _iconY = iconY;

        ButtonAction = buttonAction;
        SkillPanelFrame = skillPanelFrame & SkillPanelFrameMask;
    }

    public bool MouseIsOverButton(int mouseX, int mouseY)
    {
        return ShouldRender &&
               mouseX >= X && mouseX < X + Width &&
               mouseY >= Y && mouseY < Y + Height;
    }

    public virtual ReadOnlySpan<char> GetDigitsToRender() => ReadOnlySpan<char>.Empty;
    public virtual int GetNumberOfDigitsToRender() => 0;

    public virtual ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
    {
        return new ControlPanelButtonRenderer(spriteBank, this, _iconX, _iconY);
    }
}