using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas : IComparer<CanvasPiece>
{
    private readonly List<CanvasPiece> _terrainPieces = new(IoConstants.AssumedNumberOfTerrainInstanceDataInLevel);
    private readonly List<CanvasPiece> _gadgetPieces = new(IoConstants.AssumedNumberOfGadgetInstanceDataInLevel);
    private readonly List<CanvasPiece> _preplacedLemmingPieces = [];

    public void AddTerrainPiece(TerrainArchetypeData terrainArchetypeData)
    {
        var defaultArchetypeSize = terrainArchetypeData.DefaultSize;
        var initialPosition = GetCenterPositionOfViewport() - new Point(defaultArchetypeSize.W / 2, defaultArchetypeSize.H / 2);

        var newTerrainData = new TerrainInstanceData()
        {
            GroupName = null,
            StyleIdentifier = terrainArchetypeData.StyleIdentifier,
            PieceIdentifier = terrainArchetypeData.PieceIdentifier,
            Position = initialPosition,
            Orientation = Orientation.Down,
            FacingDirection = FacingDirection.Right,
            NoOverwrite = false,
            Tint = Color.White,
            Erase = false,
            HueAngle = 0,
            Width = defaultArchetypeSize.W,
            Height = defaultArchetypeSize.H,
        };

        var newCanvasTerrainPiece = new CanvasPiece(newTerrainData)
        {
            PieceOrder = _terrainPieces.Count
        };

        _terrainPieces.Add(newCanvasTerrainPiece);

        RepaintLevel();
    }

    public void AddGadgetPiece(GadgetArchetypeData gadgetArchetypeData)
    {
        var newGadgetData = new GadgetInstanceData()
        {
            Identifier = new GadgetIdentifier(_gadgetPieces.Count),
            OverrideName = GadgetName.Empty,
            StyleIdentifier = gadgetArchetypeData.StyleIdentifier,
            PieceIdentifier = gadgetArchetypeData.PieceIdentifier,
            Position = GetCenterPositionOfViewport(),
            GadgetRenderMode = Common.Enums.GadgetRenderMode.InFrontOfTerrain,
            Orientation = Orientation.Down,
            FacingDirection = FacingDirection.Right,
            IsFastForward = false,

            SpecificationData = CreateGadgetSpecificationData(gadgetArchetypeData),
        };

        var newCanvasGadgetPiece = new CanvasPiece(newGadgetData)
        {
            PieceOrder = _gadgetPieces.Count
        };

        _gadgetPieces.Add(newCanvasGadgetPiece);

        RepaintLevel();
    }

    private IGadgetInstanceSpecificationData CreateGadgetSpecificationData(GadgetArchetypeData gadgetArchetypeData)
    {
        throw new NotImplementedException();
    }

    private void ReorderAllPieces()
    {
        ReorderPieces(_gadgetPieces);
        ReorderPieces(_terrainPieces);
        ReorderPieces(_preplacedLemmingPieces);
    }

    private static void ReorderPieces(List<CanvasPiece> canvasPieces)
    {
        for (var i = 0; i < canvasPieces.Count; i++)
        {
            canvasPieces[i].PieceOrder = i;
        }
    }

    private void SortCanvasPieces()
    {
        _terrainPieces.Sort(this);
        _gadgetPieces.Sort(this);
        _preplacedLemmingPieces.Sort(this);
    }

    private void RepopulateLevelDataContents()
    {
        _levelData.AllGadgetInstanceData.Clear();
        foreach (var gadgetPiece in _gadgetPieces)
        {
            var gadgetInstanceData = (GadgetInstanceData)gadgetPiece.InstanceData;
            _levelData.AllGadgetInstanceData.Add(gadgetInstanceData);
        }

        _levelData.AllTerrainInstanceData.Clear();
        foreach (var terrainPiece in _terrainPieces)
        {
            var terrainInstanceData = (TerrainInstanceData)terrainPiece.InstanceData;
            _levelData.AllTerrainInstanceData.Add(terrainInstanceData);
        }

        _levelData.PrePlacedLemmingData.Clear();
        foreach (var lemmingPiece in _preplacedLemmingPieces)
        {
            var lemmingInstanceData = (LemmingInstanceData)lemmingPiece.InstanceData;
            _levelData.PrePlacedLemmingData.Add(lemmingInstanceData);
        }
    }

    private CanvasPiece? TrySelectSingleItem()
    {
        if (TrySelectSingleItemInList(_preplacedLemmingPieces, out var selectedPiece))
            return selectedPiece;

        if (TrySelectSingleItemInList(_gadgetPieces, out selectedPiece))
            return selectedPiece;

        if (TrySelectSingleItemInList(_terrainPieces, out selectedPiece))
            return selectedPiece;

        return null;
    }

    private bool TrySelectSingleItemInList(List<CanvasPiece> pieces, [MaybeNullWhen(false)] out CanvasPiece selectedPiece)
    {
        for (int i = pieces.Count - 1; i >= 0; i--)
        {
            var piece = pieces[i];

            if (piece.ContainsPoint(_canvasMouseDownPosition))
            {
                selectedPiece = piece;
                return true;
            }
        }

        selectedPiece = null;
        return false;
    }

    private void TrySelectMultipleItems(RectangularRegion clickDragBounds)
    {
        Debug.Assert(_selectedCanvasPieces.Count == 0);

        TrySelectMultipleItemsFromList(clickDragBounds, _preplacedLemmingPieces);
        TrySelectMultipleItemsFromList(clickDragBounds, _gadgetPieces);
        TrySelectMultipleItemsFromList(clickDragBounds, _terrainPieces);
    }

    private void TrySelectMultipleItemsFromList(RectangularRegion clickDragBounds, List<CanvasPiece> pieces)
    {
        foreach (var piece in pieces)
        {
            var pieceBounds = piece.GetBounds();
            if (clickDragBounds.Overlaps(pieceBounds))
            {
                _selectedCanvasPieces.Add(piece);
            }
        }
    }

    int IComparer<CanvasPiece>.Compare(CanvasPiece? x, CanvasPiece? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        if (x.InstanceData.GetType() != y.InstanceData.GetType())
            throw new InvalidOperationException("Items are not the same type!");

        var xPieceOrder = x.PieceOrder;
        var yPieceOrder = y.PieceOrder;

        if (xPieceOrder == yPieceOrder)
            throw new InvalidOperationException($"Different {nameof(CanvasPiece)}s have same {nameof(CanvasPiece.PieceOrder)}");

        return xPieceOrder.CompareTo(yPieceOrder);
    }
}
