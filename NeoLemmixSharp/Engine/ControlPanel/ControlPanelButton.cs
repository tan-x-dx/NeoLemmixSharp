namespace NeoLemmixSharp.Engine.ControlPanel;

public class ControlPanelButton
{
    public int SkillPanelFrame { get; }

    public int ScreenX { get; set; }
    public int ScreenY { get; set; }

    public int ScreenWidth { get; set; }
    public int ScreenHeight { get; set; }

    public int ScaleMultiplier { get; set; }

    public ControlPanelButton(int skillPanelFrame)
    {
        SkillPanelFrame = skillPanelFrame;
    }

    public virtual bool TryPress(int mouseX, int mouseY)
    {
        return MouseIsOverButton(mouseX, mouseY);
    }

    protected bool MouseIsOverButton(int mouseX, int mouseY)
    {
        return mouseX >= ScreenX && mouseX <= ScreenX + ScreenWidth &&
               mouseY >= ScreenY && mouseY <= ScreenY + ScreenHeight;
    }
}