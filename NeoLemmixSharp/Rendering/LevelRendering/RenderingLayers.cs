namespace NeoLemmixSharp.Rendering.LevelRendering;

public static class RenderingLayers
{
    /// <summary>
    /// These layer consts need to be between zero and one. 
    /// </summary>
    private const float ScaleFactor = 1f / 32f;

    public const float BackgroundLayer = ScaleFactor * 1f;
    public const float BehindTerrainGadgetsLayer = ScaleFactor * 2f;
    public const float TerrainLayer = ScaleFactor * 3f;
    public const float InFrontOfTerrainGadgetsLayer = ScaleFactor * 4f;
    public const float LemmingRenderLayer = ScaleFactor * 5f;
    public const float AthleteRenderLayer = ScaleFactor * 6f;

    public const float ControlPanelBackgroundLayer = ScaleFactor * 8f;
    public const float ControlPanelButtonLayer = ScaleFactor * 9f;
    public const float ControlPanelSkillCountEraseLayer = ScaleFactor * 10f;
    public const float ControlPanelSkillIconLayer = ScaleFactor * 11f;
    public const float ControlPanelSkillCountLayer = ScaleFactor * 12f;

    public const float CursorLayer = ScaleFactor * 15f;
}