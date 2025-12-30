using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Menu.LevelEditor.Components;

public sealed class PieceBank : Component, IComparer<PieceSelector>
{
    private readonly List<PieceSelector> _terrainPieces = new(IoConstants.AssumedNumberOfTerrainArchetypeDataInStyle);
    private readonly List<PieceSelector> _gadgetPieces = new(IoConstants.AssumedNumberOfGadgetArchetypeDataInStyle);
    private readonly List<PieceSelector> _backgroundPieces = [];

    private readonly MouseEventHandler.ComponentMouseAction _onSelectTerrainPiece;
    private readonly MouseEventHandler.ComponentMouseAction _onSelectGadgetPiece;
    private readonly MouseEventHandler.ComponentMouseAction _onSelectBackgroundPiece;

    public PieceBank(
        MouseEventHandler.ComponentMouseAction onSelectTerrainPiece,
        MouseEventHandler.ComponentMouseAction onSelectGadgetPiece,
        MouseEventHandler.ComponentMouseAction onSelectBackgroundPiece) : base(0, 0, string.Empty)
    {
        _onSelectTerrainPiece = onSelectTerrainPiece;
        _onSelectGadgetPiece = onSelectGadgetPiece;
        _onSelectBackgroundPiece = onSelectBackgroundPiece;
    }

    public void SetStyle(StyleData styleData)
    {
        UpdateTerrainPieces(styleData);
        UpdateGadgetPieces(styleData);
        UpdateBackgroundPieces(styleData);
    }

    private void UpdateTerrainPieces(StyleData styleData)
    {
        var terrainDirectory = RootDirectoryManager.GetStyleTerrainFolderPath(styleData.Identifier);

        _terrainPieces.Clear();
        _terrainPieces.EnsureCapacity(styleData.AllDefinedTerrainArchetypeData.Count);

        var x = UiConstants.TwiceStandardInset + PieceSelector.BaseSpriteRenderDimension;

        foreach (var terrainArchetypeData in styleData.AllDefinedTerrainArchetypeData)
        {
            var terrainPieceSelector = new PieceSelector(terrainArchetypeData, terrainDirectory);
            terrainPieceSelector.MouseDown.RegisterMouseEvent(_onSelectTerrainPiece);
            terrainPieceSelector.Left = x;
            terrainPieceSelector.Top = UiConstants.TwiceStandardInset;
            x += UiConstants.StandardInset + PieceSelector.BaseSpriteRenderDimension;
            _terrainPieces.Add(terrainPieceSelector);
        }

        _terrainPieces.Sort(this);
    }

    private void UpdateGadgetPieces(StyleData styleData)
    {
        var gadgetDirectory = RootDirectoryManager.GetStyleGadgetFolderPath(styleData.Identifier);

        _gadgetPieces.Clear();
        _gadgetPieces.EnsureCapacity(styleData.AllDefinedGadgetArchetypeData.Count);

        var x = UiConstants.TwiceStandardInset + PieceSelector.BaseSpriteRenderDimension;

        foreach (var gadgetArchetypeData in styleData.AllDefinedGadgetArchetypeData)
        {
            var gadgetPieceSelector = new PieceSelector(gadgetArchetypeData, gadgetDirectory);
            gadgetPieceSelector.MouseDown.RegisterMouseEvent(_onSelectGadgetPiece);
            gadgetPieceSelector.Left = x;
            gadgetPieceSelector.Top = UiConstants.TwiceStandardInset;
            x += UiConstants.StandardInset + PieceSelector.BaseSpriteRenderDimension;
            _gadgetPieces.Add(gadgetPieceSelector);
        }

        _gadgetPieces.Sort(this);
    }

    private void UpdateBackgroundPieces(StyleData styleData)
    {
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);

        var pieceSelectorsToRender = GetPieceSelectorsToRender();

        foreach (var pieceSelector in pieceSelectorsToRender)
        {
            pieceSelector.Render(spriteBatch);
        }
    }

    private List<PieceSelector> GetPieceSelectorsToRender()
    {
        return _terrainPieces;
    }

    int IComparer<PieceSelector>.Compare(PieceSelector? x, PieceSelector? y)
    {
        string? nameX = string.Empty;
        if (x is not null)
            nameX = x.Label;

        string? nameY = string.Empty;
        if (y is not null)
            nameY = y.Label;

        return string.Compare(nameX, nameY, StringComparison.Ordinal);
    }
}
