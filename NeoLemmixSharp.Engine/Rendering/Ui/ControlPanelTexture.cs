namespace NeoLemmixSharp.Engine.Rendering.Ui;

public enum ControlPanelTexture
{
    Panel,
    PanelMinimapRegion,
    PanelIcons,
    PanelSkillSelected,
    PanelSkills,
}

public static class ControlPanelStringHelper
{
    public static string GetTexturePath(this ControlPanelTexture texture) => texture switch
    {
        ControlPanelTexture.Panel => "panel/panels",
        ControlPanelTexture.PanelMinimapRegion => "panel/minimap_region",
        ControlPanelTexture.PanelIcons => "panel/panel_icons",
        ControlPanelTexture.PanelSkillSelected => "panel/skill_selected",
        ControlPanelTexture.PanelSkills => "panel/skills_placeholder2",

        _ => throw new ArgumentOutOfRangeException(nameof(texture), texture, "Unknown texture")
    };
}