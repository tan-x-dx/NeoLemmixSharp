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

        LevelToLoadFilepath = Path.Combine(RootDirectoryManager.RootDirectory, "levels\\movement test.nxlv");
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