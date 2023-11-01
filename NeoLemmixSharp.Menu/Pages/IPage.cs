using Myra.Graphics2D.UI;
using NeoLemmixSharp.Menu.Rendering;
using NeoLemmixSharp.Menu.Rendering.Pages;

namespace NeoLemmixSharp.Menu.Pages;

public interface IPage : IDisposable
{
    IPageRenderer GetPageRenderer(MenuSpriteBank menuSpriteBank, Desktop desktop);

    void Tick();
}