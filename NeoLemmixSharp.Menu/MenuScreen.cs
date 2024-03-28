using MGUI.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Common.Screen;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Menu.Pages;
using NeoLemmixSharp.Menu.Rendering;

namespace NeoLemmixSharp.Menu;

public sealed class MenuScreen : IBaseScreen
{
    public static MenuScreen Current { get; private set; } = null!;

    private readonly PageTransition _pageTransition = new(EngineConstants.PageTransitionDurationInFrames);
    private readonly MGDesktop _desktop;

    private IPage _currentPage;
    private IPage? _nextPage;

    public MenuInputController InputController { get; } = new();
    public MenuPageCreator MenuPageCreator { get; }
    public MenuScreenRenderer MenuScreenRenderer { get; }

    IScreenRenderer IBaseScreen.ScreenRenderer => MenuScreenRenderer;
    public string ScreenTitle => "NeoLemmixSharp";
    public bool IsDisposed { get; private set; }

    public MenuScreen(
        ContentManager contentManager,
        GraphicsDevice graphicsDevice)
    {
        _desktop = new MGDesktop(IGameWindow.Instance.MguiRenderer);

        var menuCursorRenderer = new MenuCursorRenderer(InputController);
        MenuScreenRenderer = new MenuScreenRenderer(
            menuCursorRenderer,
            _pageTransition,
            _desktop);

        MenuPageCreator = new MenuPageCreator(
            contentManager,
            graphicsDevice,
            InputController);
        _currentPage = MenuPageCreator.CreateMainPage();
        Current = this;
    }

    public void Initialise()
    {
        MenuScreenRenderer.Initialise();

        /* var window1 = new MGWindow(_desktop, 50, 50, 500, 200)
         {
             TitleText = "Sample Window with a single [b]Button[/b]: [color=yellow]Click it![/color]",
             BackgroundBrush =
             {
                 NormalValue = new MGSolidFillBrush(Color.Orange)
             },
             Padding = new Thickness(15)
         };
         var button1 = new MGButton(window1, button => { button.SetContent("I've been clicked!"); });
         button1.SetContent("Click me!");
         window1.SetContent(button1);*/

        var c = new TestWindow(IGameWindow.Instance.Content, _desktop);

        c.Show();


        //var userInterface = UserInterface.Active;
        //userInterface.Clear();
        //_currentPage.Initialise(userInterface.Root);
    }

    public void SetNextPage(IPage page)
    {
        _nextPage = page;
        _pageTransition.BeginTransition();
        InputController.ClearAllInputActions();
    }

    public void Tick(GameTime gameTime)
    {
        if (_pageTransition.IsTransitioning)
        {
            HandlePageTransition();

            return;
        }

        _desktop.Update();

        //UserInterface.Active.Update(gameTime);
        InputController.Tick();

        _currentPage.Tick();

        if (InputController.ToggleFullScreen.IsPressed)
        {
            IGameWindow.Instance.ToggleBorderless();
        }
    }

    private void HandlePageTransition()
    {
        _pageTransition.Tick();

        if (!_pageTransition.IsHalfWayDone)
            return;

        DisposableHelperMethods.DisposeOf(ref _currentPage);
        _currentPage = _nextPage!;

        //var userInterface = UserInterface.Active;
        //userInterface.Clear();
        //_currentPage.Initialise(userInterface.Root);

        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _currentPage.SetWindowDimensions(windowWidth, windowHeight);
    }

    public void OnWindowSizeChanged()
    {
        var windowWidth = IGameWindow.Instance.WindowWidth;
        var windowHeight = IGameWindow.Instance.WindowHeight;

        _currentPage.SetWindowDimensions(windowWidth, windowHeight);
        MenuScreenRenderer.OnWindowSizeChanged();
    }

    public void OnActivated()
    {
    }

    public void OnSetScreen()
    {
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        MenuScreenRenderer.Dispose();

        Current = null!;
        IsDisposed = true;
    }
}