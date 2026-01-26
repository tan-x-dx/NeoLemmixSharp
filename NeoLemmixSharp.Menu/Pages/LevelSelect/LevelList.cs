using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelList : Component, IComparer<LevelBrowserEntry>
{
    /// <summary>
    /// Arbitrary threshold for calling GC for loading after loading levels
    /// </summary>
    private const int LevelLoadCountThreshold = 32;

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
    }

    public int ScrollIndex
    {
        get => _scrollIndex;
        set
        {
            _scrollIndex = Math.Clamp(value, 0, _children!.Count);
            OnResize();
        }
    }

    public void RefreshLevels()
    {
        _levelBrowserEntries.Clear();
        //  _levelBrowserEntries.AddRange(LevelBrowserEntry.GetMenuItems(LevelsRootPath));
        _levelBrowserEntries.Sort(this);

        var children = _children!;
        children.Clear();

        foreach (var levelBrowserEntry in _levelBrowserEntries.SelectMany(l => l.GetSubEntries()))
        {
            levelBrowserEntry.Index = children.Count;
            levelBrowserEntry.MousePressed.RegisterMouseEvent(OnEntryClick);
            levelBrowserEntry.MouseDoubleClick.RegisterMouseEvent(OnEntryDoubleClick);

            children.Add(levelBrowserEntry);
        }

        GC.Collect(2, GCCollectionMode.Forced);
        OnResize();
    }

    private void OnResize()
    {
        const int interiorMargin = 8;

        var horizontalSpace = Width - (interiorMargin * 2);
        var verticalSpace = Height - (interiorMargin * 2);

        const int ItemHeight = MenuFont.GlyphHeight + (LevelBrowserEntry.ButtonPadding * 2);

        var maxNumberOfItemsDisplayed = verticalSpace / ItemHeight;

        var children = _children!;
        for (var i = 0; i < children.Count; i++)
        {
            var entry = children[i];
            if (i >= ScrollIndex && i < ScrollIndex + maxNumberOfItemsDisplayed)
            {
                entry.IsVisible = true;

                entry.SetDimensions(
                    Left + interiorMargin,
                    Top + interiorMargin + ((i - ScrollIndex) * ItemHeight),
                    horizontalSpace,
                    ItemHeight);
            }
            else
            {
                entry.IsVisible = false;
            }
        }
    }

    private void OnEntryClick(Component c, Point position)
    {
        _selectedEntry = c as LevelBrowserEntry;
    }

    private void OnEntryDoubleClick(Component c, Point position)
    {
        if (c is not LevelBrowserEntry)
            return;

        if (c is LevelFolderEntry folder)
        {
            folder.IsOpen = !folder.IsOpen;
        }

        if (c is LevelEntry level && level.LevelData is not null)
        {
            var levelStartPage = MenuScreen.Instance.MenuPageCreator.CreateLevelStartPage(level.LevelData);

            if (levelStartPage is null)
                return;

            MenuScreen.Instance.SetNextPage(levelStartPage);
        }
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

        ScrollIndex -= inputController.ScrollDelta;
    }

    public void Tick()
    {
        for (var i = 0; i < _levelBrowserEntries.Count; i++)
        {
            if (_levelBrowserEntries[i] is LevelEntry levelEntry)
            {
                if (levelEntry.IsLoading)
                {
                    levelEntry.LoadLevelData();
                    _numberOfLevelsLoaded++;
                    break;
                }
            }
        }

        if (_numberOfLevelsLoaded >= LevelLoadCountThreshold)
        {
            GC.Collect(2, GCCollectionMode.Forced);
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
