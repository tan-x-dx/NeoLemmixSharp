using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu.ControlPanel;

public sealed class LevelPiecesControlPanelTab : Component
{
    public Button RotatePieceButton { get; }
    public Button InvertPieceButton { get; }
    public Button FlipPieceButton { get; }

    public Button DrawLast { get; }
    public Button DrawLater { get; }
    public Button DrawSooner { get; }
    public Button DrawFirst { get; }

    public LevelPiecesControlPanelTab()
    {
        Colors = UiConstants.LighterRectangularButtonColors;

        RotatePieceButton = new Button(16, UiConstants.StandardInset, 72, 48)
        {
            Colors = UiConstants.LighterRectangularButtonColors
        };
        RotatePieceButton.AddTextLabel("Rotate", 12, 15, UiConstants.AllBlackColors);

        InvertPieceButton = new Button(RotatePieceButton.Right + UiConstants.TwiceStandardInset, UiConstants.StandardInset, 72, 48)
        {
            Colors = UiConstants.LighterRectangularButtonColors
        };
        InvertPieceButton.AddTextLabel("Invert", 14, 15, UiConstants.AllBlackColors);

        FlipPieceButton = new Button(InvertPieceButton.Right + UiConstants.TwiceStandardInset, UiConstants.StandardInset, 72, 48)
        {
            Colors = UiConstants.LighterRectangularButtonColors
        };
        FlipPieceButton.AddTextLabel("Flip", 18, 15, UiConstants.AllBlackColors);

        AddChild(RotatePieceButton);
        AddChild(InvertPieceButton);
        AddChild(FlipPieceButton);
    }

    public void SetLevelData(LevelData levelData)
    {
    }
}
