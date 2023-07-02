using Microsoft.Xna.Framework.Content;

namespace NeoLemmixSharp.Common.Rendering.Text;

public sealed class FontBank
{
    public INeoLemmixFont MenuFont { get; }
    public INeoLemmixFont SkillCountDigitFont { get; }

    public FontBank(ContentManager contentManager)
    {
        MenuFont = new MenuFont(contentManager);
        SkillCountDigitFont = new SkillCountDigitFont(contentManager);
    }
}