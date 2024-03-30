using MGUI.Core.UI;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelSelectPageA : PageBase
{
    /*private const FileDialogOptions LoadLevelDialogOptions =
        FileDialogOptions.AllowEnterFolders |
        FileDialogOptions.AllowOverride |
        FileDialogOptions.CageInStartingPath |
        FileDialogOptions.MustSelectExistingFile;*/

    private readonly MenuInputController _inputController;
    private readonly string _levelsRootPath;

    public LevelSelectPageA(
        MGDesktop desktop,
        MenuInputController inputController)
        : base(desktop, inputController)
    {
        _levelsRootPath = Path.Combine(RootDirectoryManager.RootDirectory, "levels");
        _inputController = inputController;
    }

    protected override void OnInitialise(MGDesktop desktop)
    {
        /* MessageBox.OpenLoadFileDialog(
             _levelsRootPath,
             OnFileSelected,
             options: LoadLevelDialogOptions,
             filterFiles: FilterFiles,
             title: "Select a level",
             loadButtonTxt: "Select level");*/
    }

    private static bool FilterFiles(string filename)
    {
        var extension = Path.GetExtension(filename);

        return LevelFileTypeHandler.FileExtensionIsValidLevelType(extension);
    }

    /*   private bool OnFileSelected(FileDialogResponse fileDialogResponse)
       {
           if (!fileDialogResponse.FileExists)
               return false;

           MenuScreen.Current.MenuPageCreator.LevelToLoadFilepath = fileDialogResponse.FullPath;

           var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

           if (levelStartPage is null)
               return false;

           MenuScreen.Current.SetNextPage(levelStartPage);

           return true;
       }*/

    private void OnCancel()
    {
        var mainPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        MenuScreen.Current.SetNextPage(mainPage);
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {

    }

    public override void Tick()
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

    protected override void OnDispose()
    {
    }
}