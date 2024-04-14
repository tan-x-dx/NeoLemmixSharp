using MGUI.Core.UI.Brushes.Fill_Brushes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

#if UseWPF
using System.Windows.Data;
#else
using MGUI.Core.UI.Data_Binding.Converters;
#endif

namespace MGUI.Core.UI.Data_Binding;

public enum DataContextResolver
{
    /// <summary>Indicates that data should be read from a 'DataContext' property on the targeted object.</summary>
    DataContext,
    /// <summary>Indicates that data should be read directly from the targeted object.</summary>
    Self
}

public enum DataBindingMode
{
    /// <summary>Indicates that the source object's property value should be read once while initializing the binding, and propagated to the target object's property value.<para/>
    /// The binding will not attempt to dynamically update the target object's property value if the source object's property value changes after initialization.</summary>
    OneTime,
    /// <summary>Indicates that the source object's property value should be propagated to the target object's property value.<para/>
    /// The binding will listen for changes to the source object's property value and attempt to dynamically update the target whenever a change is detected.<para/>
    /// This is the opposite binding direction of <see cref="OneWayToSource"/></summary>
    OneWay,
    /// <summary>Indicates that the target object's property value should be propagated to the source object's property value.<para/>
    /// The binding will listen for changes to the target object's property value and attempt to dynamically update the source whenever a change is detected.<para/>
    /// This is the opposite binding direction of <see cref="OneWay"/></summary>
    OneWayToSource,
    /// <summary>Indicates that the source object's property value should be kept in sync with the target object's property value.<para/>
    /// If either the source or the target value changes, the binding will dynamically update the other value immediately after.<para/>
    /// This mode effectively combines the functionality of <see cref="OneWay"/> and <see cref="OneWayToSource"/></summary>
    TwoWay
    //OneTimeToSource? The opposite of OneTime. Read Target object's property value during initialization, copy that value to the source object's property value.
}

public interface IObservableDataContext
{
    public const string DefaultDataContextPropertyName = "DataContext";
    public string DataContextPropertyName => DefaultDataContextPropertyName;
    event EventHandler<object> DataContextChanged;
}

/// <summary>To instantiate a binding, use <see cref="DataBindingManager.AddBinding"/></summary>
public sealed class DataBinding : IDisposable, ITypeDescriptorContext
{
    //"Weak event pattern" information to avoid memory leaks with the event listeners:
    //https://learn.microsoft.com/en-us/dotnet/desktop/wpf/events/weak-event-patterns?view=netdesktop-7.0&redirectedfrom=MSDN
    //https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.propertychangedeventmanager?redirectedfrom=MSDN&view=windowsdesktop-7.0

    public readonly BindingConfig Config;

    public readonly object TargetObject;
    public readonly string TargetPropertyName;
    public readonly PropertyInfo TargetProperty;
    public readonly Type TargetPropertyType;

    private readonly record struct ConverterConfig(IValueConverter Converter, object Parameter, bool IsConvertingBack)
    {
        public object Apply(object value, Type targetType) => IsConvertingBack ?
            Converter.ConvertBack(value, targetType, Parameter, CultureInfo.CurrentCulture) :
            Converter.Convert(value, targetType, Parameter, CultureInfo.CurrentCulture);
    }
    private readonly ConverterConfig? _convertSettings;
    private readonly ConverterConfig? _convertBackSettings;

    private object _sourceRoot;
    /// <summary>The root of the source object, as determined by <see cref="BindingConfig.SourceResolver"/> and <see cref="BindingConfig.DataContextResolver"/>.<para/>
    /// This might not be the same as <see cref="SourceObject"/> if the <see cref="BindingConfig.SourcePath"/> must recurse nested objects.<br/>
    /// For example, if the <see cref="BindingConfig.SourcePath"/> is "Foo.Bar", then the <see cref="SourceRoot"/> would be the object containing the Foo property, 
    /// and the <see cref="SourceObject"/> would be the Foo object itself. ("Bar" would be the Source Property's Name)</summary>
    public object SourceRoot
    {
        get => _sourceRoot;
        private set
        {
            if (_sourceRoot != value)
            {
                _sourceRoot = value;
                if (Config.SourcePaths.Count <= 1)
                    SourceObject = SourceRoot;
                else
                    FindAndTrackSourceObject();
            }
        }
    }

    private void FindAndTrackSourceObject()
    {
        ClearPathChangeListeners();
        SourceObject = ResolvePath(SourceRoot, Config.SourcePaths, true);

        //  Refresh the SourceObject when any object along the source path changes its value
        //  EX: SourcePath="Foo.Bar.Baz"
        //  SourceObject must be updated when the Foo property value of the SourceRoot changes, or when the Bar property value of the Foo object changes.
        if (SourceRoot == null || Config.SourcePaths.Count <= 1)
            return;

        IList<string> propertyNames = Config.SourcePaths;

        var current = SourceRoot;
        for (var i = 0; i < propertyNames.Count - 1; i++)
        {
            var propertyName = propertyNames[i];
            var property = GetPublicProperty(current, propertyName);
            if (property == null)
                break;

            if (current is INotifyPropertyChanged propChangedObject)
                UpdateSourceObjectWhenPropertyChanges(new(propChangedObject, propertyName));

            current = property.GetValue(current, null);
            if (current == null)
                break;
        }
    }

    private readonly record struct PropertyChangedSourceMetadata(INotifyPropertyChanged Object, string PropertyName);

#if UseWPF
        private readonly List<PropertyChangedSourceMetadata> PathChangeHandlers = new();
#else
    private readonly List<PropertyNameHandler> _pathChangeHandlers = new();
#endif

    private void UpdateSourceObjectWhenPropertyChanges(PropertyChangedSourceMetadata item)
    {
#if UseWPF
            PropertyChangedEventManager.AddHandler(Item.Object, UpdateSourceObject, Item.PropertyName);
            PathChangeHandlers.Add(Item);
#else
        //  Maybe use this: https://github.com/davidmilligan/WeakEventListener/blob/master/WeakEventListener/WeakEventManager.cs
        PropertyNameHandler handler = new(item.Object, item.PropertyName, UpdateSourceObject);
        _pathChangeHandlers.Add(handler);
#endif
    }
    private void ClearPathChangeListeners()
    {
        if (_pathChangeHandlers.Count == 0)
            return;

        try
        {
#if UseWPF
                    foreach (PropertyChangedSourceMetadata Item in PathChangeHandlers)
                        PropertyChangedEventManager.RemoveHandler(Item.Object, UpdateSourceObject, Item.PropertyName);
#else
            foreach (var handler in _pathChangeHandlers)
                handler.Detach();
#endif
        }
        finally { _pathChangeHandlers.Clear(); }
    }

    private void UpdateSourceObject(object sender, PropertyChangedEventArgs e) => FindAndTrackSourceObject();

    private object _sourceObject;
    public object SourceObject
    {
        get => _sourceObject;
        private set
        {
            if (_sourceObject == value)
                return;

            if (_isSubscribedToSourceObjectPropertyChanged && SourceObject is INotifyPropertyChanged previousObservableSourceObject)
            {
#if UseWPF
                        PropertyChangedEventManager.RemoveHandler(PreviousObservableSourceObject, ObservableSourceObject_PropertyChanged, SourcePropertyName);
#else
                previousObservableSourceObject.PropertyChanged -= ObservableSourceObject_PropertyChanged;
#endif
                _isSubscribedToSourceObjectPropertyChanged = false;
            }

            _sourceObject = value;
            SourceProperty = GetPublicProperty(SourceObject, SourcePropertyName);
            SourcePropertyValueChanged();

            //  Apply the FallbackValue if we couldn't find a valid property to bind to
            if (SourceProperty == null && Config.FallbackValue != null && !_isSettingValue &&
                Config.BindingMode is DataBindingMode.OneTime or DataBindingMode.OneWay or DataBindingMode.TwoWay)
            {
                TrySetPropertyValue(Config.FallbackValue, TargetObject, TargetProperty, TargetPropertyType, _convertSettings);
            }

            //  Update the target object's property value if the binding is directly on the source object (instead of a property on the source object)
            if (string.IsNullOrEmpty(SourcePropertyName) && Config.BindingMode is DataBindingMode.OneTime or DataBindingMode.OneWay)
            {
                TrySetPropertyValue(SourceObject, TargetObject, TargetProperty, TargetPropertyType, _convertSettings);
            }
            //  Apply OneTime bindings
            else if (Config.BindingMode is DataBindingMode.OneTime)
            {
                TrySetPropertyValue(SourceObject, SourceProperty, SourcePropertyType, TargetObject, TargetProperty, TargetPropertyType, _convertSettings);
            }

            //  Listen for changes to the source object's property value
            if (SourceProperty != null && Config.BindingMode is DataBindingMode.OneWay or DataBindingMode.TwoWay &&
                SourceObject is INotifyPropertyChanged observableSourceObject)
            {
#if UseWPF
                        PropertyChangedEventManager.AddHandler(ObservableSourceObject, ObservableSourceObject_PropertyChanged, SourcePropertyName);
#else
                observableSourceObject.PropertyChanged += ObservableSourceObject_PropertyChanged;
#endif
                _isSubscribedToSourceObjectPropertyChanged = true;
            }
        }
    }

    private PropertyInfo _sourceProperty;
    public PropertyInfo SourceProperty
    {
        get => _sourceProperty;
        private set
        {
            if (_sourceProperty != value)
            {
                _sourceProperty = value;
                SourcePropertyType = GetUnderlyingType(SourceProperty);
            }
        }
    }

    public Type SourcePropertyType { get; private set; }

    public readonly string SourcePropertyName;

    public static object ResolvePath(object root, IList<string> propertyNames, bool excludeLast = false)
    {
        var current = root;
        for (var i = 0; i < propertyNames.Count; i++)
        {
            if (current == null || (excludeLast && i == propertyNames.Count - 1))
                break;

            var propertyName = propertyNames[i];
            var property = GetPublicProperty(current, propertyName);
            current = property?.GetValue(current, null);
        }

        return current;
    }

    private static readonly Dictionary<object, Dictionary<string, PropertyInfo>> CachedProperties = new();
    private static PropertyInfo GetPublicProperty(object parent, string propertyName)
    {
        if (parent == null || string.IsNullOrEmpty(propertyName))
            return null;

        if (!CachedProperties.TryGetValue(parent, out var propertiesByName))
        {
            propertiesByName = new();
            CachedProperties.Add(parent, propertiesByName);
        }

        if (!propertiesByName.TryGetValue(propertyName, out var propInfo))
        {
            propInfo = parent.GetType().GetProperty(propertyName);
            propertiesByName.Add(propertyName, propInfo);
        }

        return propInfo;
    }

    /// <summary>Retrieve the given <paramref name="propInfo"/>'s <see cref="PropertyInfo.PropertyType"/>, 
    /// but prioritizes the underlying type if the type is wrapped in a Nullable&lt;T&gt;</summary>
    private static Type GetUnderlyingType(PropertyInfo propInfo) =>
        propInfo == null ? null : Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

    /// <param name="object">The object which the property paths (<see cref="BindingConfig.TargetPath"/>, <see cref="BindingConfig.SourcePath"/>) should be retrieved from.</param>
    internal DataBinding(BindingConfig config, object @object)
    {
        this.Config = config;
        _convertSettings = config.Converter == null ? null : new(config.Converter, config.ConverterParameter, false);
        _convertBackSettings = config.Converter == null ? null : new(config.Converter, config.ConverterParameter, true);

        this.TargetObject = ResolvePath(@object, config.TargetPaths, true);
        TargetPropertyName = config.TargetPaths[^1];
        TargetProperty = GetPublicProperty(TargetObject, TargetPropertyName);
        TargetPropertyType = GetUnderlyingType(TargetProperty);
        SourcePropertyName = config.SourcePaths.Count > 0 ? config.SourcePaths[^1] : null;

        //  The Source object is computed in 3 steps:
        //  1. Use the SourceObjectResolver to determine where to start
        //			If SourceObjectResolver=Self, we start from the Object constructor parameter
        //			If SourceObjectResolver=ElementName, we start from a particular named element in the Object's Window
        //			If SourceObjectResolver=Desktop, we start from the Object's Desktop
        //  2. Use the DataContextResolver to determine if we should be reading directly from
        //			the 1st step's result, or if we additionally have to read the value of a 'DataContext' property.
        //  3. Traverse the path given by SourcePaths
        //			EX: If SourcePaths="A.B.C" then we want to take the result of step #2, and look for a property named "A".
        //				Then from that property's value, look for a property named "B" and take it's value.
        //				Since we only want the parent property of the innermost source property, we would stop at object "B".

        var initialSourceRoot = config.SourceResolver.ResolveSourceObject(@object);
        switch (config.DataContextResolver)
        {
            case DataContextResolver.DataContext:
                //  Retrieve the value of the "DataContext" property
                if (initialSourceRoot is IObservableDataContext dataContextHost)
                {
                    var dataContextPropertyName = dataContextHost.DataContextPropertyName;
                    var dataContextProperty = GetPublicProperty(initialSourceRoot, dataContextPropertyName);
                    SourceRoot = dataContextProperty?.GetValue(initialSourceRoot);
                    dataContextHost.DataContextChanged += (sender, e) => { SourceRoot = dataContextProperty.GetValue(initialSourceRoot); };
                }
                else
                {
                    var dataContextPropertyName = IObservableDataContext.DefaultDataContextPropertyName;
                    var dataContextProperty = GetPublicProperty(initialSourceRoot, dataContextPropertyName);
                    SourceRoot = dataContextProperty?.GetValue(initialSourceRoot);
                }
                break;
            case DataContextResolver.Self:
                SourceRoot = initialSourceRoot;
                break;
            default: throw new NotImplementedException($"Unrecognized {nameof(DataContextResolver)}: {config.DataContextResolver}");
        }

        //  Listen for changes to the target object's property value
        if (TargetProperty != null && config.BindingMode is DataBindingMode.OneWayToSource or DataBindingMode.TwoWay &&
            TargetObject is INotifyPropertyChanged observableTargetObject)
        {
#if UseWPF
                PropertyChangedEventManager.AddHandler(ObservableTargetObject, ObservableTargetObject_PropertyChanged, TargetPropertyName);
#else
            observableTargetObject.PropertyChanged += ObservableTargetObject_PropertyChanged;
#endif
            _isSubscribedToTargetObjectPropertyChanged = true;
        }
        else
            _isSubscribedToTargetObjectPropertyChanged = false;
    }

    #region Set Property Value
    private bool _isSettingValue = false;

    private bool TrySetPropertyValue(object value, object targetObject, PropertyInfo targetProperty, Type targetPropertyType, ConverterConfig? converterSettings)
    {
        if (_isSettingValue)
            return false;

        try
        {
            _isSettingValue = true;
            return TrySetValue(this, value, targetObject, targetProperty, targetPropertyType, converterSettings, Config.StringFormat);
        }
        finally { _isSettingValue = false; }
    }

    private bool TrySetPropertyValue(object sourceObject, PropertyInfo sourceProperty, Type sourcePropertyType,
        object targetObject, PropertyInfo targetProperty, Type targetPropertyType, ConverterConfig? converterSettings)
    {
        if (_isSettingValue)
            return false;

        try
        {
            _isSettingValue = true;
            return TrySetValue(this, sourceObject, sourceProperty, sourcePropertyType,
                targetObject, targetProperty, targetPropertyType,
                converterSettings, Config.StringFormat);
        }
        finally { _isSettingValue = false; }
    }

    /// <summary>Attempts to copy the given <paramref name="value"/> into the <paramref name="targetObject"/>'s <paramref name="targetProperty"/>.</summary>
    /// <param name="targetPropertyType">If null, will be retrieved via <see cref="GetUnderlyingType(PropertyInfo)"/></param>
    private static bool TrySetValue(ITypeDescriptorContext context, object value, object targetObject, PropertyInfo targetProperty, Type targetPropertyType,
        ConverterConfig? converterSettings, string stringFormat)
    {
        if (targetObject == null || targetProperty == null)
            return false;

        var sourceType = value?.GetType();
        targetPropertyType ??= GetUnderlyingType(targetProperty);

        if (converterSettings.HasValue)
        {
            value = converterSettings.Value.Apply(value, targetPropertyType);
            sourceType = value?.GetType() ?? typeof(object);
        }

        if ((value == null && !targetPropertyType.IsValueType) ||
            (value != null && IsAssignableOrConvertible(sourceType, targetPropertyType)))
        {
            var actualValue = ConvertValue(context, sourceType, targetPropertyType, value, null, stringFormat);
            try { targetProperty.SetValue(targetObject, actualValue); }
            catch (Exception ex) { Debug.WriteLine(ex); }
            return true;
        }

        return false;
    }

    /// <summary>Attempts to copy the value of the <paramref name="sourceObject"/>'s <paramref name="sourceProperty"/> into the 
    /// <paramref name="targetObject"/>'s <paramref name="targetProperty"/>.</summary>
    /// <param name="sourcePropertyType">If null, will be retrieved via <see cref="GetUnderlyingType(PropertyInfo)"/></param>
    /// <param name="targetPropertyType">If null, will be retrieved via <see cref="GetUnderlyingType(PropertyInfo)"/></param>
    private static bool TrySetValue(ITypeDescriptorContext context, object sourceObject, PropertyInfo sourceProperty, Type sourcePropertyType,
        object targetObject, PropertyInfo targetProperty, Type targetPropertyType, ConverterConfig? converterSettings, string stringFormat)
    {
        if (sourceObject == null || sourceProperty == null || targetObject == null || targetProperty == null)
            return false;

        sourcePropertyType ??= GetUnderlyingType(sourceProperty);
        targetPropertyType ??= GetUnderlyingType(targetProperty);

        var value = sourceProperty.GetValue(sourceObject);
        if (converterSettings.HasValue)
        {
            value = converterSettings.Value.Apply(value, targetPropertyType);
            sourcePropertyType = value?.GetType() ?? typeof(object);
        }

        if (converterSettings.HasValue || IsAssignableOrConvertible(sourcePropertyType, targetPropertyType))
        {
            var actualValue = ConvertValue(context, sourcePropertyType, targetPropertyType, value, null, stringFormat);
            try { targetProperty.SetValue(targetObject, actualValue); }
            catch (Exception ex) { Debug.WriteLine(ex); }
            return true;
        }

        return false;
    }

    /// <param name="canAssign">If null, will be computed via <see cref="IsAssignable(Type, Type)"/></param>
    private static object ConvertValue(ITypeDescriptorContext context, Type sourceType, Type targetType, object value, bool? canAssign, string stringFormat)
    {
        if (value == null)
            return null;

        if (stringFormat != null && targetType == typeof(string))
        {
            try { return string.Format(stringFormat, value); }
            catch (FormatException) { return value; }
        }

        if (canAssign == true || (!canAssign.HasValue && IsAssignable(sourceType, targetType)))
            return value;

        if (TryConvertWithTypeConverter(context, sourceType, targetType, value, out var typeConvertedValue))
            return typeConvertedValue;

        if (value is IConvertible)
        {
            try { return Convert.ChangeType(value, targetType); }
            catch (FormatException) { return value; }
        }

        if (targetType == typeof(string))
            return value.ToString();

        throw new NotImplementedException($"Could not convert value from type='{sourceType.FullName}' to type='{targetType.FullName}'.");
    }

    private static bool TryConvertWithTypeConverter(ITypeDescriptorContext context, Type sourceType, Type targetType, object value, out object result)
    {
        return TryConvertFromWithTypeConverter(context, sourceType, targetType, value, out result) ||
               TryConvertToWithTypeConverter(context, sourceType, targetType, value, out result);
    }

    private static bool TryConvertFromWithTypeConverter(ITypeDescriptorContext context, Type sourceType, Type targetType, object value, out object result)
    {
        var converter = GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            try
            {
                result = context == null ?
                    converter.ConvertFrom(value) :
                    converter.ConvertFrom(context, CultureInfo.CurrentCulture, value);
                return true;
            }
            catch (ArgumentException) { }
        }

        result = null;
        return false;
    }

    private static bool TryConvertToWithTypeConverter(ITypeDescriptorContext context, Type sourceType, Type targetType, object value, out object result)
    {
        var converter = GetConverter(sourceType);
        if (converter.CanConvertTo(targetType))
        {
            result = context == null ?
                converter.ConvertTo(value, targetType) :
                converter.ConvertTo(context, CultureInfo.CurrentCulture, value, targetType);
            return true;
        }

        result = null;
        return false;
    }

    private static readonly Dictionary<Type, Dictionary<Type, bool>> CachedIsAssignable = new();
    private static bool IsAssignable(Type from, Type to)
    {
        if (from == null || to == null)
            return false;

        if (!CachedIsAssignable.TryGetValue(from, out var canAssignByType))
        {
            canAssignByType = new();
            CachedIsAssignable.Add(from, canAssignByType);
        }

        if (!canAssignByType.TryGetValue(to, out var canAssign))
        {
            canAssign = from.IsAssignableTo(to);
            canAssignByType.Add(to, canAssign);
        }

        return canAssign;
    }

    static DataBinding()
    {
        Dictionary<Type, Type> builtInTypeConverters = new()
        {
            { typeof(Microsoft.Xna.Framework.Color), typeof(XNAColorStringConverter) }
        };

        //  Apply some default TypeConverters such as being able to convert a string to a Microsoft.Xna.Framework.Color
        foreach (var kvp in builtInTypeConverters)
        {
            RegisterDefaultTypeConverter(kvp.Key, kvp.Value);
        }
    }

    /// <summary>Registers the given <see cref="TypeConverter"/> as the default converter to use when converting to the given <paramref name="targetType"/></summary>
    public static void RegisterDefaultTypeConverter(Type targetType, Type typeConverter)
        => RegisterDefaultTypeConverter(targetType, new TypeConverterAttribute(typeConverter));
    /// <summary>Registers the given <see cref="TypeConverter"/> as the default converter to use when converting to the given <paramref name="targetType"/></summary>
    public static void RegisterDefaultTypeConverter(Type targetType, TypeConverterAttribute typeConverter)
        => TypeDescriptor.AddAttributes(targetType, typeConverter);

    private static readonly Dictionary<Type, TypeConverter> CachedConverters = new();
    private static TypeConverter GetConverter(Type type)
    {
        if (type == null)
            return null;

        if (!CachedConverters.TryGetValue(type, out var converter))
        {
            converter = TypeDescriptor.GetConverter(type);
            CachedConverters.Add(type, converter);
        }

        return converter;
    }

    private static readonly Dictionary<TypeConverter, Dictionary<Type, bool>> CachedCanConvertFrom = new();
    private static readonly Dictionary<TypeConverter, Dictionary<Type, bool>> CachedCanConvertTo = new();
    private static bool IsConvertible(Type from, Type to) => IsConvertibleFrom(from, to) || IsConvertibleTo(from, to);

    //TODO TypeConverters don't necessarily return the same value for the same input types.
    //CanConvertFrom/CanConvertTo might return different values depending on the ITypeDescriptorContext
    //so we can't actually cache these results... and we should be passing in the context...
    //I don't care to fix this right now since it's pretty rare that a TypeConverter's implementation
    //needs to access the ITypeDescriptorContext to determine if it can/cannot convert to/from.
    private static bool IsConvertibleFrom(Type from, Type to)
    {
        if (from == null || to == null)
            return false;

        var converter = GetConverter(to);

        if (!CachedCanConvertFrom.TryGetValue(converter, out var canConvertByType))
        {
            canConvertByType = new();
            CachedCanConvertFrom.Add(converter, canConvertByType);
        }

        if (!canConvertByType.TryGetValue(to, out var canConvert))
        {
            canConvert = converter.CanConvertFrom(from);
            canConvertByType.Add(to, canConvert);
        }

        return canConvert;
    }

    private static bool IsConvertibleTo(Type from, Type to)
    {
        if (from == null || to == null)
            return false;

        var converter = GetConverter(from);

        if (!CachedCanConvertTo.TryGetValue(converter, out var canConvertByType))
        {
            canConvertByType = new();
            CachedCanConvertTo.Add(converter, canConvertByType);
        }

        if (!canConvertByType.TryGetValue(to, out var canConvert))
        {
            canConvert = converter.CanConvertTo(to);
            canConvertByType.Add(to, canConvert);
        }

        return canConvert;
    }

    private static bool IsAssignableOrConvertible(Type from, Type to) => IsAssignable(from, to) || IsConvertible(from, to);
    #endregion Set Property Value

    /// <summary>True if this binding subscribed to the <see cref="SourceObject"/>'s PropertyChanged event the last time <see cref="SourceObject"/> was set.<para/>
    /// If true, the event must be unsubscribed from when changing <see cref="SourceObject"/> or when disposing this <see cref="DataBinding"/></summary>
    private bool _isSubscribedToSourceObjectPropertyChanged = false;

    private void ObservableSourceObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == SourcePropertyName)
            SourcePropertyValueChanged();
#if UseWPF
            else
                throw new InvalidOperationException($"{nameof(DataBinding)}.{nameof(ObservableSourceObject_PropertyChanged)}: Expected PropertyName={SourcePropertyName}. Actual PropertyName={e.PropertyName}");
#endif
    }

    /// <summary>True if this binding subscribed to the <see cref="TargetObject"/>'s PropertyChanged event during initialization.<para/>
    /// If true, the event must be unsubscribed from when disposing this <see cref="DataBinding"/></summary>
    private readonly bool _isSubscribedToTargetObjectPropertyChanged;

    private void ObservableTargetObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == TargetPropertyName)
            TargetPropertyValueChanged();
#if UseWPF
            else
                throw new InvalidOperationException($"{nameof(DataBinding)}.{nameof(ObservableTargetObject_PropertyChanged)}: Expected PropertyName={TargetPropertyName}. Actual PropertyName={e.PropertyName}");
#endif
    }

    private void SourcePropertyValueChanged()
    {
        //  Propagate the new value to the TargetProperty
        if (!_isSettingValue && Config.BindingMode is DataBindingMode.OneWay or DataBindingMode.TwoWay)
        {
            TrySetPropertyValue(SourceObject, SourceProperty, SourcePropertyType, TargetObject, TargetProperty, TargetPropertyType, _convertSettings);
        }
    }

    private void TargetPropertyValueChanged()
    {
        //  Propagate the new value to the SourceProperty
        if (!_isSettingValue && Config.BindingMode is DataBindingMode.OneWayToSource or DataBindingMode.TwoWay)
        {
            TrySetPropertyValue(TargetObject, TargetProperty, TargetPropertyType, SourceObject, SourceProperty, SourcePropertyType, _convertBackSettings);
        }
    }

    public bool IsDisposed { get; private set; }
    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;

            //  Unsubscribe from PropertyChanged events
            if (_isSubscribedToTargetObjectPropertyChanged && TargetObject is INotifyPropertyChanged observableTargetObject)
            {
#if UseWPF
                PropertyChangedEventManager.RemoveHandler(ObservableTargetObject, ObservableTargetObject_PropertyChanged, TargetPropertyName);
#else
                observableTargetObject.PropertyChanged -= ObservableTargetObject_PropertyChanged;
#endif
            }
            if (_isSubscribedToSourceObjectPropertyChanged && SourceObject is INotifyPropertyChanged observableSourceObject)
            {
#if UseWPF
                PropertyChangedEventManager.RemoveHandler(ObservableSourceObject, ObservableSourceObject_PropertyChanged, SourcePropertyName);
#else
                observableSourceObject.PropertyChanged -= ObservableSourceObject_PropertyChanged;
#endif
                _isSubscribedToSourceObjectPropertyChanged = false;
            }
            ClearPathChangeListeners();
        }
    }

    public object Instance => TargetObject;

    public PropertyDescriptor PropertyDescriptor => null;

    public IContainer Container => null;
    public object GetService(Type serviceType) => null;
    public void OnComponentChanged() { }
    public bool OnComponentChanging() => true;
}