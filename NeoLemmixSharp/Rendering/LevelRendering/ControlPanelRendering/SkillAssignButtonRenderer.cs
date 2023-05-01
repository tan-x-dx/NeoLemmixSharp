using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Rendering.Text;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public sealed class SkillAssignButtonRenderer : ControlPanelButtonRenderer
{
    private readonly SkillAssignButton _skillAssignButton;

    private readonly Texture2D _skillPanels;
    private readonly Texture2D _skillSelected;
    private readonly Texture2D _skillCountErase;

    private readonly INeoLemmixFont _skillCountDigitFont;

    public SkillAssignButtonRenderer(
        SpriteBank spriteBank,
        FontBank fontBank,
        SkillAssignButton skillAssignButton)
    {
        _skillCountErase = spriteBank.TextureLookup["panel/skill_count_erase"];
        _skillPanels = spriteBank.TextureLookup["panel/skill_panels"];
        _skillSelected = spriteBank.TextureLookup["panel/skill_selected"];

        _skillCountDigitFont = fontBank.SkillCountDigitFont;

        _skillAssignButton = skillAssignButton;
    }

    public override void RenderAtPosition(SpriteBatch spriteBatch, int x, int y, int scaleMultiplier)
    {
    }

    public void RenderAtPosition(
        SpriteBatch spriteBatch,
        Rectangle destRectangle,
        Rectangle panelButtonBackgroundSourceRectangle,
        int scaleMultiplier)
    {
        spriteBatch.Draw(_skillPanels, destRectangle, panelButtonBackgroundSourceRectangle, Color.White);
        spriteBatch.Draw(_skillCountErase, destRectangle, Color.White);

        var dx = 3 * scaleMultiplier;

        _skillCountDigitFont.RenderText(spriteBatch, _skillAssignButton.NumberOfSkillsAvailable.ToString(), destRectangle.X + dx, destRectangle.Y + scaleMultiplier, scaleMultiplier);

        if (_skillAssignButton.IsSelected)
        {
            spriteBatch.Draw(_skillSelected, destRectangle, Color.White);
        }
    }

    public override void RenderAtPosition(SpriteBatch spriteBatch, Rectangle sourceRectangle, int x, int y, int scaleMultiplier)
    {
    }
}