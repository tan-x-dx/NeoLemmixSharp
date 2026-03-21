using Microsoft.Xna.Framework;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;
using NeoLemmixSharp.Ui.Components.Util;
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
        Colors = LighterRectangularButtonColors;

        var fileButton = new Button(StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var fileTextLabel = new TextLabel("File")
        {
            Left = fileButton.Left + TextLabelXOffset,
            Top = fileButton.Top + TextLabelYOffset,
            Colors = AllWhiteColors
        };

        var editButton = new Button(fileButton.Right + StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var editTextLabel = new TextLabel("Edit")
        {
            Left = editButton.Left + TextLabelXOffset,
            Top = editButton.Top + TextLabelYOffset,
            Colors = AllWhiteColors
        };

        var viewButton = new Button(editButton.Right + StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var viewTextLabel = new TextLabel("View")
        {
            Left = viewButton.Left + TextLabelXOffset,
            Top = viewButton.Top + TextLabelYOffset,
            Colors = AllWhiteColors
        };

        var toolsButton = new Button(viewButton.Right + StandardInset, StandardInset, 72, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var toolsTextLabel = new TextLabel("Tools")
        {
            Left = toolsButton.Left + TextLabelXOffset,
            Top = toolsButton.Top + TextLabelYOffset,
            Colors = AllWhiteColors
        };

        var optionsButton = new Button(toolsButton.Right + StandardInset, StandardInset, 84, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        var optionsTextLabel = new TextLabel("Options")
        {
            Left = optionsButton.Left + TextLabelXOffset,
            Top = optionsButton.Top + TextLabelYOffset,
            Colors = AllWhiteColors
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
            c,
            new ButtonDefinition("Clear Physics (F1)", _buttonHandler.ToggleClearPhysics, true),
            new ButtonDefinition("Terrain Rendering (F2)", _buttonHandler.ToggleTerrainRendering, true),
            new ButtonDefinition("Gadget Rendering (F3)", _buttonHandler.ToggleGadgetRendering, true),
            new ButtonDefinition("Trigger Areas (F4)", _buttonHandler.ToggleTriggerAreaRendering, true),
            new ButtonDefinition("Screen Start (F5)", _buttonHandler.ToggleScreenStartRendering, true),
            new ButtonDefinition("Background Image (F6)", _buttonHandler.ToggleBackgroundRendering, true),
            new ButtonDefinition("Deprecated Pieces (F7)", _buttonHandler.ToggleDeprecatedPieces, true));

        UiHandler.Instance.OpenPopupMenu(viewMenu);
    }

    private void OnToolsButtonPress(Component c, Common.Point position)
    {
        var toolsMenu = CreatePopupMenu(
            c,
            new ButtonDefinition("Snap To Grid (F9)", _buttonHandler.ToggleSnapToGrid),
            new ButtonDefinition("Test Level (F12)", _buttonHandler.TestLevel),
            new ButtonDefinition("Validate Level", _buttonHandler.ValidateLevel));

        UiHandler.Instance.OpenPopupMenu(toolsMenu);
    }

    private void OnOptionsButtonPress(Component c, Common.Point position)
    {
        var optionsMenu = CreatePopupMenu(
            c,
            new ButtonDefinition("Settings (F10)", _buttonHandler.ViewSettings),
            new ButtonDefinition("Hotkeys (F11)", _buttonHandler.ViewHotKeySettings),
            new ButtonDefinition("About...", _buttonHandler.ViewAbout));

        UiHandler.Instance.OpenPopupMenu(optionsMenu);
    }

    private static PopupMenu CreatePopupMenu(Component c, params ReadOnlySpan<ButtonDefinition> buttonDefinitions)
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

            var xOffset = 0;
            CheckBox? checkBox = null;

            if (buttonDefinition.includeCheckBox)
            {
                xOffset = StandardButtonHeight + TwiceStandardInset;
                checkBox = new CheckBox
                {
                    Left = button.Left + StandardInset,
                    Top = button.Top,
                    Colors = MenuButtonColors,

                    CollisionBehaviour = IContainMousePosition.NoCollisionInstance
                };
            }

            var buttonLabel = new TextLabel(buttonDefinition.ButtonLabel)
            {
                Left = button.Left + TextLabelXOffset + xOffset,
                Top = button.Top + 8,
                Colors = AllWhiteColors
            };

            y += StandardButtonHeight;

            if (checkBox is not null)
                result.AddChild(checkBox);
            result.AddChild(button);
            result.AddChild(buttonLabel);
        }

        result.Height = y;

        return result;
    }

    private readonly record struct ButtonDefinition(string ButtonLabel, MousePressEventHandler.ComponentMousePressAction ButtonAction, bool includeCheckBox = false);

    private static void ClosePopupMenu(Component c, Common.Point position)
    {
        UiHandler.Instance.ClosePopupMenu();
    }

    protected override void OnDispose()
    {
        _buttonHandler = null!;
    }
}
