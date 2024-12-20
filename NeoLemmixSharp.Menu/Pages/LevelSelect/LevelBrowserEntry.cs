using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public abstract class LevelBrowserEntry : Component
{
    public const int ButtonPadding = 4;

    protected readonly int _indentationLevel;

    public abstract string DisplayName { get; }
    public int Index { get; set; }

    protected LevelBrowserEntry(
        int indentationLevel)
        : base(0, 0, null)
    {
        _indentationLevel = indentationLevel;

        MouseEnter.RegisterMouseEvent(SetMouseOver);
        MouseDown.RegisterMouseEvent(SetMousePress);
        MouseUp.RegisterMouseEvent(SetMouseOver);
        MouseExit.RegisterMouseEvent(SetMouseNormal);
    }

    protected abstract override void OnDispose();

    public static IEnumerable<LevelBrowserEntry> GetMenuItems(string folder, int indentationLevel = 0)
    {
        var subFolders = Directory.GetDirectories(folder);
        foreach (var subFolder in subFolders)
        {
            yield return new LevelFolderEntry(subFolder, 0);
        }

        var files = Directory.GetFiles(folder);
        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.AsSpan());

            if (LevelFileTypeHandler.FileExtensionIsRecognised(fileExtension, out var fileType, out _))
            {
                switch (fileType)
                {
                    case FileType.Level: yield return new LevelEntry(file, 0); break;

                    case FileType.NeoLemmixConfig: Foo(file); break;
                }
            }
        }
    }

    private static void Foo(string file)
    {
        var fileName = Path.GetFileName(file.AsSpan());

    }

    protected abstract override void RenderComponent(SpriteBatch spriteBatch);

    public abstract IEnumerable<LevelBrowserEntry> GetSubEntries();
}
