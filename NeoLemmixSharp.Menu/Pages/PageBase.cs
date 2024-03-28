﻿using MGUI.Core.UI;
using MGUI.Core.UI.XAML;
using MGUI.Shared.Helpers;
using System.Reflection;
using Thickness = MonoGame.Extended.Thickness;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class PageBase : ViewModelBase, IDisposable
{
    private readonly MGDesktop _desktop;
    protected readonly MGWindow Window;

    private bool _isInitialised;

    protected MGResources Resources => _desktop.Resources;

    protected PageBase(MGDesktop desktop)
    {
        _desktop = desktop;
        var resourceName = $"{nameof(NeoLemmixSharp)}.{nameof(Menu)}.{nameof(Pages)}.{GetType().Name}.xaml";
        var xaml = GeneralUtils.ReadEmbeddedResourceAsString(Assembly.GetExecutingAssembly(), resourceName);

        Window = XamlParser.LoadRootWindow(desktop, xaml);
        Window.IsCloseButtonVisible = false;
        Window.WindowStyle = WindowStyle.None;
        Window.CanCloseWindow = false;
        Window.Padding = new Thickness(0);
        Window.IsUserResizable = false;
        Window.WindowDataContext = this;
    }

    protected void Show()
    {
        _desktop.Windows.Add(Window);
    }

    public void Initialise(MGDesktop desktop)
    {
        if (_isInitialised)
            return;

        OnInitialise(desktop);
        _isInitialised = true;
    }

    protected abstract void OnInitialise(MGDesktop desktop);

    public void SetWindowDimensions(int windowWidth, int windowHeight)
    {
        Window.WindowWidth = windowWidth;
        Window.WindowHeight = windowHeight;

        OnWindowDimensionsChanged(windowWidth, windowHeight);
    }

    protected abstract void OnWindowDimensionsChanged(int windowWidth, int windowHeight);

    public abstract void Tick();
    public abstract void Dispose();
}