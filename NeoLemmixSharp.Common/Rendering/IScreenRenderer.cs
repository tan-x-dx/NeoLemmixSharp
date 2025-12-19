using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering;

public interface IScreenRenderer : IDisposable
{
    void RenderScreen(GameTime gameTime, SpriteBatch spriteBatch);
    void OnWindowSizeChanged();
}