﻿using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.Ui.Data;

namespace NeoLemmixSharp.Menu.Pages.LevelSelect;

public sealed class LevelEntry : LevelBrowserEntry
{
    private readonly string _filePath;

    private string _displayData = EngineConstants.LevelLoadingDisplayString;
    private bool _hasErrored;

    public LevelData? LevelData { get; private set; }
    public override string DisplayName => LevelData?.LevelTitle ?? _displayData;
    public bool IsLoading => !_hasErrored && LevelData is null;

    public LevelEntry(
        string filePath,
        int indentationLevel)
        : base(indentationLevel)
    {
        _filePath = filePath;
    }

    public void LoadLevelData()
    {
        try
        {
            LevelData = FileTypeHandler.ReadLevel(_filePath);
        }
        catch
        {
            _displayData = EngineConstants.LevelLoadingErrorOccurredDisplayString;
            LevelData = null;
            _hasErrored = true;
        }
        finally
        {
        }
    }

    public override IEnumerable<LevelBrowserEntry> GetSubEntries()
    {
        yield return this;
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
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
        LevelData = null;
    }
}