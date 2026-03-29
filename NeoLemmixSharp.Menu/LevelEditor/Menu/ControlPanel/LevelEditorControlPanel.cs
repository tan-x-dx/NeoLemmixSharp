using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Components.Buttons;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.LevelEditor.Menu.ControlPanel;

public sealed class LevelEditorControlPanel : Component
{
    public const int LevelControlPanelWidth = 280;

    private readonly ButtonGroup _tabGroup;

    public LevelGlobalsControlPanelTab LevelGlobalsTab { get; }
    public LevelPiecesControlPanelTab LevelPiecesTab { get; }
    public LevelSkillsControlPanelTab LevelSkillsTab { get; }
    public LevelMiscControlPanelTab LevelMiscTab { get; }

    public LevelEditorControlPanel()
    {
        Width = LevelControlPanelWidth;
        Colors = UiConstants.LighterRectangularButtonColors;

        _tabGroup = new ButtonGroup(4);
        var tabButtonsSpan = _tabGroup.Buttons;

        var levelGlobalsButton = tabButtonsSpan.At(0);
        levelGlobalsButton.Left = 0;
        levelGlobalsButton.Top = UiConstants.StandardInset;
        levelGlobalsButton.Width = 80;
        levelGlobalsButton.Height = UiConstants.StandardButtonHeight;
        levelGlobalsButton.Colors = UiConstants.LighterRectangularButtonColors;
        levelGlobalsButton.MousePressed.RegisterMousePressEvent(OnSelectLevelGlobalsTab, MouseButtonType.Left);
        var levelGlobalsTextLabel = new TextLabel("Globals")
        {
            Left = UiConstants.StandardInset + levelGlobalsButton.Left,
            Top = UiConstants.StandardInset,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColors
        };
        LevelGlobalsTab = new LevelGlobalsControlPanelTab()
        {
            Top = levelGlobalsButton.Bottom
        };
        AddChild(levelGlobalsButton);
        AddChild(levelGlobalsTextLabel);
        AddChild(LevelGlobalsTab);


        var levelPiecesButton = tabButtonsSpan.At(1);
        levelPiecesButton.Left = levelGlobalsButton.Right;
        levelPiecesButton.Top = UiConstants.StandardInset;
        levelPiecesButton.Width = 70;
        levelPiecesButton.Height = UiConstants.StandardButtonHeight;
        levelPiecesButton.Colors = UiConstants.LighterRectangularButtonColors;
        levelPiecesButton.MousePressed.RegisterMousePressEvent(OnSelectLevelPiecesTab, MouseButtonType.Left);
        var levelPiecesTextLabel = new TextLabel("Pieces")
        {
            Left = UiConstants.StandardInset + levelPiecesButton.Left,
            Top = UiConstants.StandardInset,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColors
        };
        LevelPiecesTab = new LevelPiecesControlPanelTab()
        {
            Top = levelPiecesButton.Bottom
        };
        AddChild(levelPiecesButton);
        AddChild(levelPiecesTextLabel);
        AddChild(LevelPiecesTab);


        var levelSkillsButton = tabButtonsSpan.At(2);
        levelSkillsButton.Left = levelPiecesButton.Right;
        levelSkillsButton.Top = UiConstants.StandardInset;
        levelSkillsButton.Width = 60;
        levelSkillsButton.Height = UiConstants.StandardButtonHeight;
        levelSkillsButton.Colors = UiConstants.LighterRectangularButtonColors;
        levelSkillsButton.MousePressed.RegisterMousePressEvent(OnSelectLevelSkillsTab, MouseButtonType.Left);
        var levelSkillsTextLabel = new TextLabel("Skills")
        {
            Left = UiConstants.StandardInset + levelSkillsButton.Left,
            Top = UiConstants.StandardInset,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColors
        };
        LevelSkillsTab = new LevelSkillsControlPanelTab()
        {
            Top = levelSkillsButton.Bottom
        };
        AddChild(levelSkillsButton);
        AddChild(levelSkillsTextLabel);
        AddChild(LevelSkillsTab);


        var levelMiscButton = tabButtonsSpan.At(3);
        levelMiscButton.Left = levelSkillsButton.Right;
        levelMiscButton.Top = UiConstants.StandardInset;
        levelMiscButton.Width = 60;
        levelMiscButton.Height = UiConstants.StandardButtonHeight;
        levelMiscButton.Colors = UiConstants.LighterRectangularButtonColors;
        levelMiscButton.MousePressed.RegisterMousePressEvent(OnSelectLevelMiscTab, MouseButtonType.Left);
        var levelMiscTextLabel = new TextLabel("Misc")
        {
            Left = UiConstants.StandardInset + levelMiscButton.Left,
            Top = UiConstants.StandardInset,
            LabelOffsetY = UiConstants.DefaultTextYOffset,
            Colors = UiConstants.AllBlackColors
        };
        LevelMiscTab = new LevelMiscControlPanelTab()
        {
            Top = levelMiscButton.Bottom
        };
        AddChild(levelMiscButton);
        AddChild(levelMiscTextLabel);
        AddChild(LevelMiscTab);

        SetSelectedTab(LevelGlobalsTab);
    }

    private void OnSelectLevelGlobalsTab(Component c, Point position)
    {
        SetSelectedTab(LevelGlobalsTab);
    }

    private void OnSelectLevelPiecesTab(Component c, Point position)
    {
        SetSelectedTab(LevelPiecesTab);
    }

    private void OnSelectLevelSkillsTab(Component c, Point position)
    {
        SetSelectedTab(LevelSkillsTab);
    }

    private void OnSelectLevelMiscTab(Component c, Point position)
    {
        SetSelectedTab(LevelMiscTab);
    }

    private void SetSelectedTab(Component c)
    {
        Component tab = LevelGlobalsTab;
        tab.IsVisible = tab == c;

        tab = LevelPiecesTab;
        tab.IsVisible = tab == c;

        tab = LevelSkillsTab;
        tab.IsVisible = tab == c;

        tab = LevelMiscTab;
        tab.IsVisible = tab == c;
    }

    public void SetLevelData(LevelData levelData)
    {
        LevelGlobalsTab.SetLevelData(levelData);
    }

    public void SetNumericalLevelData(LevelData levelData)
    {
        LevelGlobalsTab.SetNumericalLevelData(levelData);
    }

    protected override void OnDispose()
    {
        _tabGroup.Dispose();
    }

    public void OnResize()
    {
        var y = Height + Top;

        LevelGlobalsTab.Width = Width;
        LevelGlobalsTab.Height = y - LevelGlobalsTab.Top;
        LevelPiecesTab.Width = Width;
        LevelPiecesTab.Height = y - LevelPiecesTab.Top;
        LevelSkillsTab.Width = Width;
        LevelSkillsTab.Height = y - LevelSkillsTab.Top;
        LevelMiscTab.Width = Width;
        LevelMiscTab.Height = y - LevelMiscTab.Top;
    }
}
