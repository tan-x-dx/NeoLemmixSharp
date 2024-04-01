using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MGUI.Shared.Helpers;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void NotifyPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    /// <summary>Notify Property Changed for the given <paramref name="propertyName"/></summary>
    public void Npc(string propertyName) => NotifyPropertyChanged(propertyName);
    /// <summary>Parameter <paramref name="propertyName"/> is optional. If not specified, <see cref="CallerMemberNameAttribute"/> is automatically applied by the compiler. (Do not pass in null)</summary>
    /// <param name="propertyName"></param>
    public void AutoNpc([CallerMemberName] string propertyName = null) => NotifyPropertyChanged(propertyName);
}

public static class ViewModelHelpers
{
    public static void RaisePropertyChanged<T>(this ViewModelBase viewModel, ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value))
            return;

        field = value;
        viewModel.NotifyPropertyChanged(propertyName);
    }
}