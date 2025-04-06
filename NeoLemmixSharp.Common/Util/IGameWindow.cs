using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Screen;

namespace NeoLemmixSharp.Common.Util;

public interface IGameWindow
{
    static IGameWindow Instance { get; set; } = null!;

    LevelSize WindowSize { get; }

    bool IsActive { get; }
    bool IsFullscreen { get; }
    bool IsBorderless { get; }

    GraphicsDevice GraphicsDevice { get; }
    ContentManager Content { get; }
    SpriteBatch SpriteBatch { get; }

    void SetScreen(IBaseScreen screen);
    void CaptureCursor();
    void ToggleFullscreen();
    void ToggleBorderless();
    void Escape();
}