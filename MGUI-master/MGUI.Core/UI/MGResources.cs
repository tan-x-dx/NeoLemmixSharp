using MGUI.Core.UI.XAML;
using MGUI.Shared.Helpers;
using MGUI.Shared.Rendering;
using MGUI.Shared.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace MGUI.Core.UI;

/// <summary>Stores various resources in Dictionary Key-Value pairs so that they can be accessed by their string keys.<para/>
/// For example: the name of the command that an <see cref="MGButton"/> executes when clicked, the name of a <see cref="Texture2D"/> that an <see cref="MGImage"/> draws,
/// or the name of an object in <see cref="StaticResources"/> for databinding purposes.<para/>
/// This instance is usually accessed via <see cref="MGDesktop.Resources"/> (See also: <see cref="MGElement.GetResources"/>)</summary>
public sealed class MgResources
{
    public MgResources(FontManager fontManager)
        : this(new MGTheme(fontManager.DefaultFontFamily)) { }

    public MgResources(MGTheme defaultTheme)
    {
        this.DefaultTheme = defaultTheme ?? throw new ArgumentNullException(nameof(defaultTheme));
    }

    #region Textures
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, MGTextureData> _textures = new();
    public IReadOnlyDictionary<string, MGTextureData> Textures => _textures;

    public void AddTexture(string name, MGTextureData data)
    {
        _textures.Add(name, data);
        OnTextureAdded?.Invoke(this, (name, data));
    }

    public bool RemoveTexture(string name)
    {
        if (_textures.Remove(name, out var data))
        {
            OnTextureRemoved?.Invoke(this, (name, data));
            return true;
        }

        return false;
    }

    public bool TryGetTexture(string name, out MGTextureData data)
    {
        if (name != null && _textures.TryGetValue(name, out data))
            return true;

        data = default;
        return false;
    }

    internal (int? Width, int? Height) GetTextureDimensions(string name)
    {
        if (TryGetTexture(name, out var texture))
        {
            var size = texture.RenderSize;
            return (size.Width, size.Height);
        }

        return (null, null);
    }

    public bool TryDrawTexture(DrawTransaction dt, string name, Rectangle targetBounds, float opacity = 1.0f, Color? color = null)
        => TryDrawTexture(dt, name, targetBounds.TopLeft(), targetBounds.Width, targetBounds.Height, opacity, color);
    public bool TryDrawTexture(DrawTransaction dt, MGTextureData? textureData, Rectangle targetBounds, float opacity = 1.0f, Color? color = null)
        => TryDrawTexture(dt, textureData, targetBounds.TopLeft(), targetBounds.Width, targetBounds.Height, opacity, color);
    public bool TryDrawTexture(DrawTransaction dt, string name, Point position, int? width, int? height, float opacity = 1.0f, Color? color = null)
    {
        if (TryGetTexture(name, out var textureData))
            return TryDrawTexture(dt, textureData, position, width, height, opacity, color);
        else
            return false;
    }
    public bool TryDrawTexture(DrawTransaction dt, MGTextureData? textureData, Point position, int? width, int? height, float opacity = 1.0f, Color? color = null)
    {
        if (textureData != null)
        {
            var actualWidth = width ?? textureData.Value.RenderSize.Width;
            var actualHeight = height ?? textureData.Value.RenderSize.Height;
            Rectangle destination = new(position.X, position.Y, actualWidth, actualHeight);

            dt.DrawTextureTo(textureData.Value.Texture, textureData.Value.SourceRect, destination, (color ?? Microsoft.Xna.Framework.Color.White) * opacity * textureData.Value.Opacity);

            return true;
        }
        else
            return false;
    }

    public event EventHandler<(string Name, MGTextureData Data)> OnTextureAdded;
    public event EventHandler<(string Name, MGTextureData Data)> OnTextureRemoved;
    #endregion Textures

    #region Commands
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, Action<MGElement>> _commands = new();
    /// <summary>This dictionary is commonly used by <see cref="MGButton.CommandName"/> or by <see cref="MGTextBlock"/> to reference delegates by a string key value.<para/>
    /// See also:<br/><see cref="AddCommand(string, Action{MGElement})"/><br/><see cref="RemoveCommand"/><para/>
    /// EX: If you create an <see cref="MGTextBlock"/> and set its text to:
    /// <code>[Command=ABC]This text invokes a delegate when clicked[/Command] but this text doesn't</code>
    /// then the <see cref="Action{MGElement}"/> with the name "ABC" will be invoked when clicking the substring "This text invokes a delegate when clicked"</summary>
    public IReadOnlyDictionary<string, Action<MGElement>> Commands => _commands;

    /// <param name="name">Must be unique. If the command is intended to be window-specific, 
    /// you may wish to prefix the command name with the <see cref="MgWindow"/>'s <see cref="MGElement.UniqueId"/> to ensure uniqueness.</param>
    public void AddCommand(string name, Action<MGElement> command)
    {
        _commands.Add(name, command);
        OnCommandAdded?.Invoke(this, (name, command));
    }

    public bool RemoveCommand(string name)
    {
        if (_commands.Remove(name, out var command))
        {
            OnCommandRemoved?.Invoke(this, (name, command));
            return true;
        }

        return false;
    }

    public bool TryGetCommand(string name, out Action<MGElement> command)
    {
        if (name != null && _commands.TryGetValue(name, out command))
            return true;

        command = null;
        return false;
    }

    /// <summary>Invoked when a new value is added to <see cref="Commands"/> via <see cref="AddCommand(string, Action{MGElement})"/><para/>
    /// See also: <see cref="OnCommandRemoved"/></summary>
    public event EventHandler<(string Name, Action<MGElement> Command)> OnCommandAdded;
    /// <summary>Invoked when a value is removed from <see cref="Commands"/> via <see cref="RemoveCommand"/><para/>
    /// See also: <see cref="OnCommandAdded"/></summary>
    public event EventHandler<(string Name, Action<MGElement> Command)> OnCommandRemoved;

    //TODO refactor MGWindow.NamedToolTips to be a Dictionary<Window, Dictionary<string, ToolTip>> in MGResources?
    #endregion Commands

    #region Themes
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, MGTheme> _themes = new();
    public IReadOnlyDictionary<string, MGTheme> Themes => _themes;

    public void AddTheme(string name, MGTheme theme)
    {
        _themes.Add(name, theme);
        OnThemeAdded?.Invoke(this, (name, theme));
    }

    public bool RemoveTheme(string name)
    {
        if (_themes.Remove(name, out var theme))
        {
            OnThemeRemoved?.Invoke(this, (name, theme));
            return true;
        }

        return false;
    }

    /// <param name="defaultValue">The default theme to return if there is no theme with the given <paramref name="name"/>. Uses <see cref="DefaultTheme"/> if null.</param>
    /// <param name="warnIfNotFound">If a <paramref name="name"/> is specified but no corresponding theme is found, a warning will be written via <see cref="Debug.WriteLine(string?)"/></param>
    public MGTheme GetThemeOrDefault(string name, MGTheme defaultValue = null, bool warnIfNotFound = true)
    {
        if (!string.IsNullOrEmpty(name))
        {
            if (_themes.TryGetValue(name, out var result))
                return result;
            else if (warnIfNotFound)
                Debug.WriteLine($"Warning - No {nameof(MGTheme)} was found with the name '{name}' in {nameof(MgResources)}.{nameof(Themes)}");
        }

        return defaultValue ?? DefaultTheme;
    }

    public event EventHandler<(string Name, MGTheme Theme)> OnThemeAdded;
    public event EventHandler<(string Name, MGTheme Theme)> OnThemeRemoved;

    private MGTheme _defaultTheme;
    /// <summary>The <see cref="MGTheme"/> to assign to an <see cref="MgWindow"/> after parsing a XAML string (unless the window explicitly specifies a different theme).<para/>
    /// Note: Changing this value will not dynamically update the theme of any windows that have already been parsed. This value is only applied once on each window, when the XAML is parsed.<br/>
    /// So if you do change this value, you may want to re-parse your XAML content to initialize a new window.<para/>
    /// See also: <see cref="XamlParser.LoadRootWindow(MGDesktop, string, bool, bool)"/><para/>
    /// This value cannot be null.</summary>
    public MGTheme DefaultTheme
    {
        get => _defaultTheme;
        set => _defaultTheme = value ?? throw new ArgumentNullException(nameof(DefaultTheme));
    }
    #endregion Themes

    #region Styles
    //TODO maybe this should be split up into ImplicitStyles (which are indexed by their MGElementType) and ExplicitStyles (which are indexed by their Name)
    //  If so, need to modify Element.ProcessStyles to initialize the 'StylesByType' dictionary before proceeding with the styling logic
    //Dictionary<MGElementType, Style> ImplicitStyles
    //      AddImplicitStyle(Style Style)
    //          If style has no setters, return
    //          If the Style has a name, throw exception
    //          look in ImplicitStyles for a value at Style.TargetType
    //          Create if not found. Then merge all the Style Setters together, overwriting any existing setters for properties
    //      RemoveImplicitStyle(MGElementType TargetType)
    //Dictionary<string, Style> ExplicitStyles
    //      AddExplicitStyle(Style Style)
    //          If style has no setters, return
    //          If the Style doesn't have a name, throw exception
    //          Else add it to ExplicitStyles
    //Maybe Window should have bool InheritsImplicitStyles
    //      If true, Window's Element.Styles is pulled from Resources.ImplicitStyles
    //      Or the ImplicitStyles would have to be stored as some kind of named Set?
    //      Dictionary<string, StyleSet> where StyleSet contains List<Style>
    //      Then you could say <Window StyleSetNames="...">...</Window>
    //      to automatically initialize Window.Styles to whatever is found in Resources.StyleSets[StyleSetName]
    //      could treat it as a comma separated list like StyleNames, and apply it to any Element
    //      then in Element.ProcessStyles, foreach (style in this.styles AND foreach style in each styleset that we can find in the resources... append to dictionaries etc

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, Style> _styles = new();
    public IReadOnlyDictionary<string, Style> Styles => _styles;

    public void AddStyle(string name, Style style)
    {
        _styles.Add(name, style);
        OnStyleAdded?.Invoke(this, (name, style));
    }

    public bool RemoveStyle(string name)
    {
        if (_styles.Remove(name, out var style))
        {
            OnStyleRemoved?.Invoke(this, (name, style));
            return true;
        }

        return false;
    }

    public event EventHandler<(string Name, Style Style)> OnStyleAdded;
    public event EventHandler<(string Name, Style Style)> OnStyleRemoved;
    #endregion Styles

    #region StaticResources
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, object> _staticResources = new();
    public IReadOnlyDictionary<string, object> StaticResources => _staticResources;

    public void AddStaticResource(string name, object value)
    {
        _staticResources.Add(name, value);
        OnStaticResourceAdded?.Invoke(this, (name, value));
    }

    public bool RemoveStaticResource(string name)
    {
        if (_staticResources.Remove(name, out var value))
        {
            OnStaticResourceRemoved?.Invoke(this, (name, value));
            return true;
        }

        return false;
    }

    public bool TryGetStaticResource(string name, out object value)
    {
        if (name != null && _staticResources.TryGetValue(name, out value))
            return true;

        value = null;
        return false;
    }

    public event EventHandler<(string Name, object Value)> OnStaticResourceAdded;
    public event EventHandler<(string Name, object Value)> OnStaticResourceRemoved;
    #endregion StaticResources

    #region Element Templates
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, MGElementTemplate> _elementTemplates = new();
    public IReadOnlyDictionary<string, MGElementTemplate> ElementTemplates => _elementTemplates;

    public void AddElementTemplate(MGElementTemplate template)
    {
        _elementTemplates.Add(template.Name, template);
        OnElementTemplateAdded?.Invoke(this, (template.Name, template));
    }

    public bool RemoveElementTemplate(string name)
    {
        if (_elementTemplates.Remove(name, out var template))
        {
            OnElementTemplateRemoved?.Invoke(this, (name, template));
            return true;
        }

        return false;
    }

    public bool TryGetElementTemplate(string name, out MGElementTemplate template)
    {
        if (name != null && _elementTemplates.TryGetValue(name, out template))
            return true;

        template = null;
        return false;
    }

    public event EventHandler<(string Name, MGElementTemplate Template)> OnElementTemplateAdded;
    public event EventHandler<(string Name, MGElementTemplate Template)> OnElementTemplateRemoved;
    #endregion Element Templates
}