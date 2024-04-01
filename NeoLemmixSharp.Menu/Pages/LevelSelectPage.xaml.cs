using MGUI.Core.UI;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelSelectPage : PageBase
{
    private readonly string _levelsRootPath;
    private readonly MgListBox<object> _listBox;

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
        var data = new List<object>();
        var subFolders = Directory.GetDirectories(_levelsRootPath);
        foreach (var subFolder in subFolders)
        {

        }

        var files = Directory.GetFiles(_levelsRootPath);
        foreach (var file in files)
        {
            if (LevelFileTypeHandler.FileExtensionIsValidLevelType(file))
            {

            }
        }

        _listBox.SetItemsSource(data);
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
        if (InputController.Quit.IsPressed)
        {
            GoBack();
            return;
        }

        if (InputController.F1.IsPressed)
        {
            PlayButtonClick(null!);
            return;
        }

        if (InputController.F2.IsPressed)
        {
            return;
        }
    }

    private static void PlayButtonClick(MGElement entity)
    {
        var levelStartPage = MenuScreen.Current.MenuPageCreator.CreateLevelStartPage();

        if (levelStartPage is null)
            return;

        MenuScreen.Current.SetNextPage(levelStartPage);
    }

    private static void GoBack()
    {
        var mainPage = MenuScreen.Current.MenuPageCreator.CreateMainPage();

        MenuScreen.Current.SetNextPage(mainPage);
    }

    private void HandleMouseInput()
    {
    }

    protected override void OnDispose()
    {
    }
}

public sealed class FooBar
{

}