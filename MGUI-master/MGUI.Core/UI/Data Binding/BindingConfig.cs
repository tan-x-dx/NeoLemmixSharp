using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

#if UseWPF
using System.Windows.Markup;
using System.Windows.Data;
#else
using MGUI.Core.UI.Data_Binding.Converters;
#endif

namespace MGUI.Core.UI.Data_Binding;

public readonly record struct BindingConfig(string _targetPath, string _sourcePath, DataBindingMode BindingMode = DataBindingMode.OneWay,
    ISourceObjectResolver SourceResolver = null, DataContextResolver DataContextResolver = DataContextResolver.DataContext,
    IValueConverter Converter = null, object ConverterParameter = null, object FallbackValue = null, string StringFormat = null)
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _targetPath = _targetPath ?? throw new ArgumentNullException(nameof(_targetPath));
    public string TargetPath
    {
        get => _targetPath;
        init
        {
            _targetPath = value;
            TargetPaths = GetPaths(TargetPath);
        }
    }
    public readonly ReadOnlyCollection<string> TargetPaths = GetPaths(_targetPath);

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly string _sourcePath = _sourcePath ?? throw new ArgumentNullException(nameof(_sourcePath));
    public string SourcePath
    {
        get => _sourcePath;
        init
        {
            _sourcePath = value;
            SourcePaths = GetPaths(SourcePath);
        }
    }

    public readonly ReadOnlyCollection<string> SourcePaths = GetPaths(_sourcePath);

    private static ReadOnlyCollection<string> GetPaths(string path)
        => path == string.Empty ? new List<string>() { "" }.AsReadOnly() : path.Split('.', StringSplitOptions.RemoveEmptyEntries).ToList().AsReadOnly();

    public ISourceObjectResolver SourceResolver { get; init; } = SourceResolver ?? ISourceObjectResolver.FromSelf();
}