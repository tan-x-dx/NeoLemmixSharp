using Myra.Graphics2D.UI;

namespace NeoLemmixSharp.Menu.Pages;

public interface IPage : IDisposable
{
    Widget RootWidget { get; }
}