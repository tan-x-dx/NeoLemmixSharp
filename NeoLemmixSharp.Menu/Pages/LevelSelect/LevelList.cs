using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelList : Component, IComparer<LevelBrowserEntry>
{
    /// <summary>
    /// Arbitrary threshold for calling GC for loading after loading levels
    /// </summary>
    private const int LevelLoadCountThreshold = 32;

    private static readonly string LevelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, NeoLemmixFileExtensions.LevelFolderName);

    private readonly List<LevelBrowserEntry> _levelBrowserEntries = new();
    private LevelBrowserEntry? _selectedEntry;
    private int _scrollIndex;
    private int _numberOfLevelsLoaded = 0;

    public LevelList()
        : base(0, 0, 0, 0)
    {
        _children = new List<Component>();
        Colors = new ColorPacket(
            new Color(0xff111111),
            new Color(0xff111111),
            new Color(0xff111111),
            new Color(0xff111111));

        SetResizeAction(OnResize);
    }

    public int ScrollIndex
    {
        get => _scrollIndex;
        set => _scrollIndex = Math.Clamp(value, 0, _children!.Count);
    }

    public void RefreshLevels()
    {
        _levelBrowserEntries.Clear();
        _levelBrowserEntries.AddRange(LevelBrowserEntry.GetMenuItems(LevelsRootPath));
        _levelBrowserEntries.Sort(this);

        var children = _children!;
        children.Clear();

        foreach (var levelBrowserEntry in _levelBrowserEntries.SelectMany(l => l.GetSubEntries()))
        {
            levelBrowserEntry.Index = children.Count;
            levelBrowserEntry.SetClickAction(OnEntryClick);

            children.Add(levelBrowserEntry);
        }

        GC.Collect(3, GCCollectionMode.Forced);
        OnResize();
    }

    private void OnResize()
    {
        const int interiorMargin = 8;

        var horizontalSpace = Width - (interiorMargin * 2);
        var verticalSpace = Height - (interiorMargin * 2);

        const int itemHeight = MenuFont.GlyphHeight + (LevelBrowserEntry.ButtonPadding * 2);

        var maxNumberOfItemsDisplayed = 1 + (verticalSpace / itemHeight);

        var children = _children!;
        for (var i = 0; i < children.Count; i++)
        {
            var entry = children[i];
            if (i >= ScrollIndex && i < ScrollIndex + maxNumberOfItemsDisplayed)
            {
                entry.IsVisible = true;

                entry.SetDimensions(
                    Left + interiorMargin,
                    Top + interiorMargin + (i * itemHeight),
                    horizontalSpace,
                    itemHeight);
            }
            else
            {
                entry.IsVisible = false;
            }
        }
    }

    private void OnEntryClick()
    {
    }

    public void HandleUserInput(MenuInputController inputController)
    {
        if (inputController.RightArrow.IsPressed)
        {
            if (_selectedEntry is not LevelFolderEntry folder)
                return;

            folder.IsOpen = true;


            return;
        }
    }

    public void Tick()
    {
        for (var i = 0; i < _levelBrowserEntries.Count; i++)
        {
            if (_levelBrowserEntries[i] is LevelEntry levelEntry)
            {
                if (levelEntry.IsLoading)
                {
                    levelEntry.LoadLevelData(IGameWindow.Instance.GraphicsDevice);
                    _numberOfLevelsLoaded++;
                    break;
                }
            }
        }

        if (_numberOfLevelsLoaded >= LevelLoadCountThreshold)
        {
            GC.Collect(3, GCCollectionMode.Forced);
            _numberOfLevelsLoaded = 0;
        }
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
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
