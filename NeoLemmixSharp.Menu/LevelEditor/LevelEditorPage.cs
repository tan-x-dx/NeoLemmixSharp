using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;
using NeoLemmixSharp.Menu.LevelEditor.Components.StylePieces;
using NeoLemmixSharp.Menu.LevelEditor.Menu;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class LevelEditorPage : PageBase
{
    private LevelEditorMenuBar _topPanel;
    private LevelEditorControlPanel _leftPanel;
    private LevelEditorCanvas _levelCanvas;
    private PieceBank _pieceBank;

    private StyleData _levelStyleData;
    private StyleData _pieceStyleData;

    private LevelData _currentLevelData;

    public bool IsNeoLemmix => _currentLevelData.FileFormatType == IO.FileFormats.FileFormatType.NeoLemmix;

    public LevelEditorPage(MenuInputController menuInputController, GraphicsDevice graphicsDevice) : base(menuInputController)
    {
        _topPanel = new LevelEditorMenuBar();
        _leftPanel = new LevelEditorControlPanel();
        _levelCanvas = new LevelEditorCanvas(graphicsDevice);
        _pieceBank = new PieceBank(OnSelectTerrainPiece, OnSelectGadgetPiece, OnSelectBackgroundPiece);

        SetLevelData(CreateBlankLevelData());
    }

    protected override void OnInitialise()
    {
        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = false;

        var root = UiHandler.RootComponent;
        root.IsVisible = false;
        root.AddComponent(_topPanel);
        root.AddComponent(_leftPanel);
        root.AddComponent(_levelCanvas);
        root.AddComponent(_pieceBank);

        var styleData = StyleCache.GetOrLoadStyleData(new StyleFormatPair(new StyleIdentifier("dex_brass"), IO.FileFormats.FileFormatType.NeoLemmix));
        SetStyle(styleData);

        OnResize();
    }

    private void SetLevelData(LevelData levelData)
    {
        _currentLevelData = levelData;
        _levelCanvas.SetLevelData(levelData);
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
        OnResize();
    }

    private void OnResize()
    {
        var windowSize = IGameWindow.Instance.WindowSize;

        _topPanel.Left = 0;
        _topPanel.Top = 0;
        _topPanel.Width = windowSize.W;

        _leftPanel.Left = 0;
        _leftPanel.Top = _topPanel.Height;
        _leftPanel.Height = windowSize.H - _topPanel.Height - _pieceBank.Height;

        _pieceBank.Left = 0;
        _pieceBank.Top = windowSize.H - _pieceBank.Height;
        _pieceBank.Width = windowSize.W;

        _levelCanvas.Left = _leftPanel.Width;
        _levelCanvas.Top = _topPanel.Height;
        _levelCanvas.Width = windowSize.W - _leftPanel.Width;
        _levelCanvas.Height = windowSize.H - _topPanel.Height - _pieceBank.Height;

        _levelCanvas.OnCanvasResize();
        _pieceBank.OnResize();
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed)
        {
            IGameWindow.Instance.Escape();
        }

        _levelCanvas.HandleUserInput(InputController);
        _pieceBank.HandleUserInput(InputController);
    }

    private void SetStyle(StyleData styleData)
    {
        _levelStyleData = styleData;
        _pieceStyleData = styleData;

        _pieceBank.SetStyle(styleData);
    }

    private void LoadLevel(string levelFilePath)
    {
        var levelData = IO.FileFormats.FileTypeHandler.ReadLevel(levelFilePath);

        StyleCache.EnsureStylesAreLoadedForLevel(levelData);
        SetLevelData(levelData);

        var styleData = StyleCache.GetOrLoadStyleData(levelData.GetStyleFormatPair());
        SetStyle(styleData);
    }

    private void SaveLevel(string levelFilePath)
    {
        _currentLevelData.IncrementVersion();

        IO.FileFormats.FileTypeHandler.WriteLevel(_currentLevelData, levelFilePath);
    }

    protected override void OnDispose()
    {
        if (IsDisposed)
            return;

        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = true;

        _topPanel = null!;
        _leftPanel = null!;
        _levelCanvas = null!;
        _pieceBank = null!;
    }

    private void OnSelectTerrainPiece(Component c, Point pos)
    {
        if (c is not PieceSelector pieceSelector)
            return;

        if (pieceSelector.StylePiece is TerrainArchetypeData terrainArchetypeData)
        {
            _levelCanvas.AddTerrainPiece(terrainArchetypeData);
        }
    }

    private void OnSelectGadgetPiece(Component c, Point pos)
    {
        if (c is not PieceSelector pieceSelector)
            return;

        if (pieceSelector.StylePiece is GadgetArchetypeData gadgetArchetypeData)
        {
            _levelCanvas.AddGadgetPiece(gadgetArchetypeData);
        }
    }

    private void OnSelectBackgroundPiece(Component c, Point pos)
    {

    }

    private static LevelData CreateBlankLevelData()
    {
        var result = new LevelData(IO.FileFormats.FileFormatType.NeoLemmix);
        result.SetLevelWidth(320);
        result.SetLevelHeight(160);
        result.LevelId = new LevelIdentifier((ulong)Random.Shared.NextInt64());

        return result;
    }
}
