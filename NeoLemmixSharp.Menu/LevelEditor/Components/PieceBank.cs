using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Menu.LevelEditor.Components;

public sealed class PieceBank : Component, IComparer<PieceSelector>
{
    private readonly List<PieceSelector> _terrainPieces = [];
    private readonly List<PieceSelector> _gadgetPieces = [];
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
        _terrainPieces.Clear();
        _terrainPieces.EnsureCapacity(styleData.AllDefinedTerrainArchetypeData.Count);

        var x = UiConstants.TwiceStandardInset + PieceSelector.BaseSpriteRenderDimension;

        foreach (var terrainArchetypeData in styleData.AllDefinedTerrainArchetypeData)
        {
            var terrainPieceSelector = new PieceSelector(terrainArchetypeData);
            terrainPieceSelector.MouseDown.RegisterMouseEvent(_onSelectTerrainPiece);
            terrainPieceSelector.Left = x;
            terrainPieceSelector.Top = UiConstants.TwiceStandardInset;
            x += UiConstants.StandardInset + PieceSelector.BaseSpriteRenderDimension;
            AddComponent(terrainPieceSelector);
            _terrainPieces.Add(terrainPieceSelector);
        }

        _terrainPieces.Sort(this);
    }

    private void UpdateGadgetPieces(StyleData styleData)
    {
        _gadgetPieces.Clear();
        _gadgetPieces.EnsureCapacity(styleData.AllDefinedGadgetArchetypeData.Count);

        var x = UiConstants.TwiceStandardInset + PieceSelector.BaseSpriteRenderDimension;

        foreach (var gadgetArchetypeData in styleData.AllDefinedGadgetArchetypeData)
        {
            var gadgetPieceSelector = new PieceSelector(gadgetArchetypeData);
            gadgetPieceSelector.MouseDown.RegisterMouseEvent(_onSelectGadgetPiece);
            gadgetPieceSelector.Left = x;
            gadgetPieceSelector.Top = UiConstants.TwiceStandardInset;
            x += UiConstants.StandardInset + PieceSelector.BaseSpriteRenderDimension;
            AddComponent(gadgetPieceSelector);
            _gadgetPieces.Add(gadgetPieceSelector);
        }

        _gadgetPieces.Sort(this);
    }

    private void UpdateBackgroundPieces(StyleData styleData)
    {
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        RenderComponent(spriteBatch);
        RenderPieceSelectors(spriteBatch);
    }

    protected override void RenderComponent(SpriteBatch spriteBatch)
    {
        UiSprites.DrawBeveledRectangle(spriteBatch, this);
    }

    private void RenderPieceSelectors(SpriteBatch spriteBatch)
    {
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
