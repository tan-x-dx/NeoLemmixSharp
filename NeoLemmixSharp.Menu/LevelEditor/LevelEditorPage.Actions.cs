using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed partial class LevelEditorPage : IMenuBarButtonHandler
{
    public void OnNewLevel(Component c, Point position)
    {
        SetLevelData(CreateBlankLevelData());
    }

    public void OnFileOpen(Component c, Point position)
    {
    }

    public void OnSaveLevel(Component c, Point position)
    {
        if (string.IsNullOrWhiteSpace(_currentLevelData.LevelFilePath))
        {
            OnSaveLevelAs(c, position);
        }
        else
        {
            SaveLevel(_currentLevelData.LevelFilePath);
        }
    }

    public void OnSaveLevelAs(Component c, Point position)
    {
    }

    public void OnExit(Component c, Point position)
    {
        var mainPage = MenuScreen.Instance.MenuPageCreator.CreateMainPage();

        if (mainPage is null)
            return;

        MenuScreen.Instance.SetNextPage(mainPage);
    }

    public void EditorUndo(Component c, Point position)
    {
    }

    public void EditorRedo(Component c, Point position)
    {
    }

    public void EditorCut(Component c, Point position)
    {
    }

    public void EditorCopy(Component c, Point position)
    {
    }

    public void EditorPaste(Component c, Point position)
    {
    }

    public void EditorPasteInPlace(Component c, Point position)
    {
    }

    public void EditorDuplicate(Component c, Point position)
    {
    }

    public void EditorGroup(Component c, Point position)
    {
    }

    public void EditorUngroup(Component c, Point position)
    {
    }
}
