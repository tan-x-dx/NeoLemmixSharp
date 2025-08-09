using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Pages.LevelSelect;

namespace NeoLemmixSharp.Menu;

public sealed class MenuPageCreator
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly MenuInputController _inputController;

    public string LevelToLoadFilepath { get; set; }

    public MenuPageCreator(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        MenuInputController inputController)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _inputController = inputController;
        LevelToLoadFilepath = GetLevelFilePath();
    }

    private static string GetLevelFilePath()
    {
        var file =
             // "tanxdx_TheTreacheryOfLemmings_R3V1.nxlv";
             // "rotation test.nxlv";
             // "render test.nxlv";
             "Foo.nxlv";
        // "object test 2.nxlv";
        // "hatch count test.nxlv";
        // "lemming_count_test.nxlv";
        // "test foo.nxlv";
        // "Amiga Lemmings\\Oh No! More Lemmings\\Tame\\02_Rent-a-Lemming.nxlv";
        // "Amiga Lemmings\\Oh No! More Lemmings\\Tame\\05_Snuggle_up_to_a_Lemming.nxlv";
        // "Amiga Lemmings\\Lemmings\\Tricky\\05_Careless_clicking_costs_lives.nxlv";
        // "LemRunner\\Industry\\TheNightShift.nxlv";
        // "Amiga Lemmings\\Lemmings\\Tricky\\04_Here's_one_I_prepared_earlier.nxlv";
        // "IntegralLemmingsV5\\Alpha\\TheseLemmingsAndThoseLemmings.nxlv";
        // "CuttingItClose.nxlv";
        // "scrollTest.nxlv";
        // "LemRunner\\Mona\\ACaeloUsqueAdCentrum.nxlv";
        // "groupTest.nxlv";
        // "eraseTest.nxlv";
        // "Amiga Lemmings\\Lemmings\\Fun\\19_Take_good_care_of_my_Lemmings.nxlv";

        return Path.Combine(RootDirectoryManager.LevelFolderDirectory, file);
    }

    public MainPage CreateMainPage()
    {
        return new MainPage(_inputController);
    }

    public LevelSelectPage CreateLevelSelectPage()
    {
        return new LevelSelectPage(_inputController);
    }

    public LevelStartPage? CreateLevelStartPage()
    {
        LevelStartPage? result = null;
        try
        {
            var levelData = FileTypeHandler.ReadLevel(LevelToLoadFilepath);

            levelData.AssertLevelDataIsValid();

            var levelBuilder = new LevelBuilder(_contentManager, _graphicsDevice);
            var levelScreen = levelBuilder.BuildLevel(levelData);
            result = new LevelStartPage(_inputController, levelScreen);

            GC.Collect(2, GCCollectionMode.Forced);
        }
        catch (Exception ex)
        {
            TextureCache.DisposeOfShortLivedTextures();

            var exceptionWindow = new ExceptionViewer(_inputController, ex);

            exceptionWindow.Initialise();
        }
        finally
        {
            //levelReader?.Dispose();
        }

        return result;
    }

    public LevelStartPage? CreateLevelStartPage(LevelData levelData)
    {
        LevelStartPage? result = null;
        try
        {
            levelData.AssertLevelDataIsValid();

            var levelBuilder = new LevelBuilder(_contentManager, _graphicsDevice);
            var levelScreen = levelBuilder.BuildLevel(levelData);
            result = new LevelStartPage(_inputController, levelScreen);

            GC.Collect(2, GCCollectionMode.Forced);
        }
        catch (Exception ex)
        {
            TextureCache.DisposeOfShortLivedTextures();

            var exceptionWindow = new ExceptionViewer(_inputController, ex);

            exceptionWindow.Initialise();
        }
        finally
        {
        }

        return result;
    }
}