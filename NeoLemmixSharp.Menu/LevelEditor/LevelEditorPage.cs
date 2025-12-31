using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.Menu.LevelEditor.Components;
using NeoLemmixSharp.Menu.LevelEditor.Ui;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class LevelEditorPage : PageBase
{
    private readonly LevelCanvas _levelCanvas;

    private Tab _topPanel;
    private Tab _leftPanel;
    private PieceBank _pieceBank;

    private StyleData _levelStyleData;
    private StyleData _pieceStyleData;

    private LevelData CurrentLevelData
    {
        get => field;
        set
        {
            field = value;
            _levelCanvas.LevelData = value;
        }
    }

    public bool IsNeoLemmix => CurrentLevelData.FileFormatType == IO.FileFormats.FileFormatType.NeoLemmix;

    public LevelEditorPage(MenuInputController menuInputController) : base(menuInputController)
    {
        _levelCanvas = new LevelCanvas(0, 0, 64, 64);
        _pieceBank = new PieceBank(OnSelectTerrainPiece, OnSelectGadgetPiece, OnSelectBackgroundPiece);

        CurrentLevelData = new LevelData(IO.FileFormats.FileFormatType.Default);
    }

    protected override void OnInitialise()
    {
        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = false;

        var root = UiHandler.RootComponent;
        root.IsVisible = false;
        root.AddComponent(_levelCanvas);

        BuildLevelEditorUI(root);

        var styleData = StyleCache.GetOrLoadStyleData(new("dex_brass", IO.FileFormats.FileFormatType.NeoLemmix));
        SetStyle(styleData);

        OnResize();
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
        OnResize();
    }

    private void OnResize()
    {
        LevelEditorUiHelper.OnResizeLevelEditor(_topPanel, _leftPanel, _pieceBank, _levelCanvas);

        _levelCanvas.OnResize();
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
        CurrentLevelData = levelData;

        var styleData = StyleCache.GetOrLoadStyleData(levelData.GetStyleFormatPair());
        SetStyle(styleData);
    }

    private void SaveLevel(string levelFilePath)
    {
        CurrentLevelData.IncrementVersion();

        IO.FileFormats.FileTypeHandler.WriteLevel(CurrentLevelData, levelFilePath);
    }

    protected override void OnDispose()
    {
        if (IsDisposed)
            return;

        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = true;

        _topPanel = null!;
        _leftPanel = null!;
        _pieceBank = null!;

        _levelStyleData = null!;
        _pieceStyleData = null!;

        CurrentLevelData = null!;
    }

    private void BuildLevelEditorUI(Component root)
    {
        if (IsInitialised)
            return;

        _topPanel = new Tab(0, 0, 10, 10);
        root.AddComponent(_topPanel);

        _leftPanel = LevelEditorUiHelper.BuildLeftTab();
        root.AddComponent(_leftPanel);

        root.AddComponent(_pieceBank);
    }

    private void OnSelectTerrainPiece(Component c, Point pos)
    {

    }

    private void OnSelectGadgetPiece(Component c, Point pos)
    {

    }

    private void OnSelectBackgroundPiece(Component c, Point pos)
    {

    }
}
