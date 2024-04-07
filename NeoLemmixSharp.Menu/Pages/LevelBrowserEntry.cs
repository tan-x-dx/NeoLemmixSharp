using MGUI.Core.UI;
using MGUI.Shared.Helpers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Menu.Rendering;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelBrowserEntry : ViewModelBase, IDisposable
{
    private const int IconSize = 16;

    private readonly List<LevelBrowserEntry>? _subEntries;
    
    private IconType _iconType;
    private MGTextureData _textureData;
    private bool _isOpen;

    public string Path { get; }
    public string DisplayName { get; }

    public bool IsFolder { get; }
    public int IndentationLevel { get; }
    public MGTextureData TextureData => _textureData;
    public int Offset => IndentationLevel * IconSize;

    public bool IsOpen
    {
        get => IsFolder && _isOpen;
        set
        {
            if (!IsFolder || _isOpen == value)
                return;

            _isOpen = value;

            NotifyPropertyChanged(nameof(IsOpen));

            _iconType = _isOpen
                ? IconType.ArrowOpened
                : IconType.ArrowClosed;

            _textureData = GetTextureData(_iconType);
            NotifyPropertyChanged(nameof(TextureData));
        }
    }

    public IEnumerable<LevelBrowserEntry> GetAllEntries()
    {
        yield return this;

        if (_subEntries is null || !_isOpen)
            yield break;

        foreach (var levelBrowserEntry in _subEntries.SelectMany(subEntry => subEntry.GetAllEntries()))
        {
            yield return levelBrowserEntry;
        }
    }

    public static IEnumerable<LevelBrowserEntry> GetMenuItemsForFolder(string folder, int indentationLevel = 0)
    {
        var subFolders = Directory.GetDirectories(folder);
        foreach (var subFolder in subFolders)
        {
            var folderEntry = new LevelBrowserEntry(subFolder, indentationLevel, true, IconType.ArrowClosed);

            folderEntry._subEntries!.AddRange(GetMenuItemsForFolder(subFolder, indentationLevel + 1));

            yield return folderEntry;
        }

        var files = Directory.GetFiles(folder);
        foreach (var file in files)
        {
            var fileExtension = System.IO.Path.GetExtension(file);

            if (LevelFileTypeHandler.FileExtensionIsValidLevelType(fileExtension))
            {
                var levelEntry = new LevelBrowserEntry("level name", indentationLevel, false, IconType.LevelNotAttempted);
                yield return levelEntry;
            }
        }
    }

    private LevelBrowserEntry(
        string path,
        int indentationLevel,
        bool isFolder,
        IconType iconType)
    {
        Path = path;
        DisplayName = System.IO.Path.GetFileNameWithoutExtension(path);
        IndentationLevel = indentationLevel;
        IsFolder = isFolder;
        _iconType = iconType;

        _textureData = GetTextureData(iconType);

        _subEntries = IsFolder
            ? new List<LevelBrowserEntry>()
            : null;
    }

    public enum IconType
    {
        UserAttemptedLevel,
        UserCompletedLevel,
        LevelDataChanged,
        LevelNotAttempted,
        LevelCompletedWithTalismans,
        ArrowClosed,
        ArrowOpened,
        LevelCompletedWithoutTalismans
    }

    private static MGTextureData GetTextureData(IconType iconType)
    {
        return new MGTextureData(MenuSpriteBank.MenuIcons, GetIconRectangle(iconType));
    }

    private static Rectangle GetIconRectangle(IconType iconType)
    {
        var xOffset = (int)iconType * IconSize;

        return new Rectangle(xOffset, 0, IconSize, IconSize);
    }

    public void Dispose()
    {
        if (_subEntries is not null)
        {
            foreach (var subEntry in _subEntries)
            {
                subEntry.Dispose();
            }

            _subEntries.Clear();
        }

    }
}