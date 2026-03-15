using Microsoft.Xna.Framework;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;
using NeoLemmixSharp.Ui.Events;
using static NeoLemmixSharp.Ui.Data.UiConstants;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu;

public sealed class LevelEditorMenuBar : Component
{
    private const int MenuBarButtonHeight = 40;
    private const int LevelEditorMenuBarHeight = MenuBarButtonHeight + TwiceStandardInset;

    private const int TextLabelXOffset = 16;
    private const int TextLabelYOffset = 12;

    private const int PopupMenuWidth = 256;

    private static ColorPacket MenuButtonColors => new
    (
        Color.Transparent,
        Color.CornflowerBlue,
        Color.LightGray,
        Color.CornflowerBlue
    );

    private IEditorOperationHandler _buttonHandler;

    public LevelEditorMenuBar(IEditorOperationHandler buttonHandler)
    {
        _buttonHandler = buttonHandler;
        Height = LevelEditorMenuBarHeight;
        Colors = LighterRectangularButtonColours;

        var fileButton = new Button(StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var fileTextLabel = new TextLabel("File")
        {
            Left = fileButton.Left + TextLabelXOffset,
            Top = fileButton.Top + TextLabelYOffset,
            Colors = new ColorPacket(Color.White)
        };

        var editButton = new Button(fileButton.Right + StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var editTextLabel = new TextLabel("Edit")
        {
            Left = editButton.Left + TextLabelXOffset,
            Top = editButton.Top + TextLabelYOffset,
            Colors = new ColorPacket(Color.White)
        };

        var viewButton = new Button(editButton.Right + StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var viewTextLabel = new TextLabel("View")
        {
            Left = viewButton.Left + TextLabelXOffset,
            Top = viewButton.Top + TextLabelYOffset,
            Colors = new ColorPacket(Color.White)
        };

        var toolsButton = new Button(viewButton.Right + StandardInset, StandardInset, 72, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var toolsTextLabel = new TextLabel("Tools")
        {
            Left = toolsButton.Left + TextLabelXOffset,
            Top = toolsButton.Top + TextLabelYOffset,
            Colors = new ColorPacket(Color.White)
        };

        var optionsButton = new Button(toolsButton.Right + StandardInset, StandardInset, 84, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var optionsTextLabel = new TextLabel("Options")
        {
            Left = optionsButton.Left + TextLabelXOffset,
            Top = optionsButton.Top + TextLabelYOffset,
            Colors = new ColorPacket(Color.White)
        };

        AddChild(fileButton);
        AddChild(fileTextLabel);
        AddChild(editButton);
        AddChild(editTextLabel);
        AddChild(viewButton);
        AddChild(viewTextLabel);
        AddChild(toolsButton);
        AddChild(toolsTextLabel);
        AddChild(optionsButton);
        AddChild(optionsTextLabel);

        fileButton.MousePressed.RegisterMousePressEvent(OnFileButtonPress, MouseButtonType.Left);
        editButton.MousePressed.RegisterMousePressEvent(OnEditButtonPress, MouseButtonType.Left);
        viewButton.MousePressed.RegisterMousePressEvent(OnViewButtonPress, MouseButtonType.Left);
        toolsButton.MousePressed.RegisterMousePressEvent(OnToolsButtonPress, MouseButtonType.Left);
        optionsButton.MousePressed.RegisterMousePressEvent(OnOptionsButtonPress, MouseButtonType.Left);
    }

    private void OnFileButtonPress(Component c, Common.Point position)
    {
        var fileMenu = CreatePopupMenu(
            c,
            new ButtonDefinition("New (Ctrl + N)", _buttonHandler.OnNewLevel),
            new ButtonDefinition("Open (Ctrl + O)", _buttonHandler.OnFileOpen),
            new ButtonDefinition("Save (Ctrl + S)", _buttonHandler.OnSaveLevel),
            new ButtonDefinition("Save As (Ctrl + Shift + S)", _buttonHandler.OnSaveLevelAs),
            new ButtonDefinition("Exit (Esc)", _buttonHandler.OnExit));

        UiHandler.Instance.OpenPopupMenu(fileMenu);
    }

    private void OnEditButtonPress(Component c, Common.Point position)
    {
        var editMenu = CreatePopupMenu(
            c,
            new ButtonDefinition("Undo (Ctrl + Z)", _buttonHandler.EditorUndo),
            new ButtonDefinition("Redo (Ctrl + Y)", _buttonHandler.EditorRedo),
            new ButtonDefinition("Cut (Ctrl + X)", _buttonHandler.EditorCut),
            new ButtonDefinition("Copy (Ctrl + C)", _buttonHandler.EditorCopy),
            new ButtonDefinition("Paste (Ctrl + V)", _buttonHandler.EditorPaste),
            new ButtonDefinition("Paste In Place (Ctrl + Shift + V)", _buttonHandler.EditorPasteInPlace),
            new ButtonDefinition("Duplicate (C)", _buttonHandler.EditorDuplicate),
            new ButtonDefinition("Group (G)", _buttonHandler.EditorGroup),
            new ButtonDefinition("Ungroup (H)", _buttonHandler.EditorUngroup));

        UiHandler.Instance.OpenPopupMenu(editMenu);
    }

    private void OnViewButtonPress(Component c, Common.Point position)
    {
        var viewMenu = CreatePopupMenu(
            c);

        UiHandler.Instance.OpenPopupMenu(viewMenu);
    }

    private void OnToolsButtonPress(Component c, Common.Point position)
    {
        var toolsMenu = CreatePopupMenu(
            c);

        UiHandler.Instance.OpenPopupMenu(toolsMenu);
    }

    private void OnOptionsButtonPress(Component c, Common.Point position)
    {
        var optionsMenu = CreatePopupMenu(
            c);

        UiHandler.Instance.OpenPopupMenu(optionsMenu);
    }

    private static PopupMenu CreatePopupMenu(Component c, params ButtonDefinition[] buttonDefinitions)
    {
        var result = new PopupMenu()
        {
            Left = c.Left,
            Top = c.Bottom + 1,
            Width = PopupMenuWidth
        };

        var y = 0;
        foreach (var buttonDefinition in buttonDefinitions)
        {
            var button = new Button(0, y, PopupMenuWidth, StandardButtonHeight)
            {
                Colors = MenuButtonColors
            };

            button.MousePressed.RegisterMousePressEvent(buttonDefinition.ButtonAction, MouseButtonType.Left);
            button.MousePressed.RegisterMousePressEvent(ClosePopupMenu, MouseButtonType.Left);

            var buttonLabel = new TextLabel(buttonDefinition.ButtonLabel)
            {
                Left = button.Left + TextLabelXOffset,
                Top = button.Top + 8,
                Colors = new ColorPacket(Color.White)
            };

            y += StandardButtonHeight;
            result.AddChild(button);
            result.AddChild(buttonLabel);
        }

        result.Height = y;

        return result;
    }

    private readonly record struct ButtonDefinition(string ButtonLabel, MousePressEventHandler.ComponentMousePressAction ButtonAction);

    private static void ClosePopupMenu(Component c, Common.Point position)
    {
        UiHandler.Instance.ClosePopupMenu();
    }

    protected override void OnDispose()
    {
        _buttonHandler = null!;
    }
}
