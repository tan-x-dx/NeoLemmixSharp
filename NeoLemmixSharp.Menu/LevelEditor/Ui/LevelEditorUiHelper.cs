using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.LevelEditor.Components;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Ui;

public static class LevelEditorUiHelper
{
    private const int TopPanelHeight = 128;
    private const int LeftPanelWidth = 280;

    public static void OnResizeLevelEditor(
        Component topPanel,
        Component leftPanel,
        Component pieceBank,
        LevelCanvas levelCanvas)
    {
        var windowSize = IGameWindow.Instance.WindowSize;

        topPanel.Left = 0;
        topPanel.Top = 0;
        topPanel.Width = windowSize.W;
        topPanel.Height = TopPanelHeight;

        leftPanel.Left = 0;
        leftPanel.Top = topPanel.Height;

        pieceBank.Left = 0;
        pieceBank.Top = windowSize.H - pieceBank.Height;
        pieceBank.Width = windowSize.W;

        levelCanvas.Left = leftPanel.Width;
        levelCanvas.Top = TopPanelHeight;
        levelCanvas.Width = windowSize.W - leftPanel.Width;
        levelCanvas.Height = windowSize.H - topPanel.Height - pieceBank.Height;
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
