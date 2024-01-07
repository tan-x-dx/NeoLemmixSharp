using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering;

public interface IScreenRenderer : IDisposable
{
    bool IsDisposed { get; }

    void RenderScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
    void OnWindowSizeChanged();
}