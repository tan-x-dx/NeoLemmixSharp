using Microsoft.Xna.Framework.Content;

namespace NeoLemmixSharp.Common.Rendering.Text;

public static class FontBank
{
    public static void Initialise(ContentManager contentManager)
    {
        if (MenuFont is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(FontBank)} more than once!");

        MenuFont = new MenuFont(contentManager);
        PanelFont = new PanelFont(contentManager);
        SkillCountDigitFont = new SkillCountDigitFont(contentManager);
        CountDownFont = new CountDownFont(contentManager);
    }

    public static MenuFont MenuFont { get; private set; } = null!;
    public static PanelFont PanelFont { get; private set; } = null!;
    public static SkillCountDigitFont SkillCountDigitFont { get; private set; } = null!;
    public static CountDownFont CountDownFont { get; private set; } = null!;
}