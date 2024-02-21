namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SkillAssignScrollButtonAction : IButtonAction
{
    private readonly int _delta;

    public SkillAssignScrollButtonAction(int delta)
    {
        _delta = delta;
    }

    public ButtonType ButtonType => _delta > 0
        ? ButtonType.SkillScrollLeft
        : ButtonType.SkillScrollRight;

    public void OnMouseDown()
    {
        LevelScreen.LevelControlPanel.ChangeSkillAssignButtonScroll(_delta);
    }

    public void OnPress(bool isDoubleTap)
    {
    }

    public void OnRightClick()
    {
    }
}