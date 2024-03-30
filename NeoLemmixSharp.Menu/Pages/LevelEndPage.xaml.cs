using MGUI.Core.UI;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelEndPage : PageBase
{
    public LevelEndPage(
        MGDesktop desktop,
        MenuInputController inputController)
        : base(desktop, inputController)
    {
    }

    protected override void OnInitialise(MGDesktop desktop)
    {
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
    }

    public override void Tick()
    {
        HandleKeyboardInput();
        HandleMouseInput();
    }

    private void HandleKeyboardInput()
    {
    }

    private void HandleMouseInput()
    {
    }

    protected override void OnDispose()
    {
    }
}