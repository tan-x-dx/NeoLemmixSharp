using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelSelectPageAa : PageBase
{
    private readonly string _levelsRootPath;
    private readonly List<LevelBrowserEntryAaa> _allLevelBrowserEntries = new();
    private readonly List<LevelBrowserEntryAaa> _currentlyDisplayedLevelBrowserEntries = new();

    private LevelBrowserEntryAaa _selectedLevelBrowserEntry;

    public LevelSelectPageAa(
        MenuInputController inputController)
        : base(inputController)
    {
        _levelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, NeoLemmixFileExtensions.LevelFolderName);

    }

    protected override void OnInitialise()
    {
        //root.Children.Add(_levelList.Visual);

        _allLevelBrowserEntries.AddRange(LevelBrowserEntryAaa.GetMenuItemsForFolder(_levelsRootPath));
        RepopulateMenu();

        OnResize();
    }

    private void RepopulateMenu()
    {
        _currentlyDisplayedLevelBrowserEntries.Clear();
        _currentlyDisplayedLevelBrowserEntries.AddRange(_allLevelBrowserEntries.SelectMany(x => x.GetAllEntries()));

        //_levelList.Items.Clear();
        foreach (var entry in _currentlyDisplayedLevelBrowserEntries)
        {
            // _levelList.AddChild(entry.Text.Text);
            //entry.TextBox.cl
        }
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

        UiHandler.RootComponent.Left = margin;
        UiHandler.RootComponent.Top = margin;
        UiHandler.RootComponent.Width = windowWidth - margin * 2;
        UiHandler.RootComponent.Height = windowHeight - margin * 2;
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            NavigateToMainMenuPage();
        }
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

    private int GetIndexOfItem(LevelBrowserEntryAaa levelBrowserEntry)
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

    private int GetIndexOfParentFolder(LevelBrowserEntryAaa selectedEntry)
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

    private static void OnFileSelected(LevelBrowserEntryAaa? selectedLevelBrowserEntry)
    {
        if (selectedLevelBrowserEntry is null || selectedLevelBrowserEntry.IsFolder)
            return;

        MenuScreen.Current.MenuPageCreator.LevelToLoadFilepath = selectedLevelBrowserEntry.LevelFilePath;

        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    protected override void OnDispose()
    {
        foreach (var levelBrowserEntry in _allLevelBrowserEntries)
        {
            levelBrowserEntry.Dispose();
        }
    }
}