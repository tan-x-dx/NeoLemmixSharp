using Microsoft.Xna.Framework;
using MLEM.Ui;
using MLEM.Ui.Elements;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class MainPage : PageBase
{
    private Panel _panel;
    private Button _playButton;
    private Button _levelSelectButton;

    private Button _groupButton;
    private Button _groupUpButton;
    private Button _groupDownButton;
    //  private MenuFontText _groupName;

    private Button _configButton;
    private Button _quitButton;

    public MainPage(MenuInputController inputController) : base(inputController)
    {
    }

    protected override void OnInitialise()
    {
        this._configButton = new Button(Anchor.AutoCenter, Vector2.One);

       // this.UiRoot.
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