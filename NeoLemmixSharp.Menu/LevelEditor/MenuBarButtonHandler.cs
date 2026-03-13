using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class MenuBarButtonHandler
{
    public required MousePressEventHandler.ComponentMousePressAction FileNewAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction FileOpenAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction FileSaveAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction FileSaveAsAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction FileExitAction { get; init; }

    public required MousePressEventHandler.ComponentMousePressAction EditorUndoAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorRedoAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorCutAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorCopyAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorPasteAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorPasteInPlaceAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorDuplicateAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorGroupAction { get; init; }
    public required MousePressEventHandler.ComponentMousePressAction EditorUngroupAction { get; init; }
}
