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
    private readonly SpriteBatch _spriteBatch;

    private readonly MenuInputController _inputController;

    public string LevelToLoadFilepath { get; set; }

    public MenuPageCreator(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice,
        SpriteBatch spriteBatch,
        MenuInputController inputController)
    {
        _contentManager = contentManager;
        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _inputController = inputController;

        LevelToLoadFilepath = GetLevelFilePath();
    }

    private static string GetLevelFilePath()
    {
        var file =
         // "levels\\tanxdx_TheTreacheryOfLemmings_R3V1.nxlv";
         // "levels\\rotation test.nxlv";
         // "levels\\render test.nxlv";
         // "levels\\movement test.nxlv";
          "levels\\object test.nxlv";
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
        return new MainPage(_inputController);
    }

    public LevelSelectPage CreateLevelSelectPage()
    {
        return new LevelSelectPage(_inputController);
    }

    public LevelStartPage CreateLevelStartPage()
    {
        var fileExtension = Path.GetExtension(LevelToLoadFilepath.AsSpan());
        var levelReader = LevelFileTypeHandler.GetLevelReaderForFileExtension(fileExtension);

        using var levelBuilder = new LevelBuilder(_contentManager, _graphicsDevice, _spriteBatch, levelReader);
        var level = levelBuilder.BuildLevel(LevelToLoadFilepath);

        return new LevelStartPage(IGameWindow.Instance, _inputController, level);
    }
}