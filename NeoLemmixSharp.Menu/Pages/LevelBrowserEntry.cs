using MGUI.Shared.Helpers;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelBrowserEntry : ViewModelBase
{
    private int _thingWidth;
    private string _thingName;

    public int ThingWidth
    {
        get => _thingWidth;
        set => this.RaisePropertyChanged(ref _thingWidth, value);
    }

    public string ThingName
    {
        get => _thingName;
        set => this.RaisePropertyChanged(ref _thingName, value);
    }
}