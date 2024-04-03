using MGUI.Core.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Menu.Pages;

namespace NeoLemmixSharp.Menu;

public sealed class MenuPageCreator
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly MGDesktop _desktop;

    private readonly MenuInputController _inputController;

    public string LevelToLoadFilepath { get; set; }

    public MenuPageCreator(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        MenuInputController inputController,
        MGDesktop desktop)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _inputController = inputController;
        _desktop = desktop;

        LevelToLoadFilepath = GetLevelFilePath();
    }

    private static string GetLevelFilePath()
    {
        var file =
            // "levels\\tanxdx_TheTreacheryOfLemmings_R3V1.nxlv";
            // "levels\\rotation test.nxlv";
            // "levels\\render test.nxlv";
             "levels\\Foo.nxlv";
           // "levels\\object test 2.nxlv";
        // "levels\\hatch count test.nxlv";
        // "levels\\lemming_count_test.nxlv";
        // "levels\\test foo.nxlv";
        // "levels\\Amiga Lemmings\\Oh No! More Lemmings\\Tame\\02_Rent-a-Lemming.nxlv";
        // "levels\\Amiga Lemmings\\Oh No! More Lemmings\\Tame\\05_Snuggle_up_to_a_Lemming.nxlv";
        // "levels\\Amiga Lemmings\\Lemmings\\Tricky\\05_Careless_clicking_costs_lives.nxlv";
        // "levels\\LemRunner\\Industry\\TheNightShift.nxlv";
        // "levels\\Amiga Lemmings\\Lemmings\\Tricky\\04_Here's_one_I_prepared_earlier.nxlv";
        // "levels\\IntegralLemmingsV5\\Alpha\\TheseLemmingsAndThoseLemmings.nxlv";
        // "levels\\CuttingItClose.nxlv";
        // "levels\\scrollTest.nxlv";
        // "levels\\LemRunner\\Mona\\ACaeloUsqueAdCentrum.nxlv";
        // "levels\\groupTest.nxlv";
        // "levels\\eraseTest.nxlv";
        // "levels\\Amiga Lemmings\\Lemmings\\Fun\\19_Take_good_care_of_my_Lemmings.nxlv";

        return Path.Combine(RootDirectoryManager.RootDirectory, file);
    }

    public MainPage CreateMainPage()
    {
        return new MainPage(_desktop, _inputController);
    }

    public LevelSelectPage CreateLevelSelectPage()
    {
        return new LevelSelectPage(_desktop, _inputController);
    }

    public LevelStartPage? CreateLevelStartPage()
    {
        LevelStartPage? result = null;
        LevelBuilder? levelBuilder = null;
        ILevelReader? levelReader = null;
        try
        {
            var fileExtension = Path.GetExtension(LevelToLoadFilepath);
            levelReader = LevelFileTypeHandler.GetLevelReaderForFileExtension(fileExtension);
            var levelData = levelReader.ReadLevel(LevelToLoadFilepath, _graphicsDevice);

            levelData.Validate();

            levelBuilder = new LevelBuilder(_contentManager, _graphicsDevice);
            var levelScreen = levelBuilder.BuildLevel(levelData);
            result = new LevelStartPage(_desktop, _inputController, levelScreen);
        }
        catch (Exception ex)
        {
            var exceptionWindow = new ExceptionViewer(_desktop, _inputController, ex);

            exceptionWindow.Initialise();
        }
        finally
        {
            levelReader?.Dispose();
            levelBuilder?.Dispose();
        }

        return result;
    }
}