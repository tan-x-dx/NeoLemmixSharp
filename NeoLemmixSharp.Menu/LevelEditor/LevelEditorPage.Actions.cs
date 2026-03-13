using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed partial class LevelEditorPage
{
    private MenuBarButtonHandler GetMenuButtonHandler() => new()
    {
        FileNewAction = OnNewLevel,
        FileOpenAction = OnFileOpen,
        FileSaveAction = OnSaveLevel,
        FileSaveAsAction = OnSaveLevelAs,
        FileExitAction = OnExit,

        EditorUndoAction = EditorUndo,
        EditorRedoAction = EditorRedo,
        EditorCutAction = EditorCut,
        EditorCopyAction = EditorCopy,
        EditorPasteAction = EditorPaste,
        EditorPasteInPlaceAction = EditorPasteInPlace,
        EditorDuplicateAction = EditorDuplicate,
        EditorGroupAction = EditorGroup,
        EditorUngroupAction = EditorUngroup,
    };

    private void OnNewLevel(Component c, Point position)
    {
        SetLevelData(CreateBlankLevelData());
    }

    private void OnFileOpen(Component c, Point position)
    {
    }

    private void OnSaveLevel(Component c, Point position)
    {
        SaveLevel($@"C:\Temp\{_currentLevelData.LevelTitle}.ullv");
    }

    private void OnSaveLevelAs(Component c, Point position)
    {
    }

    private void OnExit(Component c, Point position)
    {
        var mainPage = MenuScreen.Instance.MenuPageCreator.CreateMainPage();

        if (mainPage is null)
            return;

        MenuScreen.Instance.SetNextPage(mainPage);
    }

    private void EditorUndo(Component c, Point position)
    {
    }

    private void EditorRedo(Component c, Point position)
    {
    }

    private void EditorCut(Component c, Point position)
    {
    }

    private void EditorCopy(Component c, Point position)
    {
    }

    private void EditorPaste(Component c, Point position)
    {
    }

    private void EditorPasteInPlace(Component c, Point position)
    {
    }

    private void EditorDuplicate(Component c, Point position)
    {
    }

    private void EditorGroup(Component c, Point position)
    {
    }

    private void EditorUngroup(Component c, Point position)
    {
    }
}
