using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelEntry : LevelBrowserEntry
{
    private readonly string _filePath;

    private string _displayData = EngineConstants.LevelLoadingDisplayString;
    private LevelData? _levelData;

    public override string DisplayName => _levelData?.LevelTitle ?? _displayData;
    public bool IsLoading => _levelData is null;

    public LevelEntry(
        string filePath,
        int indentationLevel)
        : base(indentationLevel)
    {
        _filePath = filePath;
    }

    public void LoadLevelData(GraphicsDevice graphicsDevice)
    {
        ILevelReader? levelReader = null;
        try
        {
            var fileExtension = Path.GetExtension(_filePath.AsSpan());
            levelReader = LevelFileTypeHandler.GetLevelReaderForFileExtension(fileExtension);
            _levelData = levelReader.ReadLevel(_filePath, graphicsDevice);
        }
        catch
        {
            _displayData = EngineConstants.LevelLoadingErrorOccurredDisplayString;
            _levelData = null;
        }
        finally
        {
            levelReader?.Dispose();
        }
    }

    public override IEnumerable<LevelBrowserEntry> GetSubEntries()
    {
        yield return this;
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawStateColoredBeveledRectangle(spriteBatch, this);
        FontBank.MenuFont.RenderText(
            spriteBatch,
            DisplayName,
            Left + ButtonPadding,
            Top + ButtonPadding,
            1,
            EngineConstants.PanelBlue);
    }

    protected override void OnDispose()
    {
        _levelData = null;
    }
}