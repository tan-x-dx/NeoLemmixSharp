using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.UserSettings.Tabs;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.UserSettings;

public sealed class SettingsMenu : Component
{
    private const int SettingsMenuWidth = 800;
    private const int SettingsMenuHeight = 600;

    private static PopupMenu? _settingsMenu = null;
    private static SettingsMenu _instance = null!;

    public static PopupMenu GetMenu()
    {
        _settingsMenu ??= CreateSettingsMenu();

        return _settingsMenu;
    }

    private static PopupMenu CreateSettingsMenu()
    {
        var windowDimensions = IGameWindow.Instance.WindowSize;
        var menuX = (windowDimensions.W - SettingsMenuWidth) >> 1;
        var menuY = (windowDimensions.H - SettingsMenuHeight) >> 1;

        var menu = new PopupMenu
        {
            Left = menuX,
            Top = menuY,

            Width = SettingsMenuWidth,
            Height = SettingsMenuHeight,

            DisposeOnClose = false
        };

        _instance = new SettingsMenu();

        menu.AddChild(_instance);

        return menu;
    }

    private readonly ButtonGroup _tabGroup;

    public GeneralSettingsTab GeneralSettingsTab { get; }
    public GameplaySettingsTab GameplaySettingsTab { get; }

    private SettingsMenu()
    {
        Width = SettingsMenuWidth;
        Height = SettingsMenuHeight;

        Colors = UiConstants.LighterRectangularButtonColors;

        _tabGroup = new ButtonGroup(4);
        var tabButtonsSpan = _tabGroup.Buttons;

        var generalSettingsButton = tabButtonsSpan.At((int)SettingsTabType.GeneralSettings);
        generalSettingsButton.Left = 0;
        generalSettingsButton.Top = UiConstants.StandardInset;
        generalSettingsButton.Width = 80;
        generalSettingsButton.Height = UiConstants.StandardButtonHeight;
        generalSettingsButton.Colors = UiConstants.LighterRectangularButtonColors;
        generalSettingsButton.MousePressed.RegisterMousePressEvent(OnSelectGeneralSettingsTabTab, MouseButtonType.Left);
        var levelGlobalsTextLabel = generalSettingsButton.AddTextLabel("General");
        levelGlobalsTextLabel.Colors = UiConstants.AllBlackColors;
        GeneralSettingsTab = new GeneralSettingsTab()
        {
            Top = generalSettingsButton.Bottom
        };
        AddChild(generalSettingsButton);
        AddChild(GeneralSettingsTab);


        var gameplaySettingsButton = tabButtonsSpan.At((int)SettingsTabType.GameplaySettings);
        gameplaySettingsButton.Left = generalSettingsButton.Right;
        gameplaySettingsButton.Top = UiConstants.StandardInset;
        gameplaySettingsButton.Width = 100;
        gameplaySettingsButton.Height = UiConstants.StandardButtonHeight;
        gameplaySettingsButton.Colors = UiConstants.LighterRectangularButtonColors;
        gameplaySettingsButton.MousePressed.RegisterMousePressEvent(OnSelectGameplaySettingsTabTab, MouseButtonType.Left);
        var levelPiecesTextLabel = gameplaySettingsButton.AddTextLabel("Gameplay");
        levelPiecesTextLabel.Colors = UiConstants.AllBlackColors;
        GameplaySettingsTab = new GameplaySettingsTab()
        {
            Top = gameplaySettingsButton.Bottom
        };
        AddChild(gameplaySettingsButton);
        AddChild(GameplaySettingsTab);
        /*

        var levelSkillsButton = tabButtonsSpan.At(2);
        levelSkillsButton.Left = levelPiecesButton.Right;
        levelSkillsButton.Top = UiConstants.StandardInset;
        levelSkillsButton.Width = 60;
        levelSkillsButton.Height = UiConstants.StandardButtonHeight;
        levelSkillsButton.Colors = UiConstants.LighterRectangularButtonColors;
        levelSkillsButton.MousePressed.RegisterMousePressEvent(OnSelectLevelSkillsTab, MouseButtonType.Left);
        var levelSkillsTextLabel = levelSkillsButton.AddTextLabel("Skills");
        levelSkillsTextLabel.Colors = UiConstants.AllBlackColors;
        LevelSkillsTab = new LevelSkillsControlPanelTab()
        {
            Top = levelSkillsButton.Bottom
        };
        AddChild(levelSkillsButton);
        AddChild(LevelSkillsTab);


        var levelMiscButton = tabButtonsSpan.At(3);
        levelMiscButton.Left = levelSkillsButton.Right;
        levelMiscButton.Top = UiConstants.StandardInset;
        levelMiscButton.Width = 60;
        levelMiscButton.Height = UiConstants.StandardButtonHeight;
        levelMiscButton.Colors = UiConstants.LighterRectangularButtonColors;
        levelMiscButton.MousePressed.RegisterMousePressEvent(OnSelectLevelMiscTab, MouseButtonType.Left);
        var levelMiscTextLabel = levelMiscButton.AddTextLabel("Misc");
        levelMiscTextLabel.Colors = UiConstants.AllBlackColors;
        LevelMiscTab = new LevelMiscControlPanelTab()
        {
            Top = levelMiscButton.Bottom
        };
        AddChild(levelMiscButton);
        AddChild(LevelMiscTab);
        */

        SetSelectedTab(GeneralSettingsTab);
    }

    private void OnSelectGeneralSettingsTabTab(Component c, Point position)
    {
        SetSelectedTab(GeneralSettingsTab);
    }

    private void OnSelectGameplaySettingsTabTab(Component c, Point position)
    {
        SetSelectedTab(GameplaySettingsTab);
    }

    private void SetSelectedTab(Component c)
    {
        Component tab = GeneralSettingsTab;
        tab.IsVisible = tab == c;

        tab = GameplaySettingsTab;
        tab.IsVisible = tab == c;

        /*  tab = LevelSkillsTab;
          tab.IsVisible = tab == c;

          tab = LevelMiscTab;
          tab.IsVisible = tab == c;*/
    }

    public static void SelectTab(SettingsTabType tabType)
    {
        _instance._tabGroup.SetActive((int)tabType);
    }
}
