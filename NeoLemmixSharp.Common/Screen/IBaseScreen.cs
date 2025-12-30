using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Common.Screen;

public interface IBaseScreen : IInitialisable, IDisposable
{
    IScreenRenderer ScreenRenderer { get; }

    string ScreenTitle { get; }

    void Tick(GameTime gameTime);
    void OnWindowSizeChanged();
    void OnActivated();
}