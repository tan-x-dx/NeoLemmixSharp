using NeoLemmixSharp.Common;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public interface IMenuBarButtonHandler
{
    void OnNewLevel(Component c, Point position);
    void OnFileOpen(Component c, Point position);
    void OnSaveLevel(Component c, Point position);
    void OnSaveLevelAs(Component c, Point position);
    void OnExit(Component c, Point position);

    void EditorUndo(Component c, Point position);
    void EditorRedo(Component c, Point position);
    void EditorCut(Component c, Point position);
    void EditorCopy(Component c, Point position);
    void EditorPaste(Component c, Point position);
    void EditorPasteInPlace(Component c, Point position);
    void EditorDuplicate(Component c, Point position);
    void EditorGroup(Component c, Point position);
    void EditorUngroup(Component c, Point position);
}
