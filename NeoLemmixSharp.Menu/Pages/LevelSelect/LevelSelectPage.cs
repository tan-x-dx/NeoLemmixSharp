using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelSelectPage : PageBase, IComparer<LevelBrowserEntry>
{
    /// <summary>
    /// Arbitrary threshold for calling GC for loading after loading levels
    /// </summary>
    private const int LevelLoadCountThreshold = 32;

    private static readonly string LevelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, NeoLemmixFileExtensions.LevelFolderName);

    private readonly List<LevelBrowserEntry> _levelBrowserEntries = new();
    private readonly LevelList _levelList = new();

    private int _numberOfLevelsLoaded = 0;

    public LevelSelectPage(MenuInputController inputController)
        : base(inputController)
    {
    }

    protected override void OnInitialise()
    {
        UiHandler.RootComponent.AddComponent(_levelList);

        OnResize();
        RepopulateMenu();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        OnResize();
    }

    private void OnResize()
    {
        const int margin = 64;

        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        var rootComponent = UiHandler.RootComponent;
        rootComponent.Left = margin;
        rootComponent.Top = margin;
        rootComponent.Width = windowWidth - margin * 2;
        rootComponent.Height = windowHeight - margin * 2;

        _levelList.SetDimensions(
            margin * 2,
            margin * 2,
            windowWidth / 2 - margin * 4,
            windowHeight - margin * 4);
    }

    protected override void HandleUserInput()
    {
    }

    protected override void OnTick()
    {
        for (var i = 0; i < _levelBrowserEntries.Count; i++)
        {
            if (_levelBrowserEntries[i] is LevelEntry levelEntry && levelEntry.IsLoading)
            {
                levelEntry.LoadLevelData(IGameWindow.Instance.GraphicsDevice);
                _numberOfLevelsLoaded++;
                break;
            }
        }

        if (_numberOfLevelsLoaded >= LevelLoadCountThreshold)
        {
            GC.Collect(3, GCCollectionMode.Forced);
            _numberOfLevelsLoaded = 0;
        }
    }

    private void RepopulateMenu()
    {
        _levelBrowserEntries.Clear();
        _levelBrowserEntries.AddRange(LevelBrowserEntry.GetMenuItems(LevelsRootPath));
        _levelBrowserEntries.Sort(this);

        GC.Collect(3, GCCollectionMode.Forced);
    }

    protected override void OnDispose()
    {
        foreach (var entry in _levelBrowserEntries)
        {
            entry.Dispose();
        }
        _levelBrowserEntries.Clear();
    }

    int IComparer<LevelBrowserEntry>.Compare(LevelBrowserEntry? x, LevelBrowserEntry? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        if (x is LevelFolderEntry xFolder)
        {
            if (y is LevelFolderEntry yFolder)
                return string.CompareOrdinal(xFolder.DisplayName, yFolder.DisplayName);

            return -1;
        }

        if (y is LevelFolderEntry)
            return 1;

        var xFile = (LevelEntry)x;
        var yFile = (LevelEntry)y;

        return string.CompareOrdinal(xFile.DisplayName, yFile.DisplayName);
    }
}
