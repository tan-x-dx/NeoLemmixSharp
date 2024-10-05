using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Ui;
using MLEM.Ui.Elements;
using NeoLemmixSharp.Common.Screen;

namespace NeoLemmixSharp.Common.Util;

public interface IGameWindow
{
    static IGameWindow Instance { get; set; } = null!;

    int WindowWidth { get; }
    int WindowHeight { get; }

    bool IsActive { get; }
    bool IsFullscreen { get; }

    GraphicsDevice GraphicsDevice { get; }
    ContentManager Content { get; }

    void SetScreen(IBaseScreen screen);
    void CaptureCursor();
    void ToggleFullscreen();
    void ToggleBorderless();
    void Escape();

    UiSystem UiSystem { get; }
    Element UiRoot { get; }
}