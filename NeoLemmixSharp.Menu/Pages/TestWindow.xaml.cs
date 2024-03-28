using MGUI.Core.UI;
using MGUI.Core.UI.XAML;
using MGUI.Shared.Helpers;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using System.Reflection;

namespace NeoLemmixSharp.Menu.Pages;

public abstract class SampleBase : ViewModelBase
{
    public ContentManager Content { get; }
    public MGDesktop Desktop { get; }
    public MGResources Resources { get; }
    public MGWindow Window { get; }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                Npc(nameof(IsVisible));

                if (IsVisible)
                    Desktop.Windows.Add(Window);
                else
                    Desktop.Windows.Remove(Window);
                VisibilityChanged?.Invoke(this, IsVisible);
            }
        }
    }

    public event EventHandler<bool> VisibilityChanged;

    public void Show() => IsVisible = true;
    public void Hide() => IsVisible = false;

    /// <param name="initialize">Optional. This delegate is invoked before the XAML content is parsed, 
    /// so you may wish to use this delegate to add resources to <see cref="MGDesktop.Resources"/> that may be required in order to parse the XAML.</param>
    protected SampleBase(ContentManager content, MGDesktop desktop, string xamlFilename, Action initialize = null)
    {
        Content = content;
        Desktop = desktop;
        Resources = desktop.Resources;
        var resourceName = $"{nameof(NeoLemmixSharp)}.{nameof(Menu)}.{nameof(Pages)}.{xamlFilename}";
        var xaml = GeneralUtils.ReadEmbeddedResourceAsString(Assembly.GetExecutingAssembly(), resourceName);
        initialize?.Invoke();
        Window = XAMLParser.LoadRootWindow(desktop, xaml, false, true);
        Window.WindowClosed += (sender, e) => IsVisible = false;
    }

    protected static void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo()
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}

public class TestWindow : SampleBase
{
    public TestWindow(ContentManager content, MGDesktop desktop)
        : base(content, desktop, $"{nameof(TestWindow)}.xaml")
    {
        Window.IsCloseButtonVisible = false;

#if DEBUG
        //HUD.Show();
#endif

        Window.WindowDataContext = this;
    }
}