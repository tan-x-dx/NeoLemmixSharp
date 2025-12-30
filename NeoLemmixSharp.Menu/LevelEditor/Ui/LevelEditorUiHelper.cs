using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.LevelEditor.Components;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Ui;

public static class LevelEditorUiHelper
{
    private const int TopPanelHeight = 128;
    private const int LeftPanelWidth = 280;
    private const int BottomPanelHeight = 256;

    public static void OnResizeLevelEditor(
        Tab topPanel,
        Tab leftPanel,
        Tab bottomPanel,
        LevelCanvas levelCanvas)
    {
        var windowSize = IGameWindow.Instance.WindowSize;

        topPanel.Left = 0;
        topPanel.Top = 0;
        topPanel.Width = windowSize.W;
        topPanel.Height = TopPanelHeight;

        leftPanel.Left = 0;
        leftPanel.Top = TopPanelHeight;
        leftPanel.Width = LeftPanelWidth;
        leftPanel.Height = windowSize.H - TopPanelHeight - BottomPanelHeight;

        bottomPanel.Left = 0;
        bottomPanel.Top = windowSize.H - BottomPanelHeight;
        bottomPanel.Width = windowSize.W;
        bottomPanel.Height = BottomPanelHeight;

        levelCanvas.Left = LeftPanelWidth;
        levelCanvas.Top = TopPanelHeight;
        levelCanvas.Width = windowSize.W - LeftPanelWidth;
        levelCanvas.Height = windowSize.H - topPanel.Height - BottomPanelHeight;
    }

    public static Tab BuildLeftTab()
    {
        var leftTab = new Tab(0, 0, LeftPanelWidth, 10);

        var titleLabel = new TextLabel(UiConstants.StandardInset, UiConstants.StandardInset, "Title", UiConstants.RectangularButtonDefaultColours);
        leftTab.AddComponent(titleLabel);
        var titleTextField = new TextField(titleLabel.Width + UiConstants.TwiceStandardInset, titleLabel.Top);
        leftTab.AddComponent(titleTextField);

        return leftTab;
    }
}
