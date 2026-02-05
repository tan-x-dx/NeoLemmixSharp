using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Level.Terrain;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;

public sealed partial class LevelEditorCanvas : IComparer<CanvasPiece>
{
    public void AddTerrainPiece(TerrainArchetypeData terrainArchetypeData)
    {
        var defaultArchetypeSize = terrainArchetypeData.DefaultSize;

        var initialWidth = defaultArchetypeSize.W;
        var initialHeight = defaultArchetypeSize.H;

        var newTerrainData = new TerrainInstanceData()
        {
            GroupName = null,
            StyleIdentifier = terrainArchetypeData.StyleIdentifier,
            PieceIdentifier = terrainArchetypeData.PieceIdentifier,
            Position = GetCenterPositionOfViewport(),
            Orientation = Orientation.Down,
            FacingDirection = FacingDirection.Right,
            NoOverwrite = false,
            Tint = Color.White,
            Erase = false,
            HueAngle = 0,
            Width = initialWidth,
            Height = initialHeight,
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

    int IComparer<CanvasPiece>.Compare(CanvasPiece? x, CanvasPiece? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        if (x.GetType() != y.GetType())
            throw new InvalidOperationException("Items are not the same type!");

        var xPieceOrder = x.PieceOrder;
        var yPieceOrder = y.PieceOrder;

        if (xPieceOrder == yPieceOrder)
            throw new InvalidOperationException($"Different {nameof(CanvasPiece)}s have same {nameof(CanvasPiece.PieceOrder)}");

        return xPieceOrder.CompareTo(yPieceOrder);
    }
}
