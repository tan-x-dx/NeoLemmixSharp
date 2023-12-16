namespace NeoLemmixSharp.Engine.Rendering.Ui;

public enum ControlPanelTexture
{
    LemmingAnchorTexture,
    WhitePixel,

    PanelEmptySlot,
    PanelIconCpmAndReplay,
    PanelIconDirectional,
    PanelIconFastForward,
    PanelIconFrameskip,
    PanelIconNuke,
    PanelIconPause,
    PanelIconRestart,
    PanelIconReleaseRateMinus,
    PanelIconReleaseRatePlus,
    PanelMinimapRegion,
    PanelPanelIcons,
    PanelSkillCountErase,
    PanelSkillPanels,
    PanelSkillSelected,

    CursorStandard,
    CursorFocused,
}

public static class ControlPanelStringHelper
{
    public static string GetTexturePath(this ControlPanelTexture texture) => texture switch
    {
        ControlPanelTexture.LemmingAnchorTexture => "LemmingAnchorTexture",
        ControlPanelTexture.WhitePixel => "WhitePixel",

        ControlPanelTexture.PanelEmptySlot => "panel/empty_slot",
        ControlPanelTexture.PanelIconCpmAndReplay => "panel/icon_cpm_and_replay",
        ControlPanelTexture.PanelIconDirectional => "panel/icon_directional",
        ControlPanelTexture.PanelIconFastForward => "panel/icon_ff",
        ControlPanelTexture.PanelIconFrameskip => "panel/icon_frameskip",
        ControlPanelTexture.PanelIconNuke => "panel/icon_nuke",
        ControlPanelTexture.PanelIconPause => "panel/icon_pause",
        ControlPanelTexture.PanelIconRestart => "panel/icon_restart",
        ControlPanelTexture.PanelIconReleaseRateMinus => "panel/icon_rr_minus",
        ControlPanelTexture.PanelIconReleaseRatePlus => "panel/icon_rr_plus",
        ControlPanelTexture.PanelMinimapRegion => "panel/minimap_region",
        ControlPanelTexture.PanelPanelIcons => "panel/panel_icons",
        ControlPanelTexture.PanelSkillCountErase => "panel/skill_count_erase",
        ControlPanelTexture.PanelSkillPanels => "panel/skill_panels",
        ControlPanelTexture.PanelSkillSelected => "panel/skill_selected",

        ControlPanelTexture.CursorStandard => "cursor/standard",
        ControlPanelTexture.CursorFocused => "cursor/focused",

        _ => throw new ArgumentOutOfRangeException(nameof(texture), texture, "Unknown texture")
    };
}