using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Ui.Components.Buttons;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public abstract class LevelBrowserEntry : Button
{
    protected readonly int _indentationLevel;

    public abstract string DisplayName { get; }
    public int Index { get; set; }

    protected LevelBrowserEntry(
        int indentationLevel)
        : base(0, 0, null)
    {
        _indentationLevel = indentationLevel;
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

            if (LevelFileTypeHandler.FileExtensionIsValidLevelType(fileExtension))
            {
                yield return new LevelEntry(file, 0);
            }
        }
    }

    public abstract IEnumerable<LevelBrowserEntry> GetSubEntries();
}

public sealed class LevelFolderEntry : LevelBrowserEntry
{
    private readonly string _folderPath;
    private List<LevelBrowserEntry>? _entries;

    public override string DisplayName => _folderPath;

    public bool IsOpen { get; set; }

    public LevelFolderEntry(
        string folder,
        int indentationLevel)
        : base(indentationLevel)
    {
        _folderPath = folder;
    }

    public override void InvokeMouseDown(LevelPosition mousePosition)
    {
        base.InvokeMouseDown(mousePosition);
    }

    public void LoadSubEntries()
    {
        _entries ??= new List<LevelBrowserEntry>(GetMenuItems(_folderPath, _indentationLevel + 1));
    }

    public override IEnumerable<LevelBrowserEntry> GetSubEntries()
    {
        if (IsOpen && _entries is not null)
            return _entries.SelectMany(l => l.GetSubEntries()).Prepend(this);

        return [this];
    }

    protected override void OnDispose()
    {
        if (_entries is null)
            return;

        foreach (var entry in _entries)
        {
            entry.Dispose();
        }
        _entries.Clear();
    }
}

public sealed class LevelEntry : LevelBrowserEntry
{
    private readonly string _filePath;

    private string _displayData = EngineConstants.LevelLoadingDisplayString;
    private LevelData? _levelData;

    public override string DisplayName => _levelData?.LevelTitle ?? _displayData;
    public bool IsLoading => _levelData is not null;

    public LevelEntry(
        string filePath,
        int indentationLevel)
        : base(indentationLevel)
    {
        _filePath = filePath;
    }

    public void LoadLevelData(GraphicsDevice graphicsDevice)
    {
        ILevelReader? levelReader = null;
        try
        {
            var fileExtension = Path.GetExtension(_filePath.AsSpan());
            levelReader = LevelFileTypeHandler.GetLevelReaderForFileExtension(fileExtension);
            _levelData = levelReader.ReadLevel(_filePath, graphicsDevice);
        }
        catch
        {
            _displayData = EngineConstants.LevelLoadingErrorOccurredDisplayString;
            _levelData = null;
        }
        finally
        {
            levelReader?.Dispose();
        }
    }

    public override IEnumerable<LevelBrowserEntry> GetSubEntries()
    {
        yield return this;
    }

    protected override void OnDispose()
    {
        _levelData = null;
    }
}