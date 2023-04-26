using Microsoft.Xna.Framework.Content;

namespace NeoLemmixSharp.Rendering.Text;

public sealed class FontBank
{
    public INeoLemmixFont MenuFont { get; }

    public FontBank(ContentManager contentManager)
    {
        MenuFont = new MenuFont(contentManager);
    }
}