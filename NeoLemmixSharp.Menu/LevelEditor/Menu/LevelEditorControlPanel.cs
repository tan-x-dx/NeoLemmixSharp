using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu;

public sealed class LevelEditorControlPanel : Component
{
    private const int LevelControlPanelWidth = 280;

    private readonly TextField _titleTextField;

    public LevelEditorControlPanel()
    {
        Width = LevelControlPanelWidth;

        var titleLabel = new TextLabel(UiConstants.StandardInset, UiConstants.StandardInset, "Title", UiConstants.AllBlackColours);
        AddComponent(titleLabel);
        _titleTextField = new TextField(titleLabel.Width + UiConstants.TwiceStandardInset, UiConstants.StandardInset);
        titleLabel.Top = UiConstants.StandardInset + 6;
        _titleTextField.Width = 200;
        _titleTextField.SetCapacity(100);
        AddComponent(_titleTextField);

    }

    public void SetLevelData(LevelData levelData)
    {
        _titleTextField.SetText(levelData.LevelTitle);
    }
}
