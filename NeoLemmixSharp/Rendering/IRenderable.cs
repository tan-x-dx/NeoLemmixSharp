using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering;

public interface IRenderable
{
    bool ShouldRender { get; }

    void Render(SpriteBatch spriteBatch);
}