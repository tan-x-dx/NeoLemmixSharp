using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu;

public sealed class LevelEditorControlPanel : Component
{
    private const int LevelControlPanelWidth = 280;

    private readonly TextField _titleTextField;

    public LevelEditorControlPanel(UiHandler uiHandler)
    {
        Width = LevelControlPanelWidth;

        var titleLabel = new TextLabel(UiConstants.StandardInset, UiConstants.StandardInset, "Title", UiConstants.RectangularButtonDefaultColours);
        AddComponent(titleLabel);
        _titleTextField = new TextField(uiHandler, titleLabel.Width + UiConstants.TwiceStandardInset, titleLabel.Top, string.Empty);
        titleLabel.Width = 120;
        _titleTextField.Width = 200;
        _titleTextField.SetCapacity(100);
        AddComponent(_titleTextField);

    }

    public void SetLevelData(LevelData levelData)
    {
        _titleTextField.SetText(levelData.LevelTitle);
    }
}
