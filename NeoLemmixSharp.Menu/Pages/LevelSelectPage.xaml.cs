using MGUI.Core.UI;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelSelectPage : PageBase
{
    private readonly string _levelsRootPath;
    private readonly MgListBox<object> _listBox;
    private readonly List<LevelBrowserEntry> _allLevelBrowserEntries = new();
    private readonly List<LevelBrowserEntry> _currentlyDisplayedLevelBrowserEntries = new();

    private LevelBrowserEntry? _selectedLevelBrowserEntry;

    public LevelSelectPage(
        MGDesktop desktop,
        MenuInputController inputController)
        : base(desktop, inputController)
    {
        _levelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, NeoLemmixFileExtensions.LevelFolderName);

        _listBox = Window.Content.GetChildren().OfType<MgListBox<object>>().First();
    }

    protected override void OnInitialise()
    {
        _listBox.SelectionChanged += ListBoxOnSelectionChanged;

        _allLevelBrowserEntries.AddRange(LevelBrowserEntry.GetMenuItemsForFolder(_levelsRootPath));

        RepopulateMenu();
    }

    private void ListBoxOnSelectionChanged(object? sender, ReadOnlyCollection<MgListBoxItem<object>> e)
    {
        if (e.Count == 0)
        {
            _selectedLevelBrowserEntry = null;
            return;
        }

        var listBoxItem = e[0];

        _selectedLevelBrowserEntry = (LevelBrowserEntry)listBoxItem.Data;
    }

    private void RepopulateMenu()
    {
        _currentlyDisplayedLevelBrowserEntries.Clear();
        _currentlyDisplayedLevelBrowserEntries.AddRange(_allLevelBrowserEntries.SelectMany(x => x.GetAllEntries()));
        var data = new ObservableCollection<object>(_currentlyDisplayedLevelBrowserEntries);

        _listBox.SetItemsSource(data);
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
    }

    public override void Tick()
    {
        HandleKeyboardInput();
        HandleInputsForSelection();
    }

    private void HandleKeyboardInput()
    {
        if (InputController.Quit.IsPressed)
        {
            GoBack();
            return;
        }
    }

    private static void GoBack()
    {
        var mainPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        MenuScreen.Current.SetNextPage(mainPage);
    }

    private void HandleInputsForSelection()
    {
        if (_selectedLevelBrowserEntry is null)
            return;

        HandleListScroll();

        if (_selectedLevelBrowserEntry.IsFolder)
        {
            HandleInputsForFolder();
            return;
        }

        HandleInputsForFile();
    }

    private void HandleListScroll()
    {
        if (_selectedLevelBrowserEntry is null)
            return;

        var pressUp = InputController.UpArrow.IsPressed;
        var pressDown = InputController.DownArrow.IsPressed;

        if (!(pressUp || pressDown))
            return;

        var scrollDelta = 0;
        if (pressUp)
        {
            scrollDelta = -1;
        }

        if (pressDown)
        {
            scrollDelta = 1;
        }

        var index = GetIndexOfItem(_selectedLevelBrowserEntry);

        TrySelectEntryWithIndex(index + scrollDelta);
    }

    private int GetIndexOfItem(LevelBrowserEntry levelBrowserEntry)
    {
        var span = CollectionsMarshal.AsSpan(_currentlyDisplayedLevelBrowserEntries);
        var result = 0;
        foreach (var candidate in span)
        {
            if (ReferenceEquals(candidate, levelBrowserEntry))
                return result;
            result++;
        }

        return 0;
    }

    private void TrySelectEntryWithIndex(int index)
    {
        index = Math.Clamp(index, 0, _currentlyDisplayedLevelBrowserEntries.Count - 1);

        var entry = _currentlyDisplayedLevelBrowserEntries[index];

        _listBox.SelectItem(entry, true);
    }

    private void HandleInputsForFolder()
    {
        var selectedFolder = _selectedLevelBrowserEntry!;

        if (InputController.LeftMouseButtonAction.IsDoubleTap || InputController.Enter.IsPressed || InputController.Space.IsPressed)
        {
            var index = GetIndexOfItem(selectedFolder);

            selectedFolder.IsOpen = !selectedFolder.IsOpen;
            RepopulateMenu();

            TrySelectEntryWithIndex(index);
            return;
        }

        if (InputController.RightArrow.IsPressed && !selectedFolder.IsOpen)
        {
            var index = GetIndexOfItem(selectedFolder);

            selectedFolder.IsOpen = true;
            RepopulateMenu();

            TrySelectEntryWithIndex(index + 1);
            return;
        }

        if (InputController.LeftArrow.IsPressed && selectedFolder.IsOpen)
        {
            var index = GetIndexOfParentFolder(selectedFolder);

            selectedFolder.IsOpen = false;
            RepopulateMenu();

            TrySelectEntryWithIndex(index);
        }
    }

    private void HandleInputsForFile()
    {
        var selectedLevel = _selectedLevelBrowserEntry!;
        if (InputController.LeftMouseButtonAction.IsDoubleTap)
        {
            OnFileSelected(selectedLevel);
        }

        if (InputController.LeftArrow.IsPressed)
        {
            var index = GetIndexOfParentFolder(selectedLevel);
            TrySelectEntryWithIndex(index);
        }
    }

    private int GetIndexOfParentFolder(LevelBrowserEntry selectedEntry)
    {
        var index = GetIndexOfItem(selectedEntry);

        var requiredIndentationLevel = selectedEntry.IndentationLevel - 1;
        if (requiredIndentationLevel == -1)
            return index;

        var span = CollectionsMarshal.AsSpan(_currentlyDisplayedLevelBrowserEntries);
        while (index > 0)
        {
            if (span[index].IndentationLevel <= requiredIndentationLevel)
            {
                return index;
            }

            index--;
        }

        return index;
    }

    private static void OnFileSelected(LevelBrowserEntry? selectedLevelBrowserEntry)
    {
        if (selectedLevelBrowserEntry is null || selectedLevelBrowserEntry.IsFolder)
            return;

        MenuScreen.Current.MenuPageCreator.LevelToLoadFilepath = selectedLevelBrowserEntry.Path;

        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    protected override void OnDispose()
    {
        _listBox.SelectionChanged -= ListBoxOnSelectionChanged;
        foreach (var levelBrowserEntry in _allLevelBrowserEntries)
        {
            levelBrowserEntry.Dispose();
        }
    }
}