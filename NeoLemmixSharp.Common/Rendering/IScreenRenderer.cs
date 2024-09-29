using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering;

public interface IScreenRenderer : IDisposable
{
    bool IsDisposed { get; }

    void RenderScreen(GameTime gameTime, SpriteBatch spriteBatch);
    void OnWindowSizeChanged();
}