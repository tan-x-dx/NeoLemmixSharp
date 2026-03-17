namespace NeoLemmixSharp.Menu.LevelEditor.ChangeSet;

public sealed class LevelEditorChangeTracker
{
    private readonly List<ILevelEditorChange> _levelEditorChanges = [];

    public void Clear() => _levelEditorChanges.Clear();
}
