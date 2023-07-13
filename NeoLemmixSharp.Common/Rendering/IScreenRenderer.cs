using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Rendering;

public interface IScreenRenderer : IDisposable
{
    bool IsDisposed { get; protected set; }

    IGameWindow GameWindow { get; set; }

    void RenderScreen(SpriteBatch spriteBatch);
    void OnWindowSizeChanged(int windowWidth, int windowHeight);
}