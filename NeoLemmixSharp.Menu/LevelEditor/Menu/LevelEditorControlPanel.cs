using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu;

public sealed class LevelEditorControlPanel : Component
{
    private const int LevelControlPanelWidth = 280;

    public LevelEditorControlPanel()
    {
        Width = LevelControlPanelWidth;

        var titleLabel = new TextLabel(UiConstants.StandardInset, UiConstants.StandardInset, "Title", UiConstants.RectangularButtonDefaultColours);
        AddComponent(titleLabel);
        var titleTextField = new TextField(titleLabel.Width + UiConstants.TwiceStandardInset, titleLabel.Top, string.Empty);
        AddComponent(titleTextField);

    }
}
