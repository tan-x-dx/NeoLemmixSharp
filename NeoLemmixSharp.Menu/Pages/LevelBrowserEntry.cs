using MGUI.Core.UI;
using MGUI.Shared.Helpers;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Menu.Rendering;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelBrowserEntry : ViewModelBase, IDisposable
{
    private const int IconSize = 16;

    private readonly string? _fileExtension;
    private readonly List<LevelBrowserEntry>? _subEntries;

    private IconType _iconType;
    private MGTextureData _textureData;
    private string _displayName;
    private bool _isOpen;

    public string Path { get; }
    public string DisplayName
    {
        get => _displayName;
        private set => this.RaisePropertyChanged(ref _displayName, value);
    }

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

            if (!_isOpen)
                return;

            foreach (var subEntry in _subEntries!)
            {
                if (string.IsNullOrWhiteSpace(subEntry.DisplayName))
                {
                    subEntry.OnDisplay();
                }
            }
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
            var folderEntry = new LevelBrowserEntry(subFolder, indentationLevel);

            folderEntry._subEntries!.AddRange(GetMenuItemsForFolder(subFolder, indentationLevel + 1));

            yield return folderEntry;
        }

        var files = Directory.GetFiles(folder);
        foreach (var file in files)
        {
            var fileExtension = LevelFileTypeHandler.MatchLevelFileExtension(file);

            if (LevelFileTypeHandler.FileExtensionIsValidLevelType(fileExtension))
            {
                var levelEntry = new LevelBrowserEntry(file, fileExtension!, indentationLevel, IconType.LevelNotAttempted);

                yield return levelEntry;
            }
        }
    }

    private LevelBrowserEntry(
        string path,
        int indentationLevel)
    {
        _fileExtension = null;
        Path = path;
        _displayName = System.IO.Path.GetFileNameWithoutExtension(path);
        IndentationLevel = indentationLevel;
        IsFolder = true;
        _iconType = IconType.ArrowClosed;

        _textureData = GetTextureData(_iconType);

        _subEntries = new List<LevelBrowserEntry>();
    }

    private LevelBrowserEntry(
        string path,
        string fileExtension,
        int indentationLevel,
        IconType iconType)
    {
        Path = path;
        _fileExtension = fileExtension;
        IndentationLevel = indentationLevel;
        if (indentationLevel == 0)
        {
            ScrapeLevelTitle();
        }
        else
        {
            _displayName = string.Empty;
        }

        IsFolder = false;
        _iconType = iconType;

        _textureData = GetTextureData(_iconType);

        _subEntries = null;
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

    private void ScrapeLevelTitle()
    {
        ILevelReader? levelReader = null;
        try
        {
            levelReader = LevelFileTypeHandler.GetLevelReaderForFileExtension(_fileExtension);
            DisplayName = levelReader.ScrapeLevelTitle(Path);
        }
        catch (Exception e)
        {
            DisplayName = "ERROR OCCURRED WHEN LOADING";
        }
        finally
        {
            levelReader?.Dispose();
        }
    }

    private void OnDisplay()
    {
        ScrapeLevelTitle();
    }
}