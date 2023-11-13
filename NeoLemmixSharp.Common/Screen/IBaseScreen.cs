using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Screen;

public interface IBaseScreen : IDisposable
{
    IScreenRenderer ScreenRenderer { get; }
    bool IsDisposed { get; }

    string ScreenTitle { get; }

    void Tick(GameTime gameTime);
    void OnWindowSizeChanged();
}