using GeonBit.UI.Entities;
using GeonBit.UI.Utils;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelSelectPage : IPage
{
    private const FileDialogOptions LoadLevelDialogOptions =
        FileDialogOptions.AllowEnterFolders |
        FileDialogOptions.AllowOverride |
        FileDialogOptions.CageInStartingPath |
        FileDialogOptions.MustSelectExistingFile;

    private readonly MenuInputController _inputController;
    private readonly string _levelsRootPath;

    public LevelSelectPage(MenuInputController inputController)
    {
        _levelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, "levels");
        _inputController = inputController;
    }

    public void Initialise(RootPanel rootPanel)
    {
        MessageBox.OpenLoadFileDialog(
            _levelsRootPath,
            OnFileSelected,
            options: LoadLevelDialogOptions,
            filterFiles: FilterFiles,
            title: "Select a level",
            loadButtonTxt: "Select level");
    }

    private static bool FilterFiles(string filename)
    {
        var extension = Path.GetExtension(filename.AsSpan());

        return LevelFileTypeHandler.FileExtensionIsValidLevelType(extension);
    }

    private bool OnFileSelected(FileDialogResponse fileDialogResponse)
    {
        if (!fileDialogResponse.FileExists)
            return false;

        MenuScreen.Current.MenuPageCreator.LevelToLoadFilepath = fileDialogResponse.FullPath;

        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        MenuScreen.Current.SetNextPage(levelStartPage);

        return true;
    }

    private void OnCancel()
    {
        var mainPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        MenuScreen.Current.SetNextPage(mainPage);
    }

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {

    }

    public void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
        if (_inputController.Quit.IsPressed)
        {
            OnCancel();
            return;
        }
    }

    private void HandleMouseInput()
    {
    }

    public void Dispose()
    {
    }
}