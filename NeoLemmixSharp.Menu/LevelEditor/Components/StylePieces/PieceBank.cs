using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.Ui.Components;
using NeoLemmixSharp.Ui.Data;
using NeoLemmixSharp.Ui.Events;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.StylePieces;

public sealed class PieceBank : Component, IComparer<PieceSelector>
{
    private const int InitialPieceOffset = UiConstants.TwiceStandardInset + PieceSelector.BaseSpriteRenderDimension;
    private const int PieceOffsetDelta = UiConstants.StandardInset + PieceSelector.BaseSpriteRenderDimension;

    private readonly List<PieceSelector> _terrainPieces = [];
    private readonly List<PieceSelector> _gadgetPieces = [];
    private readonly List<PieceSelector> _backgroundPieces = [];

    private readonly MouseEventHandler.ComponentMouseAction _onSelectTerrainPiece;
    private readonly MouseEventHandler.ComponentMouseAction _onSelectGadgetPiece;
    private readonly MouseEventHandler.ComponentMouseAction _onSelectBackgroundPiece;

    private PieceBankSelectionMode _selectionMode = PieceBankSelectionMode.Terrain;
    private int _scrollIndex;
    private int _numberOfPiecesToDisplay;

    public PieceBank(
        MouseEventHandler.ComponentMouseAction onSelectTerrainPiece,
        MouseEventHandler.ComponentMouseAction onSelectGadgetPiece,
        MouseEventHandler.ComponentMouseAction onSelectBackgroundPiece) : base(0, 0, string.Empty)
    {
        _onSelectTerrainPiece = onSelectTerrainPiece;
        _onSelectGadgetPiece = onSelectGadgetPiece;
        _onSelectBackgroundPiece = onSelectBackgroundPiece;

        Height = PieceSelector.BaseSpriteRenderDimension + 32;
    }

    public void SetStyle(StyleData styleData)
    {
        _scrollIndex = 0;
        UpdateTerrainPieces(styleData);
        UpdateGadgetPieces(styleData);
        UpdateBackgroundPieces(styleData);

        AdjustPieceOffsets();
    }

    private void UpdateTerrainPieces(StyleData styleData)
    {
        _terrainPieces.Clear();
        _terrainPieces.EnsureCapacity(styleData.AllDefinedTerrainArchetypeData.Count);

        var offset = InitialPieceOffset;

        foreach (var terrainArchetypeData in styleData.AllDefinedTerrainArchetypeData)
        {
            var terrainPieceSelector = new PieceSelector(terrainArchetypeData)
            {
                IsVisible = _selectionMode == PieceBankSelectionMode.Terrain
            };
            terrainPieceSelector.MousePressed.RegisterMouseEvent(_onSelectTerrainPiece);
            terrainPieceSelector.Left = offset;
            terrainPieceSelector.Top = UiConstants.TwiceStandardInset;
            offset += PieceOffsetDelta;
            AddComponent(terrainPieceSelector);
            _terrainPieces.Add(terrainPieceSelector);
        }

        _terrainPieces.Sort(this);
    }

    private void UpdateGadgetPieces(StyleData styleData)
    {
        _gadgetPieces.Clear();
        _gadgetPieces.EnsureCapacity(styleData.AllDefinedGadgetArchetypeData.Count);

        var offset = InitialPieceOffset;

        foreach (var gadgetArchetypeData in styleData.AllDefinedGadgetArchetypeData)
        {
            var gadgetPieceSelector = new PieceSelector(gadgetArchetypeData)
            {
                IsVisible = _selectionMode == PieceBankSelectionMode.Gadgets
            };
            gadgetPieceSelector.MousePressed.RegisterMouseEvent(_onSelectGadgetPiece);
            gadgetPieceSelector.Left = offset;
            gadgetPieceSelector.Top = UiConstants.TwiceStandardInset;
            offset += PieceOffsetDelta;
            AddComponent(gadgetPieceSelector);
            _gadgetPieces.Add(gadgetPieceSelector);
        }

        _gadgetPieces.Sort(this);
    }

    private void UpdateBackgroundPieces(StyleData styleData)
    {
    }

    public void OnResize()
    {
        _numberOfPiecesToDisplay = (Width - InitialPieceOffset) / PieceOffsetDelta;

        AdjustPieceOffsets();
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
        var pieceSelectorsToRender = GetCurrentPieceSelectors();

        foreach (var pieceSelector in pieceSelectorsToRender)
        {
            pieceSelector.Render(spriteBatch);
        }
    }

    private List<PieceSelector> GetCurrentPieceSelectors()
    {
        return _selectionMode switch
        {
            PieceBankSelectionMode.Terrain => _terrainPieces,
            PieceBankSelectionMode.Gadgets => _gadgetPieces,
            PieceBankSelectionMode.Background => _backgroundPieces,
            PieceBankSelectionMode.Sketches => [],

            _ => [],
        };
    }

    private void SetSelectionMode(PieceBankSelectionMode selectionMode)
    {
        if (_selectionMode == selectionMode)
            return;

        var currentlyDisplayedItems = GetCurrentPieceSelectors();
        foreach (var pieceSelector in currentlyDisplayedItems)
        {
            pieceSelector.IsVisible = false;
        }

        _selectionMode = selectionMode;
        _scrollIndex = 0;

        currentlyDisplayedItems = GetCurrentPieceSelectors();
        foreach (var pieceSelector in currentlyDisplayedItems)
        {
            pieceSelector.IsVisible = true;
        }
    }

    public void HandleUserInput(MenuInputController inputController)
    {
        if (!ContainsPoint(inputController.MousePosition))
            return;

        Scroll(inputController.ScrollDelta);
    }

    private void Scroll(int scrollDelta)
    {
        if (scrollDelta == 0)
            return;

        var currentlyDisplayedItems = GetCurrentPieceSelectors();
        if (currentlyDisplayedItems.Count == 0)
            return;

        _scrollIndex = Helpers.LogicalMod(_scrollIndex + scrollDelta, currentlyDisplayedItems.Count);

        AdjustPieceOffsets();
    }

    private void AdjustPieceOffsets()
    {
        var currentlyDisplayedItems = GetCurrentPieceSelectors();

        for (var i = 0; i < currentlyDisplayedItems.Count; i++)
        {
            var effectiveIndex = Helpers.LogicalMod(i + _scrollIndex, currentlyDisplayedItems.Count);
            var piece = currentlyDisplayedItems[effectiveIndex];
            piece.IsVisible = i < _numberOfPiecesToDisplay;
            piece.Top = Bottom - (PieceSelector.BaseSpriteRenderDimension + UiConstants.StandardInset);
            piece.Left = Left + InitialPieceOffset + (i * PieceOffsetDelta);

        }
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

public enum PieceBankSelectionMode
{
    Terrain,
    Gadgets,
    Background,
    Sketches
}
