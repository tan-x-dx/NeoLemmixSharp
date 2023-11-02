using Myra.Graphics2D.UI;

namespace NeoLemmixSharp.Menu.Pages;

public interface IPage : IDisposable
{
    void Initialise();

    Widget GetRootWidget();

    void SetWindowDimensions(int windowWidth, int windowHeight);

    void Tick();
}