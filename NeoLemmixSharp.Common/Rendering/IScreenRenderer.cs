using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering;

public interface IScreenRenderer : IDisposable
{
    bool IsDisposed { get; }

    void RenderScreen(SpriteBatch spriteBatch);
    void OnWindowSizeChanged();
}