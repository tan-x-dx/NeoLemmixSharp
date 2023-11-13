using Microsoft.Xna.Framework.Content;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class FontBank
{
    public static FontBank Instance { get; private set; } = null!;

    public MenuFont MenuFont { get; }
    public PanelFont PanelFont { get; }
    public SkillCountDigitFont SkillCountDigitFont { get; }

    public FontBank(ContentManager contentManager)
    {
        MenuFont = new MenuFont(contentManager);
        PanelFont = new PanelFont(contentManager);
        SkillCountDigitFont = new SkillCountDigitFont(contentManager);

        Instance = this;
    }
}