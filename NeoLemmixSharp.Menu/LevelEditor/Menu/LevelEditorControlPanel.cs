using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu;

public sealed class LevelEditorControlPanel : Component
{
    private const int LevelControlPanelWidth = 280;
    private const int TextFieldLeftPosition = 72;
    private const int TextFieldWidth = LevelControlPanelWidth - TextFieldLeftPosition - UiConstants.StandardInset;

    public TextField TitleTextField { get; }
    public TextField AuthorTextField { get; }
    public TextField MusicTextField { get; }

    public TextField LevelWidthTextField { get; }
    public TextField LevelHeightTextField { get; }
    public TextField LevelXStartTextField { get; }
    public TextField LevelYStartTextField { get; }

    public TextField LevelIdTextField { get; }

    public LevelEditorControlPanel()
    {
        Width = LevelControlPanelWidth;
        Colors = UiConstants.LighterRectangularButtonColours;

        TitleTextField = new TextField(TextFieldLeftPosition, UiConstants.StandardInset);
        TitleTextField.Width = TextFieldWidth;
        TitleTextField.SetCapacity(40);

        var titleLabel = new TextLabel(UiConstants.StandardInset, TitleTextField.Top, "Title", UiConstants.AllBlackColours);
        titleLabel.LabelOffsetY = TitleTextField.TextYOffset;

        AuthorTextField = new TextField(TextFieldLeftPosition, UiConstants.StandardInset + TitleTextField.Bottom);
        AuthorTextField.Width = TextFieldWidth;
        AuthorTextField.SetCapacity(32);

        var authorLabel = new TextLabel(UiConstants.StandardInset, AuthorTextField.Top, "Author", UiConstants.AllBlackColours);
        authorLabel.LabelOffsetY = TitleTextField.TextYOffset;

        MusicTextField = new TextField(TextFieldLeftPosition, UiConstants.StandardInset + AuthorTextField.Bottom);
        MusicTextField.Width = TextFieldWidth;
        MusicTextField.SetCapacity(100);

        var musicLabel = new TextLabel(UiConstants.StandardInset, MusicTextField.Top, "Music", UiConstants.AllBlackColours);
        musicLabel.LabelOffsetY = TitleTextField.TextYOffset;

        var sizeLabel = new TextLabel(UiConstants.StandardInset, UiConstants.TwiceStandardInset + MusicTextField.Bottom, "Size", UiConstants.AllBlackColours);
        var wLabel = new TextLabel(TextFieldLeftPosition, sizeLabel.Top, "W", UiConstants.AllBlackColours);
        LevelWidthTextField = new TextField(wLabel.Left + 4 + UiConstants.TwiceStandardInset, sizeLabel.Top);
        LevelWidthTextField.Width = TextFieldLeftPosition;
        LevelWidthTextField.SetCapacity(4);
        LevelWidthTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        var hLabel = new TextLabel(LevelWidthTextField.Right + UiConstants.TwiceStandardInset, sizeLabel.Top, "H", UiConstants.AllBlackColours);
        LevelHeightTextField = new TextField(hLabel.Left + 4 + UiConstants.TwiceStandardInset, sizeLabel.Top);
        LevelHeightTextField.Width = TextFieldLeftPosition;
        LevelHeightTextField.SetCapacity(4);
        LevelHeightTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        wLabel.LabelOffsetY = LevelWidthTextField.TextYOffset;
        sizeLabel.LabelOffsetY = LevelWidthTextField.TextYOffset;
        hLabel.LabelOffsetY = LevelWidthTextField.TextYOffset;

        var startLabel = new TextLabel(UiConstants.StandardInset, UiConstants.TwiceStandardInset + sizeLabel.Bottom, "Start", UiConstants.AllBlackColours);
        var xLabel = new TextLabel(TextFieldLeftPosition, startLabel.Top, "X", UiConstants.AllBlackColours);
        LevelXStartTextField = new TextField(xLabel.Left + 4 + UiConstants.TwiceStandardInset, startLabel.Top);
        LevelXStartTextField.Width = TextFieldLeftPosition;
        LevelXStartTextField.SetCapacity(4);
        LevelXStartTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        var yLabel = new TextLabel(LevelXStartTextField.Right + UiConstants.TwiceStandardInset, startLabel.Top, "Y", UiConstants.AllBlackColours);
        LevelYStartTextField = new TextField(yLabel.Left + 4 + UiConstants.TwiceStandardInset, startLabel.Top);
        LevelYStartTextField.Width = TextFieldLeftPosition;
        LevelYStartTextField.SetCapacity(4);
        LevelYStartTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        xLabel.LabelOffsetY = LevelXStartTextField.TextYOffset;
        startLabel.LabelOffsetY = LevelXStartTextField.TextYOffset;
        yLabel.LabelOffsetY = LevelXStartTextField.TextYOffset;

        var idLabel = new TextLabel(UiConstants.StandardInset, UiConstants.TwiceStandardInset + LevelYStartTextField.Bottom, "ID", UiConstants.AllBlackColours);
        LevelIdTextField = new TextField(TextFieldLeftPosition, idLabel.Top);
        LevelIdTextField.Width = TextFieldWidth;
        LevelIdTextField.SetCapacity(16);
        LevelIdTextField.AutoCapitaliseLetters = true;
        LevelIdTextField.SetTextMask(UiConstants.HexdecimalTextFieldMask);
        idLabel.LabelOffsetY = LevelIdTextField.TextYOffset;

        AddComponent(TitleTextField);
        AddComponent(titleLabel);
        AddComponent(AuthorTextField);
        AddComponent(authorLabel);
        AddComponent(MusicTextField);
        AddComponent(musicLabel);

        AddComponent(LevelWidthTextField);
        AddComponent(LevelHeightTextField);
        AddComponent(sizeLabel);
        AddComponent(wLabel);
        AddComponent(hLabel);

        AddComponent(LevelXStartTextField);
        AddComponent(LevelYStartTextField);
        AddComponent(startLabel);
        AddComponent(xLabel);
        AddComponent(yLabel);

        AddComponent(LevelIdTextField);
        AddComponent(idLabel);
    }

    [SkipLocalsInit]
    public void SetLevelData(LevelData levelData)
    {
        TitleTextField.SetText(levelData.LevelTitle);
        AuthorTextField.SetText(levelData.LevelAuthor);
        //MusicTextField.SetText(levelData.Music);

        var levelDimensions = levelData.LevelDimensions;

        Span<char> numberBuffer = stackalloc char[16];

        levelDimensions.W.TryFormat(numberBuffer, out var charsWritten);
        LevelWidthTextField.SetText(Helpers.Slice(numberBuffer, 0, charsWritten));

        levelDimensions.H.TryFormat(numberBuffer, out charsWritten);
        LevelHeightTextField.SetText(Helpers.Slice(numberBuffer, 0, charsWritten));

        levelData.LevelId.LevelId.TryFormat(numberBuffer, out var idWritten, "X16");
        LevelIdTextField.SetText(numberBuffer);
    }
}
