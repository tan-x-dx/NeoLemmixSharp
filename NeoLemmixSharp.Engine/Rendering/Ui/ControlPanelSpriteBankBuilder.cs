using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public static class ControlPanelSpriteBankBuilder
{
    public static ControlPanelSpriteBank BuildControlPanelSpriteBank(ContentManager contentManager)
    {
        var result = new ControlPanelSpriteBank
        {
            Panel = contentManager.Load<Texture2D>("panel/panels"),
            PanelMinimapRegion = contentManager.Load<Texture2D>("panel/minimap_region"),
            PanelIcons = contentManager.Load<Texture2D>("panel/panel_icons"),
            PanelSkillSelected = contentManager.Load<Texture2D>("panel/skill_selected"),
            PanelSkills = contentManager.Load<Texture2D>("panel/skills_placeholder2"),
        };

        return result;
    }
}