using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu;

public sealed class LevelEditorControlPanel : Component
{
    private const int LevelControlPanelWidth = 280;

    public LevelEditorControlPanel(UiHandler uiHandler)
    {
        Width = LevelControlPanelWidth;

        var titleLabel = new TextLabel(UiConstants.StandardInset, UiConstants.StandardInset, "Title", UiConstants.RectangularButtonDefaultColours);
        AddComponent(titleLabel);
        var titleTextField = new TextField(uiHandler, titleLabel.Width + UiConstants.TwiceStandardInset, titleLabel.Top, string.Empty);
        titleLabel.Width = 120;
        titleTextField.Width = 200;
        titleTextField.SetCapacity(6);
        AddComponent(titleTextField);

    }
}
