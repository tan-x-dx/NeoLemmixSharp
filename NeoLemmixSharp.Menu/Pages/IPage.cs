using GeonBit.UI;

namespace NeoLemmixSharp.Menu.Pages;

public interface IPage : IDisposable
{
    void Initialise();

    UserInterface UserInterface { get; }

    void SetWindowDimensions(int windowWidth, int windowHeight);

    void Tick();
}