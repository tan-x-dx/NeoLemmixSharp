using GeonBit.UI;
using GeonBit.UI.Entities;

namespace NeoLemmixSharp.Menu.Pages;

public interface IPage : IDisposable
{
    void Initialise(RootPanel rootPanel);

    void SetWindowDimensions(int windowWidth, int windowHeight);

    void Tick();
}