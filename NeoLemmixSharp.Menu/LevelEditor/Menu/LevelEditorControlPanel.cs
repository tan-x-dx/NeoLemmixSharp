using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;
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
    public Button GenerateNewLevelIdButton { get; }

    public CheckBox WrapHorizontalCheckBox { get; }
    public CheckBox WrapVerticalCheckBox { get; }

    public LevelEditorControlPanel()
    {
        Width = LevelControlPanelWidth;
        Colors = UiConstants.LighterRectangularButtonColours;

        var y = UiConstants.StandardInset;

        TitleTextField = new TextField()
        {
            Left = TextFieldLeftPosition,
            Top = y,
            Width = TextFieldWidth
        };
        TitleTextField.SetCapacity(40);

        var titleLabel = new TextLabel("Title")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };

        y = UiConstants.StandardInset + TitleTextField.Bottom;

        AuthorTextField = new TextField()
        {
            Left = TextFieldLeftPosition,
            Top = y,
            Width = TextFieldWidth
        };
        AuthorTextField.SetCapacity(32);

        var authorLabel = new TextLabel("Author")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };

        y = UiConstants.StandardInset + AuthorTextField.Bottom;

        MusicTextField = new TextField()
        {
            Left = TextFieldLeftPosition,
            Top = y,
            Width = TextFieldWidth
        };
        MusicTextField.SetCapacity(100);

        var musicLabel = new TextLabel("Music")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };

        y = UiConstants.TwiceStandardInset + MusicTextField.Bottom;

        var sizeLabel = new TextLabel("Size")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        var wLabel = new TextLabel("W")
        {
            Left = TextFieldLeftPosition,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        LevelWidthTextField = new TextField()
        {
            Left = 4 + UiConstants.TwiceStandardInset + wLabel.Left,
            Top = y,
            Width = TextFieldLeftPosition
        };
        LevelWidthTextField.SetCapacity(4);
        LevelWidthTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        var hLabel = new TextLabel("H")
        {
            Left = UiConstants.TwiceStandardInset + LevelWidthTextField.Right,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        LevelHeightTextField = new TextField()
        {
            Left = 4 + UiConstants.TwiceStandardInset + hLabel.Left,
            Top = y,
            Width = TextFieldLeftPosition
        };
        LevelHeightTextField.SetCapacity(4);
        LevelHeightTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        y = UiConstants.TwiceStandardInset + sizeLabel.Bottom;

        var startLabel = new TextLabel("Start")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        var xLabel = new TextLabel("X")
        {
            Left = TextFieldLeftPosition,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        LevelXStartTextField = new TextField()
        {
            Left = 4 + UiConstants.TwiceStandardInset + xLabel.Left,
            Top = y,
            Width = TextFieldLeftPosition
        };
        LevelXStartTextField.SetCapacity(4);
        LevelXStartTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        var yLabel = new TextLabel("Y")
        {
            Left = UiConstants.TwiceStandardInset + LevelXStartTextField.Right,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        LevelYStartTextField = new TextField()
        {
            Left = 4 + UiConstants.TwiceStandardInset + yLabel.Left,
            Top = y,
            Width = TextFieldLeftPosition
        };
        LevelYStartTextField.SetCapacity(4);
        LevelYStartTextField.SetTextMask(UiConstants.NumericTextFieldMask);

        xLabel.LabelOffsetY = LevelXStartTextField.TextYOffset;
        startLabel.LabelOffsetY = LevelXStartTextField.TextYOffset;
        yLabel.LabelOffsetY = LevelXStartTextField.TextYOffset;

        y = UiConstants.TwiceStandardInset + LevelYStartTextField.Bottom;

        var idLabel = new TextLabel("ID")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        LevelIdTextField = new TextField
        {
            Left = TextFieldLeftPosition,
            Top = y,
            Width = TextFieldWidth,
            AutoCapitaliseLetters = true
        };
        LevelIdTextField.SetCapacity(16);
        LevelIdTextField.SetTextMask(UiConstants.HexdecimalTextFieldMask);

        y = UiConstants.StandardInset + LevelIdTextField.Bottom;

        GenerateNewLevelIdButton = new Button(0, y)
        {
            Left = TextFieldLeftPosition,
            Width = TextFieldWidth,
            Height = 36
        };
        var generateNewLevelIdLabel = new TextLabel(0, y, "Generate new ID")
        {
            Left = TextFieldLeftPosition,
            LabelOffsetX = 36,
            LabelOffsetY = 10,
            Colors = new ColorPacket(0xffbbbbbb.AsAbgrColor())
        };

        y = UiConstants.TwiceStandardInset + GenerateNewLevelIdButton.Bottom;

        var wrapHorizontalLabel = new TextLabel("Wrap Horizontal")
        {
            Left = UiConstants.StandardInset,
            Top = y,
            Width = TextFieldWidth,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        WrapHorizontalCheckBox = new CheckBox
        {
            Left = LevelWidthTextField.Left,
            Top = y
        };

        var wrapVerticalLabel = new TextLabel("Wrap Vertical")
        {
            Left = UiConstants.TwiceStandardInset + WrapHorizontalCheckBox.Right,
            Top = y,
            Width = TextFieldWidth,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColours
        };
        WrapVerticalCheckBox = new CheckBox
        {
            Left = LevelHeightTextField.Left,
            Top = y
        };

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
        AddComponent(GenerateNewLevelIdButton);
        AddComponent(generateNewLevelIdLabel);

        AddComponent(wrapHorizontalLabel);
        AddComponent(WrapHorizontalCheckBox);
        AddComponent(wrapVerticalLabel);
        AddComponent(WrapVerticalCheckBox);
    }

    public void SetLevelData(LevelData levelData)
    {
        TitleTextField.SetText(levelData.LevelTitle);
        AuthorTextField.SetText(levelData.LevelAuthor);
        //MusicTextField.SetText(levelData.Music);

        SetNumericalLevelData(levelData);

        var wrapHorizontal = levelData.HorizontalBoundaryBehaviour == Common.BoundaryBehaviours.BoundaryBehaviourType.Wrap;
        WrapHorizontalCheckBox.SetCheckedValue(wrapHorizontal);

        var wrapVertical = levelData.VerticalBoundaryBehaviour == Common.BoundaryBehaviours.BoundaryBehaviourType.Wrap;
        WrapVerticalCheckBox.SetCheckedValue(wrapVertical);
    }

    [SkipLocalsInit]
    public unsafe void SetNumericalLevelData(LevelData levelData)
    {
        char* numberBuffer = stackalloc char[16];

        var levelDimensions = levelData.LevelDimensions;

        uint uintValue = (uint)levelDimensions.W;
        var digitsWritten = NumberFormattingHelpers.WriteDigits(numberBuffer, uintValue);
        var span = Helpers.CreateReadOnlySpan<char>(numberBuffer, digitsWritten);
        LevelWidthTextField.SetText(span);

        uintValue = (uint)levelDimensions.H;
        digitsWritten = NumberFormattingHelpers.WriteDigits(numberBuffer, uintValue);
        span = Helpers.CreateReadOnlySpan<char>(numberBuffer, digitsWritten);
        LevelHeightTextField.SetText(span);

        ulong levelId = levelData.LevelId.LevelId;
        NumberFormattingHelpers.WriteHexDigits(numberBuffer, levelId);
        span = Helpers.CreateReadOnlySpan<char>(numberBuffer, 16);
        LevelIdTextField.SetText(span);
    }
}
