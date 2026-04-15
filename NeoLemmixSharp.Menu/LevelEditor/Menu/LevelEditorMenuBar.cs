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

    private readonly Dictionary<Component, PopupMenu> _buttonMenuMapping;

    private IEditorOperationHandler _buttonHandler;

    public LevelEditorMenuBar(IEditorOperationHandler buttonHandler)
    {
        _buttonHandler = buttonHandler;
        _buttonMenuMapping = new Dictionary<Component, PopupMenu>(5);

        Height = LevelEditorMenuBarHeight;
        Colors = LighterRectangularButtonColors;

        var fileButton = new Button(StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        fileButton.AddTextLabel("File", TextLabelXOffset, TextLabelYOffset, AllWhiteColors);

        var editButton = new Button(fileButton.Right + StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        editButton.AddTextLabel("Edit", TextLabelXOffset, TextLabelYOffset, AllWhiteColors);

        var viewButton = new Button(editButton.Right + StandardInset, StandardInset, 64, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        viewButton.AddTextLabel("View", TextLabelXOffset, TextLabelYOffset, AllWhiteColors);

        var toolsButton = new Button(viewButton.Right + StandardInset, StandardInset, 72, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        toolsButton.AddTextLabel("Tools", TextLabelXOffset, TextLabelYOffset, AllWhiteColors);

        var optionsButton = new Button(toolsButton.Right + StandardInset, StandardInset, 84, MenuBarButtonHeight)
        {
            Colors = MenuButtonColors
        };
        optionsButton.AddTextLabel("Options", TextLabelXOffset, TextLabelYOffset, AllWhiteColors);

        AddChild(fileButton);
        AddChild(editButton);
        AddChild(viewButton);
        AddChild(toolsButton);
        AddChild(optionsButton);

        fileButton.MousePressed.RegisterMousePressEvent(OpenPopupMenu, MouseButtonType.Left);
        editButton.MousePressed.RegisterMousePressEvent(OpenPopupMenu, MouseButtonType.Left);
        viewButton.MousePressed.RegisterMousePressEvent(OpenPopupMenu, MouseButtonType.Left);
        toolsButton.MousePressed.RegisterMousePressEvent(OpenPopupMenu, MouseButtonType.Left);
        optionsButton.MousePressed.RegisterMousePressEvent(OpenPopupMenu, MouseButtonType.Left);

        CreatePopupMenu(
            fileButton,
            new ButtonDefinition("New (Ctrl + N)", _buttonHandler.OnNewLevel),
            new ButtonDefinition("Open (Ctrl + O)", _buttonHandler.OnFileOpen),
            new ButtonDefinition("Save (Ctrl + S)", _buttonHandler.OnSaveLevel),
            new ButtonDefinition("Save As (Ctrl + Shift + S)", _buttonHandler.OnSaveLevelAs),
            new ButtonDefinition("Exit (Esc)", _buttonHandler.OnExit));

        CreatePopupMenu(
            editButton,
            new ButtonDefinition("Undo (Ctrl + Z)", _buttonHandler.EditorUndo),
            new ButtonDefinition("Redo (Ctrl + Y)", _buttonHandler.EditorRedo),
            new ButtonDefinition("Cut (Ctrl + X)", _buttonHandler.EditorCut),
            new ButtonDefinition("Copy (Ctrl + C)", _buttonHandler.EditorCopy),
            new ButtonDefinition("Paste (Ctrl + V)", _buttonHandler.EditorPaste),
            new ButtonDefinition("Paste In Place (Ctrl + Shift + V)", _buttonHandler.EditorPasteInPlace),
            new ButtonDefinition("Duplicate (C)", _buttonHandler.EditorDuplicate),
            new ButtonDefinition("Group (G)", _buttonHandler.EditorGroup),
            new ButtonDefinition("Ungroup (H)", _buttonHandler.EditorUngroup));

        CreatePopupMenu(
            viewButton,
            new ToggleButtonDefinition("Clear Physics (F1)", _buttonHandler.ToggleClearPhysics),
            new ToggleButtonDefinition("Terrain Rendering (F2)", _buttonHandler.ToggleTerrainRendering),
            new ToggleButtonDefinition("Gadget Rendering (F3)", _buttonHandler.ToggleGadgetRendering),
            new ToggleButtonDefinition("Trigger Areas (F4)", _buttonHandler.ToggleTriggerAreaRendering),
            new ToggleButtonDefinition("Screen Start (F5)", _buttonHandler.ToggleScreenStartRendering),
            new ToggleButtonDefinition("Background Image (F6)", _buttonHandler.ToggleBackgroundRendering),
            new ToggleButtonDefinition("Deprecated Pieces (F7)", _buttonHandler.ToggleDeprecatedPieces));

        CreatePopupMenu(
            toolsButton,
            new ToggleButtonDefinition("Snap To Grid (F9)", _buttonHandler.ToggleSnapToGrid),
            new ButtonDefinition("Test Level (F12)", _buttonHandler.TestLevel),
            new ButtonDefinition("Validate Level", _buttonHandler.ValidateLevel));

        CreatePopupMenu(
            optionsButton,
            new ButtonDefinition("Settings (F10)", _buttonHandler.ViewSettings),
            new ButtonDefinition("Hotkeys (F11)", _buttonHandler.ViewHotKeySettings),
            new ButtonDefinition("About...", _buttonHandler.ViewAbout));
    }

    private void CreatePopupMenu(Component c, params IButtonDefinition[] buttonDefinitions)
    {
        var result = new PopupMenu()
        {
            Left = c.Left,
            Top = c.Bottom + 1,
            Width = PopupMenuWidth,

            DisposeOnClose = false
        };

        var y = 0;
        foreach (var buttonDefinition in buttonDefinitions)
        {
            buttonDefinition.CreateButton(result, y, ClosePopupMenu);

            y += StandardButtonHeight;
        }

        result.Height = y;

        _buttonMenuMapping.Add(c, result);

        return;
    }

    private void OpenPopupMenu(Component c, Common.Point position)
    {
        var menu = _buttonMenuMapping[c];

        UiHandler.Instance.OpenPopupMenu(menu);
    }

    private void ClosePopupMenu(Component c, Common.Point position)
    {
        var menu = (PopupMenu)c.GetTopParent();

        UiHandler.Instance.ClosePopupMenu(menu);
    }

    protected override void OnDispose()
    {
        _buttonHandler = null!;
        _buttonMenuMapping.Clear();
    }

    private interface IButtonDefinition
    {
        void CreateButton(PopupMenu popupMenu, int y, MousePressEventHandler.ComponentMousePressAction closePopupMenu);
    }

    private sealed class ButtonDefinition : IButtonDefinition
    {
        public string ButtonLabel { get; }
        public MousePressEventHandler.ComponentMousePressAction ButtonAction { get; }

        public ButtonDefinition(string buttonLabel, MousePressEventHandler.ComponentMousePressAction buttonAction)
        {
            ButtonLabel = buttonLabel;
            ButtonAction = buttonAction;
        }

        public void CreateButton(PopupMenu popupMenu, int y, MousePressEventHandler.ComponentMousePressAction closePopupMenu)
        {
            var button = new Button(0, y, PopupMenuWidth, StandardButtonHeight)
            {
                Colors = MenuButtonColors
            };

            button.MousePressed.RegisterMousePressEvent(ButtonAction, MouseButtonType.Left);
            button.MousePressed.RegisterMousePressEvent(closePopupMenu, MouseButtonType.Left);

            button.AddTextLabel(ButtonLabel, TextLabelXOffset, 8, AllWhiteColors);

            popupMenu.AddChild(button);
        }
    }

    private sealed class ToggleButtonDefinition : IButtonDefinition
    {
        public string ButtonLabel { get; }
        public GenericEventHandler.ComponentAction CheckBoxToggleAction { get; }

        public ToggleButtonDefinition(string buttonLabel, GenericEventHandler.ComponentAction checkBoxOnCheckedAction)
        {
            ButtonLabel = buttonLabel;
            CheckBoxToggleAction = checkBoxOnCheckedAction;
        }

        public void CreateButton(PopupMenu popupMenu, int y, MousePressEventHandler.ComponentMousePressAction closePopupMenu)
        {
            var button = new Button(0, y, PopupMenuWidth, StandardButtonHeight)
            {
                Colors = MenuButtonColors
            };

            button.MousePressed.RegisterMousePressEvent(closePopupMenu, MouseButtonType.Left);

            var checkBox = CreateMenuCheckBox(button);

            button.AddTextLabel(ButtonLabel, TextLabelXOffset + StandardButtonHeight + TwiceStandardInset, 8, AllWhiteColors);

            popupMenu.AddChild(checkBox);
            popupMenu.AddChild(button);
        }

        private CheckBox CreateMenuCheckBox(Button button)
        {
            var checkBox = new CheckBox
            {
                Left = button.Left + StandardInset,
                Top = button.Top,
                Colors = MenuButtonColors,

                CollisionBehaviour = new ProxyCollision(button)
            };

            checkBox.OnChecked.RegisterEvent(CheckBoxToggleAction);
            checkBox.OnUnchecked.RegisterEvent(CheckBoxToggleAction);

            return checkBox;
        }
    }

    private sealed class ProxyCollision : IMouseCollision
    {
        private readonly Component _proxyComponent;

        public ProxyCollision(Component proxyComponent)
        {
            _proxyComponent = proxyComponent;
        }

        public bool ContainsPoint(Component c, Common.Point position)
        {
            return _proxyComponent.ContainsPoint(position);
        }
    }
}
