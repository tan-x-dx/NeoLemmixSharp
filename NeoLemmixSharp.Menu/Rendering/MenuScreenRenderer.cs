using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Menu.Rendering;

public sealed class MenuScreenRenderer : IScreenRenderer
{
    public bool IsDisposed { get; private set; }
    public IGameWindow GameWindow { get; set; }

    public void RenderScreen(SpriteBatch spriteBatch)
    {
    }

    public void OnWindowSizeChanged(int windowWidth, int windowHeight)
    {
    }

    public void Dispose()
    {
    }
}