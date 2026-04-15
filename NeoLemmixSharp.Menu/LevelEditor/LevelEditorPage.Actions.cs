using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Terrain;
using NeoLemmixSharp.Menu.LevelEditor.Components.StylePieces;
using NeoLemmixSharp.Ui.Components;

namespace NeoLemmixSharp.Menu.LevelEditor;

public sealed partial class LevelEditorPage : IEditorOperationHandler
{
    public void OnNewLevel(Component c, Point position)
    {
        SetLevelData(CreateBlankLevelData());
    }

    public void OnFileOpen(Component c, Point position)
    {
    }

    public void OnSaveLevel(Component c, Point position)
    {
        if (string.IsNullOrWhiteSpace(_currentLevelData.LevelFilePath))
        {
            OnSaveLevelAs(c, position);
        }
        else
        {
            SaveLevel(_currentLevelData.LevelFilePath);
        }
    }

    public void OnSaveLevelAs(Component c, Point position)
    {
    }

    public void OnExit(Component c, Point position)
    {
        var mainPage = MenuScreen.Instance.MenuPageCreator.CreateMainPage();

        if (mainPage is null)
            return;

        MenuScreen.Instance.SetNextPage(mainPage);
    }

    public void EditorUndo(Component c, Point position)
    {
    }

    public void EditorRedo(Component c, Point position)
    {
    }

    public void EditorCut(Component c, Point position)
    {
    }

    public void EditorCopy(Component c, Point position)
    {
    }

    public void EditorPaste(Component c, Point position)
    {
    }

    public void EditorPasteInPlace(Component c, Point position)
    {
    }

    public void EditorDuplicate(Component c, Point position)
    {
    }

    public void EditorGroup(Component c, Point position)
    {
    }

    public void EditorUngroup(Component c, Point position)
    {
    }

    public void SelectTerrainPiece(Component c, Point pos)
    {
        if (c is not PieceSelector pieceSelector)
            return;

        if (pieceSelector.StylePiece is TerrainArchetypeData terrainArchetypeData)
        {
            _levelCanvas.AddTerrainPiece(terrainArchetypeData);
        }
    }

    public void SelectGadgetPiece(Component c, Point pos)
    {
        if (c is not PieceSelector pieceSelector)
            return;

        if (pieceSelector.StylePiece is GadgetArchetypeData gadgetArchetypeData)
        {
            _levelCanvas.AddGadgetPiece(gadgetArchetypeData);
        }
    }

    public void SelectBackgroundPiece(Component c, Point pos)
    {

    }

    public void ToggleClearPhysics(Component c)
    {
        var checkBox = (CheckBox)c;

        if (checkBox.IsChecked)
        {

        }
        else
        {

        }
    }

    public void ToggleTerrainRendering(Component c)
    {

    }

    public void ToggleGadgetRendering(Component c)
    {

    }

    public void ToggleTriggerAreaRendering(Component c)
    {

    }

    public void ToggleScreenStartRendering(Component c)
    {

    }

    public void ToggleBackgroundRendering(Component c)
    {

    }

    public void ToggleDeprecatedPieces(Component c)
    {

    }

    public void ToggleSnapToGrid(Component c)
    {

    }

    public void TestLevel(Component c, Point position)
    {

    }

    public void ValidateLevel(Component c, Point position)
    {

    }

    public void ViewSettings(Component c, Point position)
    {

    }

    public void ViewHotKeySettings(Component c, Point position)
    {

    }

    public void ViewAbout(Component c, Point position)
    {

    }
}
