using MGUI.Core.UI.Data_Binding;
using MGUI.Core.UI.Data_Binding.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using XNAColor = Microsoft.Xna.Framework.Color;

namespace MGUI.Core.UI.XAML;

[TypeConverter(typeof(ElementStringConverter))]
public abstract class Element
{
    public abstract MGElementType ElementType { get; }

    public string Name { get; set; }

    [Category("Layout")]
    public Thickness? Margin { get; set; }
    [Category("Layout")]
    public Thickness? Padding { get; set; }

    [Category("Layout")] public HorizontalAlignment? HorizontalAlignment { get; set; }
    [Category("Layout")] public VerticalAlignment? VerticalAlignment { get; set; }
    [Category("Layout")] public HorizontalAlignment? HorizontalContentAlignment { get; set; }
    [Category("Layout")] public VerticalAlignment? VerticalContentAlignment { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public HorizontalAlignment? Ha { get => HorizontalAlignment; set => HorizontalAlignment = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public VerticalAlignment? Va { get => VerticalAlignment; set => VerticalAlignment = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public HorizontalAlignment? Hca { get => HorizontalContentAlignment; set => HorizontalContentAlignment = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public VerticalAlignment? Vca { get => VerticalContentAlignment; set => VerticalContentAlignment = value; }

    [Category("Layout")]
    public int? MinWidth { get; set; }
    [Category("Layout")]
    public int? MinHeight { get; set; }
    [Category("Layout")]
    public int? MaxWidth { get; set; }
    [Category("Layout")]
    public int? MaxHeight { get; set; }

    [Browsable(false)]
    public int? PreferredWidth { get; set; }
    [Browsable(false)]
    public int? PreferredHeight { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Layout")]
    public int? Width { get => PreferredWidth; set => PreferredWidth = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Layout")]
    public int? Height { get => PreferredHeight; set => PreferredHeight = value; }

    public ToolTip ToolTip { get; set; }
    public ContextMenu ContextMenu { get; set; }

    [Category("Behavior")]
    public bool? CanHandleInputsWhileHidden { get; set; }
    [Category("Behavior")]
    public bool? IsHitTestVisible { get; set; }

    [Category("Behavior")]
    public bool? IsSelected { get; set; }
    [Category("Behavior")]
    public bool? IsEnabled { get; set; }

    [Category("Appearance")]
    public Thickness? BackgroundRenderPadding { get; set; }
    [Category("Appearance")]
    public FillBrush Background { get; set; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Browsable(false)]
    public FillBrush Bg { get => Background; set => Background = value; }
    [Category("Appearance")]
    public FillBrush DisabledBackground { get; set; }
    [Category("Appearance")]
    public FillBrush SelectedBackground { get; set; }
    [Category("Appearance")]
    public XAMLColor? BackgroundFocusedColor { get; set; }

    [Category("Appearance")]
    public XAMLColor? TextForeground { get; set; }
    [Category("Appearance")]
    public XAMLColor? DisabledTextForeground { get; set; }
    [Category("Appearance")]
    public XAMLColor? SelectedTextForeground { get; set; }

    [Category("Appearance")]
    public Visibility? Visibility { get; set; }

    [Category("Layout")]
    public bool? ClipToBounds { get; set; }

    [Category("Appearance")]
    public float? Opacity { get; set; }

    [Category("Appearance")]
    public float? RenderScale { get; set; }

    /// <summary>Used by <see cref="DockPanel"/>'s children</summary>
    [Category("Attached")]
    public Dock Dock { get; set; } = Dock.Top;

    /// <summary>Used by <see cref="Grid"/>'s children</summary>
    [Browsable(false)]
    public int GridRow { get; set; } = 0;
    /// <summary>Used by <see cref="Grid"/>'s children</summary>
    [Browsable(false)]
    public int GridColumn { get; set; } = 0;
    /// <summary>Used by <see cref="Grid"/>'s children</summary>
    [Browsable(false)]
    public int GridRowSpan { get; set; } = 1;
    /// <summary>Used by <see cref="Grid"/>'s children</summary>
    [Browsable(false)]
    public int GridColumnSpan { get; set; } = 1;
    /// <summary>Used by <see cref="Grid"/>'s children</summary>
    [Category("Attached")]
    public bool GridAffectsMeasure { get; set; } = true;

    /// <summary>Used by <see cref="OverlayPanel"/>'s children</summary>
    [Category("Attached")]
    public Thickness Offset { get; set; } = new();
    /// <summary>Used by <see cref="OverlayPanel"/>'s children and by <see cref="Overlay"/>s.</summary>
    [Category("Attached")]
    public double? ZIndex { get; set; } = null;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Attached")]
    public int Row { get => GridRow; set => GridRow = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Attached")]
    public int Column { get => GridColumn; set => GridColumn = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Attached")]
    public int RowSpan { get => GridRowSpan; set => GridRowSpan = value; }
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [Category("Attached")]
    public int ColumnSpan { get => GridColumnSpan; set => GridColumnSpan = value; }

    /// <summary>If true, this object can have <see cref="Setter"/>s applied to its properties.<para/>
    /// Default value: true</summary>
    [Category("Appearance")]
    public bool IsStyleable { get; set; } = true;
    [Category("Appearance")]
    public List<Style> Styles { get; set; } = new();
    /// <summary>The names of the named <see cref="Style"/>s that should be applied to this <see cref="Element"/>.<br/>
    /// Use a comma to delimit multiple names, such as: "Style1,Style2<br/>
    /// to apply <see cref="Style"/> with <see cref="Style.Name"/>="Style1" and <see cref="Style"/> with <see cref="Style.Name"/>="Style2" to this <see cref="Element"/><para/>
    /// See also: <see cref="Style.Name"/></summary>
    [Category("Appearance")]
    public string StyleNames { get; set; }

    [Category("Attached")]
    public Dictionary<string, object> AttachedProperties { get; set; } = new();

    [Category("Attached")]
    public object Tag { get; set; }

    protected internal List<BindingConfig> Bindings { get; } = new();

    /// <param name="applyBaseSettings">If not null, this action will be invoked before <see cref="ApplySettings(MGElement, MGElement, bool)"/> executes.</param>
    public T ToElement<T>(MgWindow window, MGElement parent, Action<T> applyBaseSettings = null)
        where T : MGElement
    {
        T element = CreateElementInstance(window, parent) as T;
        applyBaseSettings?.Invoke(element);
        ApplySettings(parent, element, true);
        return element;
    }

    /// <param name="includeContent">Recommended value: true. If true, the child XAML content, if any, will also be processed.</param>
    protected internal void ApplySettings(MGElement parent, MGElement element, bool includeContent)
    {
        using (element.BeginInitializing())
        {
            ApplyBaseSettings(parent, element, true);
            ApplyDerivedSettings(parent, element, includeContent);
        }
    }

    internal void ApplyBaseSettings(MGElement parent, MGElement element, bool includeBindings)
    {
        using (element.BeginInitializing())
        {
            if (Name != null)
                element.Name = Name;

            if (Margin.HasValue)
                element.Margin = Margin.Value.ToThickness();
            if (Padding.HasValue)
                element.Padding = Padding.Value.ToThickness();

            if (HorizontalAlignment.HasValue)
                element.HorizontalAlignment = HorizontalAlignment.Value;
            if (VerticalAlignment.HasValue)
                element.VerticalAlignment = VerticalAlignment.Value;
            if (HorizontalContentAlignment.HasValue)
                element.HorizontalContentAlignment = HorizontalContentAlignment.Value;
            if (VerticalContentAlignment.HasValue)
                element.VerticalContentAlignment = VerticalContentAlignment.Value;

            if (MinWidth.HasValue)
                element.MinWidth = MinWidth.Value;
            if (MinHeight.HasValue)
                element.MinHeight = MinHeight.Value;
            if (MaxWidth.HasValue)
                element.MaxWidth = MaxWidth.Value;
            if (MaxHeight.HasValue)
                element.MaxHeight = MaxHeight.Value;

            if (PreferredWidth.HasValue)
                element.PreferredWidth = PreferredWidth.Value;
            if (PreferredHeight.HasValue)
                element.PreferredHeight = PreferredHeight.Value;

            if (ToolTip != null)
                element.ToolTip = ToolTip.ToElement<MGToolTip>(element.SelfOrParentWindow, element);
            if (ContextMenu != null)
                element.ContextMenu = ContextMenu.ToElement<MGContextMenu>(element.SelfOrParentWindow, element);

            if (CanHandleInputsWhileHidden.HasValue)
                element.CanHandleInputsWhileHidden = CanHandleInputsWhileHidden.Value;
            if (IsHitTestVisible.HasValue)
                element.IsHitTestVisible = IsHitTestVisible.Value;
            if (IsSelected.HasValue)
                element.IsSelected = IsSelected.Value;
            if (IsEnabled.HasValue)
                element.IsEnabled = IsEnabled.Value;

            if (BackgroundRenderPadding.HasValue)
                element.BackgroundRenderPadding = BackgroundRenderPadding.Value.ToThickness();
            ApplyBackground(element);

            if (TextForeground.HasValue)
                element.DefaultTextForeground.NormalValue = TextForeground.Value.ToXNAColor();
            if (DisabledTextForeground.HasValue)
                element.DefaultTextForeground.DisabledValue = DisabledTextForeground.Value.ToXNAColor();
            if (SelectedTextForeground.HasValue)
                element.DefaultTextForeground.SelectedValue = SelectedTextForeground.Value.ToXNAColor();

            if (Visibility.HasValue)
                element.Visibility = Visibility.Value;

            if (ClipToBounds.HasValue)
                element.ClipToBounds = ClipToBounds.Value;

            if (Opacity.HasValue)
                element.Opacity = Opacity.Value;

            if (RenderScale.HasValue)
                element.RenderScale = new(RenderScale.Value, RenderScale.Value);

            element.Tag = this.Tag;

            if (includeBindings)
            {
                //  These bindings are initialized in Window.ToElement(Desktop, Theme)
                //  Because ElementName references cannot be resolved until named elements have been processed and added to the MGWindow instance
                if (Bindings.Count > 0)
                    element.Metadata[BindingsMetadataKey] = this.Bindings;
            }
        }
    }

    private const string BindingsMetadataKey = "TmpBindings";

    //  DataBindings are defined in XAML (so they are applied to the properties of the XAML types)
    //  but are bound to the properties of the actual type (such as MGUI.Core.UI.MGButton instead of MGUI.Core.UI.XAML.Button).
    //  This dictionary is intended to handle cases where a property on the XAML type isn't the same name/path as the actual property of the binding
    private static readonly Dictionary<string, string> BindingPathMappings = new()
    {
        { nameof(Background), $"{nameof(MGElement.BackgroundBrush)}.{nameof(VisualStateFillBrush.NormalValue)}" },
        { nameof(SelectedBackground), $"{nameof(MGElement.BackgroundBrush)}.{nameof(VisualStateFillBrush.SelectedValue)}" },
        { nameof(DisabledBackground), $"{nameof(MGElement.BackgroundBrush)}.{nameof(VisualStateFillBrush.DisabledValue)}" },
        { nameof(TextForeground), $"{nameof(MGElement.DefaultTextForeground)}.{nameof(VisualStateSetting<XNAColor>.NormalValue)}" },
        { nameof(SelectedTextForeground), $"{nameof(MGElement.DefaultTextForeground)}.{nameof(VisualStateSetting<XNAColor>.SelectedValue)}" },
        { nameof(DisabledTextForeground), $"{nameof(MGElement.DefaultTextForeground)}.{nameof(VisualStateSetting<XNAColor>.DisabledValue)}" },
        { nameof(Width), $"{nameof(MGElement.PreferredWidth)}" },
        { nameof(Height), $"{nameof(MGElement.PreferredHeight)}" }
    };

    /// <summary>Resolves any pending <see cref="BindingConfig"/>s by converting them into <see cref="DataBinding"/>s</summary>
    /// <param name="dataContextOverride">If not null, this value will be applied to the <see cref="MGElement.DataContextOverride"/> value of every element that is processed.<para/>
    /// If <paramref name="recurseChildren"/> is false, this is only applied to <paramref name="element"/>. Else it's applied to <paramref name="element"/> and all its nested children.</param>
    internal static void ProcessBindings(MGElement element, bool recurseChildren, object dataContextOverride)
    {
        if (recurseChildren)
        {
            foreach (MGElement child in element.TraverseVisualTree(true, true, true, true, MGElement.TreeTraversalMode.Preorder))
            {
                ProcessBindings(child, false, dataContextOverride);
            }

            return;
        }

        if (dataContextOverride != null)
            element.DataContextOverride = dataContextOverride;

        if (element.Metadata.TryGetValue(BindingsMetadataKey, out object value) && value is List<BindingConfig> bindings)
        {
            foreach (BindingConfig binding in bindings)
            {
                object targetObject = element;
                BindingConfig postProcessedBinding = binding;

                //  Handle some special-cases where the name of the XAML property isn't the same as the corresponding property on the c# object
                if (BindingPathMappings.TryGetValue(binding.TargetPath, out string actualPath))
                {
                    postProcessedBinding = binding with { TargetPath = actualPath };
                }

                if (postProcessedBinding.Converter is StringToToolTipConverter stringToolTipConverter)
                    stringToolTipConverter.Host = element;

                DataBindingManager.AddBinding(postProcessedBinding, targetObject);
            }
            element.Metadata.Remove(BindingsMetadataKey);
        }
    }

    protected void ApplyBackground(MGElement element)
    {
        MGDesktop desktop = element.GetDesktop();

        if (Background != null)
            element.BackgroundBrush.NormalValue = Background.ToFillBrush(desktop);
        if (DisabledBackground != null)
            element.BackgroundBrush.DisabledValue = DisabledBackground.ToFillBrush(desktop);
        if (SelectedBackground != null)
            element.BackgroundBrush.SelectedValue = SelectedBackground.ToFillBrush(desktop);
        if (BackgroundFocusedColor != null)
            element.BackgroundBrush.FocusedColor = BackgroundFocusedColor.Value.ToXNAColor();
    }

    protected abstract MGElement CreateElementInstance(MgWindow window, MGElement parent);
    /// <param name="includeContent">Recommended value: true. If true, child XAML content, if any, will also be processed.</param>
    protected internal abstract void ApplyDerivedSettings(MGElement parent, MGElement element, bool includeContent);

    protected internal abstract IEnumerable<Element> GetChildren();

    protected internal void ProcessStyles(MgResources resources)
    {
        Dictionary<string, Style> stylesByName = resources.Styles.ToDictionary(x => x.Key, x => x.Value);
        ProcessStyles(stylesByName, new Dictionary<MGElementType, Dictionary<string, List<object>>>());
    }
    private void ProcessStyles(Dictionary<string, Style> stylesByName, Dictionary<MGElementType, Dictionary<string, List<object>>> stylesByType)
    {
        Dictionary<string, List<object>> valuesByProperty;

        //  Append current style setters to indexed data
        foreach (Style style in this.Styles.Where(x => x.Setters.Count > 0))
        {
            if (style.Name != null)
            {
                stylesByName.Add(style.Name, style);
            }
            else
            {
                MGElementType type = style.TargetType;
                if (!stylesByType.TryGetValue(type, out valuesByProperty))
                {
                    valuesByProperty = new();
                    stylesByType.Add(type, valuesByProperty);
                }

                foreach (Setter setter in style.Setters)
                {
                    string property = setter.Property;
                    if (!valuesByProperty.TryGetValue(property, out List<object> values))
                    {
                        values = new();
                        valuesByProperty.Add(property, values);
                    }

                    values.Add(setter.Value);
                }
            }
        }

        //  Apply the appropriate style setters to this instance
        if (this.IsStyleable)
        {
            Type thisType = GetType();

            HashSet<string> modifiedPropertyNames = new();

            //  Apply implicit styles (styles that aren't referenced by a Name)
            if (stylesByType.TryGetValue(this.ElementType, out valuesByProperty))
            {
                foreach (KeyValuePair<string, List<object>> kvp in valuesByProperty)
                {
#if DEBUG
                    //  Sanity check
                    if (kvp.Value.Count == 0)
                        throw new InvalidOperationException($"{nameof(Element)}.{nameof(ProcessStyles)}.{nameof(valuesByProperty)} should never be empty. The indexed data might not be properly updated.");
#endif

                    string propertyName = kvp.Key;
                    PropertyInfo propertyInfo = thisType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance); // | BindingFlags.IgnoreCase?
                    if (propertyInfo != null)
                    {
                        if (modifiedPropertyNames.Contains(propertyName)
                            || propertyInfo.GetValue(this) == default) // Don't allow a style to override a value that was already explicitly set (needs more robust logic since some controls initialize properties to non-null values)
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                            foreach (object value in kvp.Value)
                            {
                                if (value is string stringValue)
                                    propertyInfo.SetValue(this, converter.ConvertFromString(stringValue));
                                else
                                    propertyInfo.SetValue(this, value);
                            }

                            modifiedPropertyNames.Add(propertyName);
                        }
                    }
                }
            }

            //  Apply explicit styles (styles that were explicitly referenced by their Name)
            if (StyleNames != null)
            {
                string[] names = StyleNames.Split(',');
                List<Style> explicitStyles = names.Select(x => stylesByName[x]).Where(x => x.TargetType == this.ElementType).ToList();

                //  Get all the properties that the explicit styles will modify
                HashSet<string> propertyNames = explicitStyles.SelectMany(x => x.Setters).Select(x => x.Property).ToHashSet();
                Dictionary<string, PropertyInfo> propertiesByName = new();
                foreach (string propertyName in propertyNames)
                {
                    PropertyInfo propertyInfo = thisType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance); // | BindingFlags.IgnoreCase?
                    if (propertyInfo != null)
                    {
                        if (modifiedPropertyNames.Contains(propertyName)
                            || propertyInfo.GetValue(this) == default) // Don't allow a style to override a value that was already explicitly set
                        {
                            propertiesByName.Add(propertyName, propertyInfo);
                        }
                    }
                }

                //  Apply the values of each setter
                foreach (Style style in explicitStyles)
                {
                    foreach (Setter setter in style.Setters)
                    {
                        string propertyName = setter.Property;
                        if (propertiesByName.TryGetValue(propertyName, out PropertyInfo propertyInfo))
                        {
                            TypeConverter converter = TypeDescriptor.GetConverter(propertyInfo.PropertyType);
                            if (setter.Value is string stringValue)
                                propertyInfo.SetValue(this, converter.ConvertFromString(stringValue));
                            else
                                propertyInfo.SetValue(this, setter.Value);

                            modifiedPropertyNames.Add(propertyName);
                        }
                    }
                }
            }
        }

        //  Recursively process all children
        foreach (Element child in GetChildren())
        {
            child.ProcessStyles(stylesByName, stylesByType);
        }

        //  Remove current style setters from indexed data
        foreach (Style style in this.Styles.Where(x => x.Setters.Count > 0))
        {
            if (style.Name != null)
            {
                stylesByName.Remove(style.Name);
            }
            else
            {
                MGElementType type = style.TargetType;
                if (stylesByType.TryGetValue(type, out valuesByProperty))
                {
                    foreach (Setter setter in style.Setters)
                    {
                        string property = setter.Property;
                        if (valuesByProperty.TryGetValue(property, out List<object> values))
                        {
                            if (values.Remove(setter.Value) && values.Count == 0)
                            {
                                valuesByProperty.Remove(property);
                                if (valuesByProperty.Count == 0)
                                    stylesByType.Remove(type);
                            }
                        }
                    }
                }
            }
        }
    }
}

public sealed class ElementStringConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
            return true;
        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string stringValue)
        {
            TextBlock textBlock = new() { Text = stringValue };
            return textBlock;
        }

        return base.ConvertFrom(context, culture, value);
    }
}