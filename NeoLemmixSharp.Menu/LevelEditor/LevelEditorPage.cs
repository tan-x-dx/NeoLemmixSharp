using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.BoundaryBehaviours;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Objectives;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.Menu.LevelEditor.Components.Canvas;
using NeoLemmixSharp.Menu.LevelEditor.Components.StylePieces;
using NeoLemmixSharp.Menu.LevelEditor.Menu;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed class LevelEditorPage : PageBase
{
    private LevelEditorMenuBar _menuBar;
    private LevelEditorControlPanel _controlPanel;
    private LevelEditorCanvas _levelCanvas;
    private PieceBank _pieceBank;

    private StyleData _levelStyleData;
    private StyleData _pieceStyleData;

    private LevelData _currentLevelData;

    public bool IsNeoLemmix => _currentLevelData.FileFormatType == FileFormatType.NeoLemmix;

    public LevelEditorPage(MenuInputController menuInputController, GraphicsDevice graphicsDevice) : base(menuInputController)
    {
        _menuBar = new LevelEditorMenuBar();
        _controlPanel = new LevelEditorControlPanel();
        _levelCanvas = new LevelEditorCanvas(graphicsDevice, menuInputController.InputController);
        _pieceBank = new PieceBank(OnSelectTerrainPiece, OnSelectGadgetPiece, OnSelectBackgroundPiece);

        SetLevelData(CreateBlankLevelData());
    }

    protected override void OnInitialise()
    {
        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = false;

        var root = UiHandler.RootComponent;
        root.IsVisible = false;
        root.AddComponent(_levelCanvas);
        root.AddComponent(_menuBar);
        root.AddComponent(_controlPanel);
        root.AddComponent(_pieceBank);

        SetUpHandlers();

        // LoadLevel(RootDirectoryManager.GetLevelFilePath(@"Amiga Lemmings\Lemmings\Fun\21_You_Live_and_Lem", FileFormatType.NeoLemmix));
        // LoadLevel(RootDirectoryManager.GetLevelFilePath(@"Amiga Lemmings\Lemmings\Tricky\04_Here's_one_I_prepared_earlier", FileFormatType.NeoLemmix));
        LoadLevel(RootDirectoryManager.GetLevelFilePath("skill test", FileFormatType.NeoLemmix));

        // var styleData = StyleCache.GetOrLoadStyleData(new StyleFormatPair(new StyleIdentifier("orig_dirt"), FileFormatType.NeoLemmix));
        // SetStyle(styleData);

        OnResize();
    }

    private void SetLevelData(LevelData levelData)
    {
        _currentLevelData = levelData;
        _controlPanel.SetLevelData(levelData);
        _levelCanvas.SetLevelData(levelData);
    }

    protected override void OnWindowDimensionsChanged(Size windowSize)
    {
        OnResize();
    }

    private void OnResize()
    {
        var windowSize = IGameWindow.Instance.WindowSize;

        _menuBar.Left = 0;
        _menuBar.Top = 0;
        _menuBar.Width = windowSize.W;

        _controlPanel.Left = 0;
        _controlPanel.Top = _menuBar.Height;
        _controlPanel.Height = windowSize.H - _menuBar.Height - _pieceBank.Height;

        _pieceBank.Left = 0;
        _pieceBank.Top = windowSize.H - _pieceBank.Height;
        _pieceBank.Width = windowSize.W;

        _levelCanvas.Left = _controlPanel.Width;
        _levelCanvas.Top = _menuBar.Height;
        _levelCanvas.Width = windowSize.W - _controlPanel.Width;
        _levelCanvas.Height = windowSize.H - _menuBar.Height - _pieceBank.Height;

        _levelCanvas.OnCanvasResize();
        _pieceBank.OnResize();
    }

    protected override void HandleUserInput()
    {
        if (InputController.Quit.IsPressed && UiHandler.SelectedTextField is null)
        {
            IGameWindow.Instance.Escape();
        }

        _levelCanvas.HandleUserInput(InputController);
        _pieceBank.HandleUserInput(InputController);

        if (InputController.F1.IsPressed)
        {
            SaveLevel($@"C:\Temp\{_currentLevelData.LevelTitle}.ullv");
        }

        if (InputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleFullscreenSetting();
        }
    }

    private void SetStyle(StyleData styleData)
    {
        _levelStyleData = styleData;
        _pieceStyleData = styleData;

        _pieceBank.SetStyle(styleData);
    }

    private void LoadLevel(string levelFilePath)
    {
        var levelData = FileTypeHandler.ReadLevel(levelFilePath);

        StyleCache.EnsureStylesAreLoadedForLevel(levelData);
        SetLevelData(levelData);

        var styleData = StyleCache.GetOrLoadStyleData(levelData.GetStyleFormatPair());
        SetStyle(styleData);
    }

    private void SaveLevel(string levelFilePath)
    {
        _currentLevelData.IncrementVersion();

        FileTypeHandler.WriteLevel(_currentLevelData, levelFilePath);
    }

    protected override void OnDispose()
    {
        if (IsDisposed)
            return;

        MenuScreen.Instance.MenuScreenRenderer.RenderBackground = true;

        _menuBar = null!;
        _controlPanel = null!;
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

    private void SetUpHandlers()
    {
        _controlPanel.TitleTextField.TextSubmit.RegisterEvent(SetLevelTitle);
        _controlPanel.AuthorTextField.TextSubmit.RegisterEvent(SetLevelAuthor);
        _controlPanel.MusicTextField.TextSubmit.RegisterEvent(SetLevelMusic);

        _controlPanel.LevelWidthTextField.TextSubmit.RegisterEvent(SetLevelWidth);
        _controlPanel.LevelHeightTextField.TextSubmit.RegisterEvent(SetLevelHeight);

        _controlPanel.LevelIdTextField.TextSubmit.RegisterEvent(SetLevelId);
        _controlPanel.GenerateNewLevelIdButton.MouseReleased.RegisterMouseEvent(GenerateNewLevelId);

        _controlPanel.WrapHorizontalCheckBox.OnChecked.RegisterEvent(SetWrapHorizontal);
        _controlPanel.WrapHorizontalCheckBox.OnUnchecked.RegisterEvent(SetWrapHorizontal);
        _controlPanel.WrapVerticalCheckBox.OnChecked.RegisterEvent(SetWrapVertical);
        _controlPanel.WrapVerticalCheckBox.OnUnchecked.RegisterEvent(SetWrapVertical);
    }

    private void SetLevelTitle(Component c)
    {
        var textField = (TextField)c;
        _currentLevelData.LevelTitle = textField.CurrentString;
    }

    private void SetLevelAuthor(Component c)
    {
        var textField = (TextField)c;
        _currentLevelData.LevelAuthor = textField.CurrentString;
    }

    private void SetLevelMusic(Component c)
    {
        var textField = (TextField)c;
        //_currentLevelData.Music = textField.CurrentString;
    }

    private void SetLevelWidth(Component c)
    {
        var textField = (TextField)c;

        var currentLevelDimensions = _currentLevelData.LevelDimensions;
        var newLevelWidth = EvaluateLevelDimensionFromTextField(textField);

        if (currentLevelDimensions.W == newLevelWidth)
            return;

        _currentLevelData.SetLevelWidth(newLevelWidth);
        _controlPanel.SetNumericalLevelData(_currentLevelData);
        _levelCanvas.OnLevelDataChanged();
    }

    private void SetLevelHeight(Component c)
    {
        var textField = (TextField)c;

        var currentLevelDimensions = _currentLevelData.LevelDimensions;
        var newLevelHeight = EvaluateLevelDimensionFromTextField(textField);

        if (currentLevelDimensions.H == newLevelHeight)
            return;

        _currentLevelData.SetLevelHeight(newLevelHeight);
        _controlPanel.SetNumericalLevelData(_currentLevelData);
        _levelCanvas.OnLevelDataChanged();
    }

    private static int EvaluateLevelDimensionFromTextField(TextField textField)
    {
        if (textField.IsBlank())
            return EngineConstants.MinLevelSize;

        var newLevelSize = textField.ParseInt();

        if (newLevelSize > EngineConstants.MaxLevelSize)
            newLevelSize = EngineConstants.MaxLevelSize;

        return newLevelSize;
    }

    private void SetLevelId(Component c)
    {
        var textField = (TextField)c;

        var newLevelId = ulong.Parse(textField.CurrentTextSpan, System.Globalization.NumberStyles.AllowHexSpecifier, null);
        _currentLevelData.LevelId = new LevelIdentifier(newLevelId);
    }

    private void GenerateNewLevelId(Component c, Point position)
    {
        var newLevelId = (ulong)Random.Shared.NextInt64();
        _currentLevelData.LevelId = new LevelIdentifier(newLevelId);
        _controlPanel.SetNumericalLevelData(_currentLevelData);
    }

    private void SetWrapHorizontal(Component c)
    {
        var checkBox = (CheckBox)c;

        var newWrapType = checkBox.IsChecked
            ? BoundaryBehaviourType.Wrap
            : BoundaryBehaviourType.Void;

        _currentLevelData.HorizontalBoundaryBehaviour = newWrapType;
    }

    private void SetWrapVertical(Component c)
    {
        var checkBox = (CheckBox)c;

        var newWrapType = checkBox.IsChecked
            ? BoundaryBehaviourType.Wrap
            : BoundaryBehaviourType.Void;

        _currentLevelData.VerticalBoundaryBehaviour = newWrapType;
    }

    private static LevelData CreateBlankLevelData()
    {
        var result = new LevelData(FileFormatType.NeoLemmix);
        result.SetLevelWidth(320);
        result.SetLevelHeight(160);
        result.LevelId = new LevelIdentifier((ulong)Random.Shared.NextInt64());
        result.LevelStyle = StyleCache.GetOrLoadStyleData(new StyleFormatPair(new StyleIdentifier("orig_dirt"), FileFormatType.NeoLemmix)).Identifier;
        result.MaxNumberOfClonedLemmings = 0;

        var objective = new LevelObjectiveData()
        {
            ObjectiveName = "Save Lemmings",

            SkillSetData = [],
            ObjectiveCriteria = [new SaveLemmingsCriterionData { SaveRequirement = 20, TribeId = EngineConstants.ClassicTribeId }],
            ObjectiveModifiers = [],
            TalismanData = []
        };

        result.SetObjectiveData(objective);

        result.TribeIdentifiers.Add(new(result.LevelStyle, EngineConstants.ClassicTribeId));

        return result;
    }
}
