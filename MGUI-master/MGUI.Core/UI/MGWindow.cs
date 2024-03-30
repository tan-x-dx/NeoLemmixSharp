using MGUI.Core.UI.Brushes.Border_Brushes;
using MGUI.Core.UI.Brushes.Fill_Brushes;
using MGUI.Core.UI.Containers;
using MGUI.Core.UI.Containers.Grids;
using MGUI.Shared.Helpers;
using MGUI.Shared.Input.Keyboard;
using MGUI.Shared.Input.Mouse;
using MGUI.Shared.Rendering;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace MGUI.Core.UI;

public class CancelEventArgs<T> : CancelEventArgs
{
    public T Data { get; }

    public CancelEventArgs(T data)
        : base(false)
    {
        Data = data;
    }
}

public class MgWindow : MGSingleContentHost
{
    public MGDesktop Desktop { get; }

    #region Position / Size
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Point TopLeft
    {
        get => new(Left, Top);
        set
        {
            Left = value.X;
            Top = value.Y;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _left;
    public int Left
    {
        get => _left;
        set
        {
            if (_left != value)
            {
                _left = value;
                Npc(nameof(Left));
                Npc(nameof(TopLeft));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _top;
    public int Top
    {
        get => _top;
        set
        {
            if (_top != value)
            {
                _top = value;
                Npc(nameof(Top));
                Npc(nameof(TopLeft));
            }
        }
    }

    /// <summary>Note: This event is not invoked immediately after <see cref="Left"/> or <see cref="Top"/> changes.<br/>
    /// It is invoked during the Update tick to improve performance by only allowing it to notify once per tick.</summary>
    public event EventHandler<EventArgs<(int Left, int Top)>> OnWindowPositionChanged;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _windowWidth;
    public int WindowWidth
    {
        get => _windowWidth;
        set
        {
            var actualValue = Math.Clamp(value, MinWidth ?? 0, MaxWidth ?? int.MaxValue);
            if (_windowWidth != actualValue)
            {
                _windowWidth = actualValue;
                LayoutChanged(this, true);
                UpdateScaleTransforms();
                _recentSizeToContentSettings = null;
                Npc(nameof(WindowWidth));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _windowHeight;
    public int WindowHeight
    {
        get => _windowHeight;
        set
        {
            var actualValue = Math.Clamp(value, MinHeight ?? 0, MaxHeight ?? int.MaxValue);
            if (_windowHeight != actualValue)
            {
                _windowHeight = actualValue;
                LayoutChanged(this, true);
                UpdateScaleTransforms();
                _recentSizeToContentSettings = null;
                Npc(nameof(WindowHeight));
            }
        }
    }

    /// <summary>Note: This event is not invoked immediately after <see cref="WindowWidth"/> or <see cref="WindowHeight"/> changes.<br/>
    /// It is invoked during the Update tick to improve performance by only allowing it to notify once per tick.</summary>
    public event EventHandler<EventArgs<(int Width, int Height)>> OnWindowSizeChanged;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _previousLeft;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _previousTop;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _previousWidth;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private int _previousHeight;

    protected void InvokeWindowPositionChanged(Point previousPosition, Point newPosition)
        => OnWindowPositionChanged?.Invoke(this, new((previousPosition.X, previousPosition.Y), (newPosition.X, newPosition.Y)));

    /// <summary>Fires the <see cref="OnWindowPositionChanged"/> and/or <see cref="OnWindowSizeChanged"/> events if necessary.<para/>
    /// This method is automatically invoked at the beginning of <see cref="MGElement.Update(ElementUpdateArgs)"/>,<br/>
    /// but in rare cases you may want to manually invoke this after changing <see cref="Left"/>, <see cref="Top"/>, <see cref="WindowWidth"/>, or <see cref="WindowHeight"/> to make changes take effect immediately.</summary>
    public void ValidateWindowSizeAndPosition()
    {
        if (_previousLeft != Left || _previousTop != Top)
        {
            try
            {
                UpdateScaleTransforms();
                InvokeWindowPositionChanged(new(_previousLeft, _previousTop), new(Left, Top));
            }
            finally
            {
                _previousLeft = Left;
                _previousTop = Top;
            }
        }

        if (_previousWidth != WindowWidth || _previousHeight != WindowHeight)
        {
            try
            {
                UpdateScaleTransforms();
#if NEVER
                    UpdateRenderTarget();
#endif
                OnWindowSizeChanged?.Invoke(this, new((_previousWidth, _previousHeight), (WindowWidth, WindowHeight)));
            }
            finally
            {
                _previousWidth = WindowWidth;
                _previousHeight = WindowHeight;
            }
        }
    }

    public Size ComputeContentSize(int minWidth = 100, int minHeight = 100, int maxWidth = 1920, int maxHeight = 1080)
    {
        Size minSize = new(minWidth, minHeight);
        Size maxSize = new(Math.Min(GetDesktop().ValidScreenBounds.Width, maxWidth), Math.Min(GetDesktop().ValidScreenBounds.Height, maxHeight));
        UpdateMeasurement(maxSize, out _, out var fullSize, out _, out _);
        var size = fullSize.Size.Clamp(minSize, maxSize);
        return size;
    }

    private readonly record struct SizeToContentSettings(SizeToContent Type, int MinWidth, int MinHeight, int? MaxWidth, int? MaxHeight);
    private SizeToContentSettings? _recentSizeToContentSettings = null;

    /// <summary>Resizes this <see cref="MgWindow"/> to satisfy the given constraints.</summary>
    /// <param name="updateLayoutImmediately">If true, the layout of child content is refreshed immediately rather than waiting until the next update tick.</param>
    /// <returns>The computed size that this <see cref="MgWindow"/> will be changed to.</returns>
    public Size ApplySizeToContent(SizeToContent value, int minWidth = 50, int minHeight = 50, int? maxWidth = 1920, int? maxHeight = 1080, bool updateLayoutImmediately = true)
    {
        Size minSize = new(minWidth, minHeight);
        Size maxSize = new(Math.Min(GetDesktop().ValidScreenBounds.Width - Left, maxWidth ?? int.MaxValue), Math.Min(GetDesktop().ValidScreenBounds.Height - Top, maxHeight ?? int.MaxValue));
        var availableSize = GetActualAvailableSize(new Size(WindowWidth, WindowHeight), value).Clamp(minSize, maxSize);
        UpdateMeasurement(availableSize, out _, out var fullSize, out _, out _);
        var size = fullSize.Size.Clamp(minSize, maxSize);
        WindowWidth = size.Width;
        WindowHeight = size.Height;
        LayoutChanged(this, true);

        if (updateLayoutImmediately)
        {
            ValidateWindowSizeAndPosition();
            UpdateLayout(new Rectangle(Left, Top, WindowWidth, WindowHeight));
        }

        _recentSizeToContentSettings = new(value, minWidth, minHeight, maxWidth, maxHeight);

        return size;
    }

    protected static Size GetActualAvailableSize(Size size, SizeToContent sizeToContent)
    {
        var actualAvailableWidth = sizeToContent switch
        {
            SizeToContent.Manual => size.Width,
            SizeToContent.Width => int.MaxValue,
            SizeToContent.Height => size.Width,
            SizeToContent.WidthAndHeight => int.MaxValue,
            _ => throw new NotImplementedException($"Unrecognized {nameof(sizeToContent)}: {sizeToContent}")
        };

        var actualAvailableHeight = sizeToContent switch
        {
            SizeToContent.Manual => size.Height,
            SizeToContent.Width => size.Height,
            SizeToContent.Height => int.MaxValue,
            SizeToContent.WidthAndHeight => int.MaxValue,
            _ => throw new NotImplementedException($"Unrecognized {nameof(sizeToContent)}: {sizeToContent}")
        };

        Size actualAvailableSize = new(actualAvailableWidth, actualAvailableHeight);

        return actualAvailableSize;
    }

    #region Scale
    private float _scale = 1.0f;
    /// <summary>Scales this <see cref="MgWindow"/> from the window's center point.<para/>
    /// Default value: 1.0f</summary>
    public float Scale
    {
        get => _scale;
        set
        {
            if (_scale != value)
            {
                var previous = Scale;
                _scale = value;
                UpdateScaleTransforms();
#if NEVER
                    UpdateRenderTarget();
#endif
                Npc(nameof(Scale));
                Npc(nameof(IsWindowScaled));
                ScaleChanged?.Invoke(this, new(previous, Scale));
            }
        }
    }

    /// <summary>Invoked when <see cref="Scale"/> changes.</summary>
    public event EventHandler<EventArgs<float>> ScaleChanged;

    public bool IsWindowScaled => !Scale.IsAlmostEqual(1.0f);

    private Matrix _unscaledScreenSpaceToScaledScreenSpace;
    /// <summary>A <see cref="Matrix"/> that converts coordinates that haven't accounted for <see cref="Scale"/> to coordinates that have.<para/>
    /// If <see cref="Scale"/> is 1.0f, this value is <see cref="Matrix.Identity"/><para/>
    /// See also: <see cref="ScaledScreenSpaceToUnscaledScreenSpace"/></summary>
    protected internal Matrix UnscaledScreenSpaceToScaledScreenSpace { get => _unscaledScreenSpaceToScaledScreenSpace; }

    private Matrix _scaledScreenSpaceToUnscaledScreenSpace;
    /// <summary>A <see cref="Matrix"/> that converts coordinates in screen space to coordinates that haven't accounted for <see cref="Scale"/>.<para/>
    /// If <see cref="Scale"/> is 1.0f, this value is <see cref="Matrix.Identity"/><para/>
    /// See also: <see cref="UnscaledScreenSpaceToScaledScreenSpace"/></summary>
    protected internal Matrix ScaledScreenSpaceToUnscaledScreenSpace { get => _scaledScreenSpaceToUnscaledScreenSpace; }

    private void UpdateScaleTransforms()
    {
        if (IsWindowScaled)
        {
            var scaleOrigin = TopLeft.ToVector2(); //TopLeft.ToVector2() + new Vector2(WindowWidth / 2, WindowHeight / 2); // Center of window
            _unscaledScreenSpaceToScaledScreenSpace =
                Matrix.CreateTranslation(new Vector3(-scaleOrigin, 0)) *
                Matrix.CreateScale(Scale) *
                Matrix.CreateTranslation(new Vector3(scaleOrigin, 0));
            _scaledScreenSpaceToUnscaledScreenSpace = Matrix.Invert(UnscaledScreenSpaceToScaledScreenSpace);
        }
        else
        {
            _unscaledScreenSpaceToScaledScreenSpace = Matrix.Identity;
            _scaledScreenSpaceToUnscaledScreenSpace = Matrix.Identity;
        }
    }

#if NEVER
        private void UpdateRenderTarget()
        {
            if (IsWindowScaled)
                RenderTarget = RenderUtils.CreateRenderTarget(GetDesktop().Renderer.GraphicsDevice, WindowWidth, WindowHeight, true);
            else
                RenderTarget = null;
        }

        private RenderTarget2D _RenderTarget;
        private RenderTarget2D RenderTarget
        {
            get => _RenderTarget;
            set
            {
                if (_RenderTarget != value)
                {
                    _RenderTarget?.Dispose();
                    _RenderTarget = value;
                }
            }
        }
#endif
    #endregion Scale

    #region Resizing
    /// <summary>Provides direct access to the resizer grip that appears in the bottom-right corner of this textbox when <see cref="IsUserResizable"/> is true.</summary>
    public MGComponent<MGResizeGrip> ResizeGripComponent { get; }
    private MGResizeGrip ResizeGripElement { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _isUserResizable;
    /// <summary>If true, a <see cref="MGResizeGrip"/> will be visible in the bottom-right corner of the window, allowing the user to click+drag it to adjust this <see cref="MgWindow"/>'s <see cref="WindowWidth"/>/<see cref="WindowHeight"/><para/>
    /// Default value: true (except in special-cases such as <see cref="MGComboBox{TItemType}.Dropdown"/> window or for <see cref="MGToolTip"/>s)</summary>
    public bool IsUserResizable
    {
        get => _isUserResizable;
        set
        {
            _isUserResizable = value;
            ResizeGripElement.Visibility = IsUserResizable ? Visibility.Visible : Visibility.Collapsed;
            Npc(nameof(IsUserResizable));
        }
    }
    #endregion Resizing
    #endregion Position / Size

    #region Border
    /// <summary>Provides direct access to this element's border.</summary>
    public MGComponent<MGBorder> BorderComponent { get; }
    private MGBorder BorderElement { get; }
    public override MGBorder GetBorder() => BorderElement;

    public IBorderBrush BorderBrush
    {
        get => BorderElement.BorderBrush;
        set => BorderElement.BorderBrush = value;
    }

    public Thickness BorderThickness
    {
        get => BorderElement.BorderThickness;
        set => BorderElement.BorderThickness = value;
    }
    #endregion Border

    #region Nested Windows
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MgWindow _modalWindow;
    /// <summary>A child <see cref="MgWindow"/> of this <see cref="MgWindow"/>, which blocks all input handling on this <see cref="MgWindow"/></summary>
    public MgWindow ModalWindow
    {
        get => _modalWindow;
        set
        {
            if (_modalWindow != value)
            {
                var previous = ModalWindow;
                _modalWindow = value;
                Npc(nameof(ModalWindow));
                Npc(nameof(HasModalWindow));
                previous?.Npc(nameof(IsModalWindow));
                ModalWindow?.Npc(nameof(IsModalWindow));
            }
        }
    }
    /// <summary>True if a modal window is being displayed overtop of this window.</summary>
    public bool HasModalWindow => ModalWindow != null;
    /// <summary>True if this window instance is the modal window of its parent window.</summary>
    public bool IsModalWindow => ParentWindow?.ModalWindow == this;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly List<MgWindow> _nestedWindows;
    /// <summary>The last element represents the <see cref="MgWindow"/> that will be 
    /// drawn last (I.E., rendered overtop of everything else), and updated first (I.E., has the first chance to handle inputs)<br/>
    /// except in cases where a Topmost window is prioritized (See: <see cref="IsTopmost"/>)<para/>
    /// This list does not include <see cref="ModalWindow"/>, which is always prioritized over all <see cref="NestedWindows"/></summary>
    public IReadOnlyList<MgWindow> NestedWindows => _nestedWindows;
    public void AddNestedWindow(MgWindow nestedWindow)
    {
        if (nestedWindow == null)
            throw new ArgumentNullException(nameof(nestedWindow));
        if (_nestedWindows.Contains(nestedWindow))
            throw new ArgumentException("Cannot add the same nested window to a parent window multiple times.");
        if (nestedWindow == this)
            throw new ArgumentException("Cannot add a window as a nested window to itself as this would create an infinite recursive dependency.");
        _nestedWindows.Add(nestedWindow);
    }
    public bool RemoveNestedWindow(MgWindow nestedWindow) => _nestedWindows.Remove(nestedWindow);

    /// <summary>Moves the given <paramref name="nestedWindow"/> to the end of <see cref="NestedWindows"/> list.<br/>
    /// It will typically be rendered overtop of all other <see cref="NestedWindows"/>, unless another nested window is Topmost (See: <see cref="IsTopmost"/>).<para/>
    /// If there is a <see cref="ModalWindow"/>, then the <see cref="ModalWindow"/> will be rendered overtop of the front-most <paramref name="nestedWindow"/></summary>
    /// <param name="nestedWindow"></param>
    /// <returns>True if the <paramref name="nestedWindow"/> was brought to the front. False if it was not a valid element in <see cref="NestedWindows"/>.</returns>
    public bool BringToFront(MgWindow nestedWindow)
    {
        if (!_nestedWindows.Contains(nestedWindow))
        {
            return false;
        }
        else
        {
            if (_nestedWindows.IndexOf(nestedWindow) != _nestedWindows.Count - 1)
            {
                _nestedWindows.Remove(nestedWindow);
                _nestedWindows.Add(nestedWindow);
            }
            return true;
        }
    }

    /// <summary>Moves the given <paramref name="nestedWindow"/> to the start of <see cref="NestedWindows"/> list.<br/>
    /// It will typically be rendered underneath of all other <see cref="NestedWindows"/>, unless it is Topmost (See: <see cref="IsTopmost"/>)</summary>
    /// <param name="nestedWindow"></param>
    /// <returns>True if the <paramref name="nestedWindow"/> was moved to the back. False if it was not a valid element in <see cref="NestedWindows"/>.</returns>
    public bool BringToBack(MgWindow nestedWindow)
    {
        if (!_nestedWindows.Contains(nestedWindow))
        {
            return false;
        }
        else
        {
            if (_nestedWindows.IndexOf(nestedWindow) != 0)
            {
                _nestedWindows.Remove(nestedWindow);
                _nestedWindows.Insert(0, nestedWindow);
            }
            return true;
        }
    }

    public IEnumerable<MgWindow> RecurseNestedWindows(bool includeSelf, TreeTraversalMode traversalMode = TreeTraversalMode.Postorder)
    {
        if (includeSelf && traversalMode == TreeTraversalMode.Preorder)
            yield return this;

        foreach (var nested in NestedWindows)
        {
            foreach (var item in nested.RecurseNestedWindows(true, traversalMode))
                yield return item;
        }

        if (includeSelf && traversalMode == TreeTraversalMode.Postorder)
            yield return this;
    }
    #endregion Nested Windows

    #region Title Bar
    /// <summary>Provides direct access to the dockpanel component that displays this window's title-bar content.<para/>
    /// See also: <see cref="IsTitleBarVisible"/>, <see cref="TitleBarTextBlockElement"/></summary>
    public MGComponent<MGDockPanel> TitleBarComponent { get; }
    private MGDockPanel TitleBarElement { get; }

    /// <summary>The textblock element that contains this window's <see cref="TitleText"/> in the title-bar.</summary>
    public MGTextBlock TitleBarTextBlockElement { get; }

    /// <summary>This property is functionally equivalent to <see cref="TitleBarTextBlockElement"/>'s <see cref="MGTextBlock.Text"/> property.<para/>
    /// See also: <see cref="IsTitleBarVisible"/></summary>
    public string TitleText
    {
        get => TitleBarTextBlockElement.Text;
        set
        {
            if (TitleBarTextBlockElement.Text != value)
            {
                TitleBarTextBlockElement.Text = value;
                Npc(nameof(TitleText));
            }
        }
    }

    /// <summary>True if the title bar should be visible at the top of this <see cref="MgWindow"/><para/>
    /// Default value: true for most types of <see cref="MgWindow"/>, false for <see cref="MGToolTip"/></summary>
    public bool IsTitleBarVisible
    {
        get => TitleBarElement.Visibility == Visibility.Visible;
        set
        {
            var actualValue = value ? Visibility.Visible : Visibility.Collapsed;
            if (TitleBarElement.Visibility != actualValue)
            {
                TitleBarElement.Visibility = actualValue;
                Npc(nameof(IsTitleBarVisible));
            }
        }
    }

    #region Close
    public MGButton CloseButtonElement { get; }

    public bool IsCloseButtonVisible
    {
        get => CloseButtonElement.Visibility == Visibility.Visible;
        set
        {
            var actualValue = value ? Visibility.Visible : Visibility.Collapsed;
            if (CloseButtonElement.Visibility != actualValue)
            {
                CloseButtonElement.Visibility = actualValue;
                Npc(nameof(IsCloseButtonVisible));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _canCloseWindow = true;
    public bool CanCloseWindow
    {
        get => _canCloseWindow;
        set
        {
            if (_canCloseWindow != value)
            {
                _canCloseWindow = value;
                Npc(nameof(CanCloseWindow));
            }
        }
    }

    public bool TryCloseWindow()
    {
        if (!CanCloseWindow)
            return false;

        if ((ParentWindow != null && (ParentWindow.NestedWindows.Contains(this) || ParentWindow.ModalWindow == this))
            || (ParentWindow == null && Desktop.Windows.Contains(this)))
        {
            if (WindowClosing != null)
            {
                CancelEventArgs closingArgs = new();
                WindowClosing.Invoke(this, closingArgs);
                if (closingArgs.Cancel)
                    return false;
            }

            var isClosed = false;
            if (ParentWindow != null && ParentWindow.ModalWindow == this)
            {
                ParentWindow.ModalWindow = null;
                isClosed = true;
            }
            if (ParentWindow != null && ParentWindow.NestedWindows.Contains(this))
                isClosed = ParentWindow.RemoveNestedWindow(this);
            else if (ParentWindow == null && Desktop.Windows.Contains(this))
                isClosed = Desktop.Windows.Remove(this);

            if (isClosed)
                WindowClosed?.Invoke(this, EventArgs.Empty);
            return isClosed;
        }

        return false;
    }

    public event EventHandler<CancelEventArgs> WindowClosing;
    public event EventHandler<EventArgs> WindowClosed;
    #endregion Close
    #endregion Title Bar

    #region RadioButton Groups
    private Dictionary<string, MGRadioButtonGroup> RadioButtonGroups { get; }
    public bool HasRadioButtonGroup(string name) => RadioButtonGroups.ContainsKey(name);
    public MGRadioButtonGroup GetOrCreateRadioButtonGroup(string name)
    {
        if (RadioButtonGroups.TryGetValue(name, out var existingGroup))
            return existingGroup;
        else
        {
            MGRadioButtonGroup newGroup = new(this, name);
            RadioButtonGroups.Add(name, newGroup);
            return newGroup;
        }
    }
    #endregion RadioButton Groups

    #region Named ToolTips

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Dictionary<string, MGToolTip> _namedToolTips;
    /// <summary>This dictionary is commonly used by <see cref="MGTextBlock"/> to reference <see cref="MGToolTip"/>s by a string key value.<para/>
    /// See also:<br/><see cref="AddNamedToolTip(string, MGToolTip)"/><br/><see cref="RemoveNamedToolTip(string)"/><para/>
    /// EX: If you create an <see cref="MGTextBlock"/> and set its text to:
    /// <code>[ToolTip=ABC]This text has a ToolTip[/ToolTip] but this text doesn't</code>
    /// then the ToolTip with the name "ABC" will be shown when hovering over the substring "This text has a ToolTip"</summary>
    public IReadOnlyDictionary<string, MGToolTip> NamedToolTips => _namedToolTips;

    public void AddNamedToolTip(string name, MGToolTip toolTip) => _namedToolTips.Add(name, toolTip);
    public void RemoveNamedToolTip(string name) => _namedToolTips.Remove(name);
    #endregion Named ToolTips

    /// <summary>A <see cref="MouseHandler"/> that is updated just before <see cref="MGElement.MouseHandler"/> is updated.<para/>
    /// This allows subscribing to mouse events that can get handled just before the <see cref="MgWindow"/>'s input handling can occur.</summary>
    public MouseHandler WindowMouseHandler { get; }
    /// <summary>A <see cref="KeyboardHandler"/> that is updated just before <see cref="MGElement.KeyboardHandler"/> is updated.<para/>
    /// This allows subscribing to keyboard events that can get handled just before the <see cref="MgWindow"/>'s input handling can occur.</summary>
    public KeyboardHandler WindowKeyboardHandler { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _allowsClickThrough = false;
    /// <summary>If true, mouse clicks overtop of this <see cref="MgWindow"/> that weren't handled by any child elements will remain unhandled,<br/>
    /// allowing content underneath this <see cref="MgWindow"/> to handle the mouse event.<para/>
    /// Default value: false<para/>
    /// This property is ignored if <see cref="IsModalWindow"/> is true.</summary>
    public bool AllowsClickThrough
    {
        get => _allowsClickThrough;
        set
        {
            if (_allowsClickThrough != value)
            {
                _allowsClickThrough = value;
                Npc(nameof(AllowsClickThrough));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _isDraggable = true;
    /// <summary>True if this <see cref="MgWindow"/> can be moved by dragging the title bar.<para/>
    /// Warning: You may need to set <see cref="IsTitleBarVisible"/> to true to utilize this feature.</summary>
    public bool IsDraggable
    {
        get => _isDraggable;
        set
        {
            if (_isDraggable != value)
            {
                _isDraggable = value;
                Npc(nameof(IsDraggable));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MGTheme _theme;
    /// <summary>If null, uses <see cref="MGDesktop.Theme"/> instead.<para/>
    /// Default value: null<para/>
    /// See also:<br/><see cref="MGElement.GetTheme()"/><br/><see cref="MGDesktop.Theme"/></summary>
    public MGTheme Theme
    {
        get => _theme;
        set
        {
            if (_theme != value)
            {
                _theme = value;
                Npc(nameof(Theme));
            }
        }
    }

    private bool _isTopmost;
    /// <summary>If true, this <see cref="MgWindow"/> will always be updated first (so that it has first-chance to receive and handle inputs)
    /// and drawn last (so that it appears overtop of all other <see cref="MgWindow"/>s).<para/>
    /// This property is only respected within the window's scope.<br/>
    /// If this window is topmost, but is also a nested window of a non-topmost window, it will only be on top of it's siblings (the other nested windows of its parent)<para/>
    /// If multiple windows are topmost, their draw/update priority depends on their index in <see cref="MGDesktop.Windows"/> and <see cref="NestedWindows"/><para/>
    /// <see cref="ModalWindow"/>s will still take priority even over topmost windows.</summary>
    public bool IsTopmost
    {
        get => _isTopmost;
        set
        {
            if (_isTopmost != value)
            {
                _isTopmost = value;
                Npc(nameof(IsTopmost));
            }
        }
    }

    private MGElement PressedElementAtBeginUpdate { get; set; }
    private MGElement HoveredElementAtBeginUpdate { get; set; }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MGElement _pressedElement;
    /// <summary>The inner-most element of the visual tree that the mouse was hovering at the moment that the left mouse button was pressed.<br/>
    /// Null if the left mouse button is currently released or the mouse wasn't hovering an element when the button was pressed down.<para/>
    /// See also: <see cref="HoveredElement"/>, <see cref="PressedElementChanged"/></summary>
    public MGElement PressedElement
    {
        get => _pressedElement;
        private set
        {
            if (_pressedElement != value)
            {
                _pressedElement = value;
                Npc(nameof(PressedElement));
            }
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private MGElement _hoveredElement;
    /// <summary>The inner-most element of the visual tree that the mouse is currently hovering, if any.<para/>
    /// If the mouse is hovering several sibling elements (such as children of an <see cref="MGOverlayPanel"/>, or elements placed inside the same cell of an <see cref="MGGrid"/>)<br/>
    /// then this property prioritizes the topmost element.<para/>
    /// See also: <see cref="PressedElement"/>, <see cref="HoveredElementChanged"/></summary>
    public MGElement HoveredElement
    {
        get => _hoveredElement;
        private set
        {
            if (_hoveredElement != value)
            {
                _hoveredElement = value;
                Npc(nameof(HoveredElement));
            }
        }
    }

    /// <summary>Invoked after <see cref="PressedElement"/> changes, but is intentionally deferred until the end of the current update tick so that <see cref="MGElement.VisualState"/> 
    /// values are properly synced with the <see cref="PressedElement"/></summary>
    public event EventHandler<EventArgs<MGElement>> PressedElementChanged;
    /// <summary>Invoked after <see cref="HoveredElement"/> changes, but is intentionally deferred until the end of the current update tick so that <see cref="MGElement.VisualState"/> 
    /// values are properly synced with the <see cref="HoveredElement"/></summary>
    public event EventHandler<EventArgs<MGElement>> HoveredElementChanged;

    internal bool InvalidatePressedAndHoveredElements { get; set; } = false;

    /// <summary>If true, this <see cref="MgWindow"/>'s layout will be recomputed at the start of the next update tick.</summary>
    public bool QueueLayoutRefresh { get; set; }

    private static readonly Thickness DefaultWindowPadding = new(5);
    private static readonly Thickness DefaultWindowBorderThickness = new(2);

    #region Data Context
    private object _windowDataContext;
    /// <summary>The default <see cref="MGElement.DataContext"/> for all elements that do not explicitly define a <see cref="MGElement.DataContextOverride"/>.<para/>
    /// If not specified, this value is automatically inherited from the <see cref="MGElement.ParentWindow"/> if there is one.</summary>
    public object WindowDataContext
    {
        get => _windowDataContext ?? ParentWindow?.WindowDataContext;
        set
        {
            if (_windowDataContext != value)
            {
                _windowDataContext = value;
                Npc(nameof(WindowDataContext));
                Npc(nameof(DataContextOverride));
                Npc(nameof(DataContext));
                WindowDataContextChanged?.Invoke(this, WindowDataContext);
                //  Changing the DataContext may have changed the sizes of elements on this window, so attempt to resize the window to its contents
                //RevalidateSizeToContent(false);
            }
        }
    }

    public override object DataContextOverride
    {
        get => WindowDataContext;
        set => WindowDataContext = value;
    }

    public event EventHandler<object> WindowDataContextChanged;

    private void RevalidateSizeToContent(bool updateImmediately)
    {
        if (_recentSizeToContentSettings.HasValue)
        {
            ApplySizeToContent(_recentSizeToContentSettings.Value.Type, _recentSizeToContentSettings.Value.MinWidth, _recentSizeToContentSettings.Value.MinHeight,
                _recentSizeToContentSettings.Value.MaxWidth, _recentSizeToContentSettings.Value.MaxHeight, updateImmediately);
        }
    }
    #endregion Data Context

    #region Constructors
    /// <summary>Initializes a root-level window.</summary>
    public MgWindow(MGDesktop desktop, int left, int top, int width, int height, MGTheme theme = null)
        : this(desktop, theme, null, MGElementType.Window, left, top, width, height)
    {

    }

    /// <summary>Initializes a nested window (such as a popup). You should still call <see cref="AddNestedWindow(MgWindow)"/> (or set <see cref="ModalWindow"/>) afterwards.</summary>
    public MgWindow(MgWindow window, int left, int top, int width, int height, MGTheme theme = null)
        : this(window.Desktop, theme, window, MGElementType.Window, left, top, width, height)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));
    }

    /// <exception cref="InvalidOperationException">Thrown if you attempt to change <see cref="MGElement.HorizontalAlignment"/> or <see cref="MGElement.VerticalAlignment"/> on this <see cref="MgWindow"/></exception>
    protected MgWindow(MGDesktop desktop, MGTheme windowTheme, MgWindow parentWindow, MGElementType elementType, int left, int top, int width, int height)
        : base(desktop, windowTheme, parentWindow, elementType)
    {
        if (parentWindow == null && !WindowElementTypes.Contains(elementType))
            throw new InvalidOperationException($"All {nameof(MGElement)}s must either belong to an {nameof(MgWindow)} or be a root-level {nameof(MgWindow)} instance.");

        using (BeginInitializing())
        {
            Desktop = desktop ?? throw new ArgumentNullException(nameof(desktop));
            Theme = windowTheme;

            var actualTheme = GetTheme();

            WindowMouseHandler = InputTracker.Mouse.CreateHandler(this, null);
            WindowKeyboardHandler = InputTracker.Keyboard.CreateHandler(this, null);
            MouseHandler.DragStartCondition = DragStartCondition.Both;

            RadioButtonGroups = new();
            _nestedWindows = new();
            _namedToolTips = new();
            ModalWindow = null;

            Left = left;
            _previousLeft = left;
            Top = top;
            _previousTop = top;

            WindowWidth = width;
            _previousWidth = width;
            WindowHeight = height;
            _previousHeight = height;

            MinWidth = 50;
            MinHeight = 50;
            MaxWidth = 4000;
            MaxHeight = 2000;

            Padding = DefaultWindowPadding;

            BorderElement = new(this, DefaultWindowBorderThickness, MGUniformBorderBrush.Black);
            BorderComponent = MGComponentBase.Create(BorderElement);
            AddComponent(BorderComponent);
            BorderElement.OnBorderBrushChanged += (sender, e) => { Npc(nameof(BorderBrush)); };
            BorderElement.OnBorderThicknessChanged += (sender, e) => { Npc(nameof(BorderThickness)); };

            TitleBarElement = new(this);
            TitleBarElement.Padding = new(2);
            TitleBarElement.MinHeight = 24;
            TitleBarElement.HorizontalAlignment = HorizontalAlignment.Stretch;
            TitleBarElement.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            TitleBarElement.VerticalAlignment = VerticalAlignment.Stretch;
            TitleBarElement.VerticalContentAlignment = VerticalAlignment.Stretch;
            TitleBarElement.BackgroundBrush = actualTheme.TitleBackground.GetValue(true);

            TitleBarComponent = new(TitleBarElement, true, false, true, true, false, false, false,
                (availableBounds, componentSize) => ApplyAlignment(availableBounds, HorizontalAlignment.Stretch, VerticalAlignment.Top, componentSize.Size));
            AddComponent(TitleBarComponent);
#if NEVER
                TitleBarElement.OnEndDraw += (sender, e) =>
                {
                    //  Attempt to draw the window's border around just the titlebar, so that the border is also drawn just underneath the title bar
                    if (IsTitleBarVisible)
                    {
                        Rectangle TargetBounds = new(TitleBarElement.LayoutBounds.Left - BorderThickness.Left, TitleBarElement.LayoutBounds.Top - BorderThickness.Top, 
                            TitleBarElement.LayoutBounds.Width + BorderThickness.Width, TitleBarElement.LayoutBounds.Height + BorderThickness.Top + BorderThickness.Bottom / 2);
                        BorderElement.BorderBrush.Draw(e.DA, this, TargetBounds, BorderThickness);
                    }
                };
#endif

            CloseButtonElement = new(this, x => { TryCloseWindow(); });
            CloseButtonElement.MinWidth = 12;
            CloseButtonElement.MinHeight = 12;
            CloseButtonElement.BackgroundBrush = new(Color.Crimson.AsFillBrush() * 0.5f, Color.White * 0.18f, PressedModifierType.Darken, 0.06f);
            CloseButtonElement.BorderBrush = MGUniformBorderBrush.Black;
            CloseButtonElement.BorderThickness = new(1);
            CloseButtonElement.Margin = new(1, 1, 1, 1 + BorderElement.BorderThickness.Bottom);
            CloseButtonElement.Padding = new(4, -1);
            CloseButtonElement.VerticalAlignment = VerticalAlignment.Center;
            CloseButtonElement.VerticalContentAlignment = VerticalAlignment.Center;
            CloseButtonElement.HorizontalContentAlignment = HorizontalAlignment.Center;
            CloseButtonElement.SetContent(new MGTextBlock(this, "[b][shadow=Black 1 1]x[/shadow][/b]", Color.White));
            //CloseButtonElement.CanChangeContent = false;

            TitleBarTextBlockElement = new(this, null, Color.White, actualTheme.FontSettings.SmallFontSize)
            {
                Margin = new(4, 0),
                Padding = new(0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = HorizontalAlignment.Left,
                DefaultTextForeground = new VisualStateSetting<Color?>(Color.White, Color.White, Color.White)
            };
            TitleText = null;

            TitleBarElement.TryAddChild(CloseButtonElement, Dock.Right);
            TitleBarElement.TryAddChild(TitleBarTextBlockElement, Dock.Left);
            TitleBarElement.CanChangeContent = false;

            IsTitleBarVisible = true;

            ResizeGripElement = new(this);
            ResizeGripComponent = MGComponentBase.Create(ResizeGripElement);
            AddComponent(ResizeGripComponent);
            IsUserResizable = true;

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;

            OnHorizontalAlignmentChanged += (sender, e) =>
            {
                if (elementType == MGElementType.Window && e.NewValue != HorizontalAlignment.Stretch)
                {
                    var error = $"The {nameof(HorizontalAlignment)} of a root-level window element must always be set to: " +
                                $"{nameof(HorizontalAlignment)}.{nameof(HorizontalAlignment.Stretch)}";
                    throw new InvalidOperationException(error);
                }
            };
            OnVerticalAlignmentChanged += (sender, e) =>
            {
                if (elementType == MGElementType.Window && e.NewValue != VerticalAlignment.Stretch)
                {
                    var error = $"The {nameof(VerticalAlignment)} of a root-level window element must always be set to: " +
                                $"{nameof(VerticalAlignment)}.{nameof(VerticalAlignment.Stretch)}";
                    throw new InvalidOperationException(error);
                }
            };

            ElementsByName = new();
            OnDirectOrNestedContentAdded += Element_Added;
            OnDirectOrNestedContentRemoved += Element_Removed;

            OnBeginUpdate += (sender, e) =>
            {
                PressedElementAtBeginUpdate = PressedElement;
                HoveredElementAtBeginUpdate = HoveredElement;

                var shouldUpdateHoveredElement = MouseHandler.Tracker.MouseMovedRecently || !IsLayoutValid || QueueLayoutRefresh || InvalidatePressedAndHoveredElements;

                ValidateWindowSizeAndPosition();

                if (!IsLayoutValid || QueueLayoutRefresh)
                {
                    QueueLayoutRefresh = false;
                    if (_recentSizeToContentSettings.HasValue)
                        RevalidateSizeToContent(true);
                    else
                        UpdateLayout(new(Left, Top, WindowWidth, WindowHeight));
                }

                if (shouldUpdateHoveredElement)
                    HoveredElement = GetTopmostHoveredElement(e.UA);

                if (MouseHandler.Tracker.MouseLeftButtonPressedRecently)
                    PressedElement = GetTopmostHoveredElement(e.UA);
                else if (MouseHandler.Tracker.MouseLeftButtonReleasedRecently)
                    PressedElement = null;
            };

            OnEndUpdate += (sender, e) =>
            {
                //  These 2 events are intentionally deferred because they affect MGElement.VisualState,
                //  and subscribing code probably wants to access the most up-to-date MGElement.VisualState values.
                if (PressedElementAtBeginUpdate != PressedElement)
                    PressedElementChanged?.Invoke(this, new(PressedElementAtBeginUpdate, PressedElement));
                if (HoveredElementAtBeginUpdate != HoveredElement)
                    HoveredElementChanged?.Invoke(this, new(HoveredElementAtBeginUpdate, HoveredElement));
            };

            //  Ensure all mouse events that haven't already been handled by a child element of this window are handled, so that the mouse events won't fall-through to underneath this window
            MouseHandler.PressedInside += (sender, e) =>
            {
                if (!AllowsClickThrough || IsModalWindow)
                    e.SetHandledBy(this, false);
            };
            MouseHandler.ReleasedInside += (sender, e) =>
            {
                if (!AllowsClickThrough || IsModalWindow)
                    e.SetHandledBy(this, false);
            };
            MouseHandler.DragStart += (sender, e) =>
            {
                if (!AllowsClickThrough || IsModalWindow)
                    e.SetHandledBy(this, false);
            };
            MouseHandler.Scrolled += (sender, e) =>
            {
                if (!AllowsClickThrough || IsModalWindow)
                    e.SetHandledBy(this, false);
            };
            MouseHandler.PressedOutside += (sender, e) =>
            {
                if (IsModalWindow)
                    e.SetHandledBy(this, false);
            };
            MouseHandler.ReleasedOutside += (sender, e) =>
            {
                if (IsModalWindow)
                    e.SetHandledBy(this, false);
            };
            MouseHandler.DragStartOutside += (sender, e) =>
            {
                if (IsModalWindow)
                    e.SetHandledBy(this, false);
            };

            OnBeginUpdateContents += (sender, e) =>
            {
                var updateArgs = e.UA.ChangeOffset(Origin);

                //TODO does this order make sense?
                //What if this window has both a ModalWindow and a NestedWindow, and the NestedWindow has a ModalWindow.
                //Should we update the ModalWindow of the NestedWindow before we update the ModalWindow of this Window?
                ModalWindow?.Update(updateArgs);

                foreach (var nested in _nestedWindows.Reverse<MgWindow>().OrderByDescending(x => x.IsTopmost))
                    nested.Update(updateArgs);
            };

            OnEndUpdateContents += (sender, e) =>
            {
                WindowMouseHandler.ManualUpdate();
                WindowKeyboardHandler.ManualUpdate();
            };

            //  Nested windows inherit their WindowDataContext from the parent if they don't have their own explicit value
            if (parentWindow != null)
            {
                parentWindow.WindowDataContextChanged += (sender, e) =>
                {
                    if (_windowDataContext == null)
                    {
                        Npc(nameof(WindowDataContext));
                        Npc(nameof(DataContextOverride));
                        Npc(nameof(DataContext));
                        WindowDataContextChanged?.Invoke(this, WindowDataContext);
                        RevalidateSizeToContent(false);
                    }
                };
            }

            MakeDraggable();
        }
    }
    #endregion Constructors

    #region Drag Window Position
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _isDraggingWindowPosition = false;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private Point? _dragWindowPositionOffset = null;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private bool _isDrawingDraggedWindowPreview = false;

    /// <summary>Intended to be called once during initialization. Subscribes to the appropriate events to allow the user to move this <see cref="MgWindow"/> by clicking and dragging the window's Title bar.</summary>
    private void MakeDraggable()
    {
        MouseHandler.DragStart += (sender, e) =>
        {
            if (e.IsLMB && IsDraggable && e.Condition == DragStartCondition.MouseMovedAfterPress && IsTitleBarVisible)
            {
                var layoutSpacePosition = ConvertCoordinateSpace(CoordinateSpace.Screen, CoordinateSpace.Layout, e.Position);
                if (TitleBarElement.LayoutBounds.ContainsInclusive(layoutSpacePosition) && !CloseButtonElement.LayoutBounds.ContainsInclusive(layoutSpacePosition))
                {
                    _isDraggingWindowPosition = true;
                    _dragWindowPositionOffset = Point.Zero;
                    e.SetHandledBy(this, false);
                    //Debug.WriteLine($"{nameof(MGWindow)}: Drag Start at: {e.Position}");
                }
            }
        };

        MouseHandler.Dragged += (sender, e) =>
        {
            if (e.IsLMB && _isDraggingWindowPosition)
            {
                var scalar = 1.0f / Scale;
                Point delta = new((int)(e.PositionDelta.X * scalar), (int)(e.PositionDelta.Y * scalar));
                _dragWindowPositionOffset = delta;
            }
        };

        MouseHandler.DragEnd += (sender, e) =>
        {
            try
            {
                if (e.IsLMB && _isDraggingWindowPosition && _dragWindowPositionOffset.HasValue)
                {
                    Point delta = new((int)(_dragWindowPositionOffset.Value.X * Scale), (int)(_dragWindowPositionOffset.Value.Y * Scale));

                    Left += delta.X;
                    Top += delta.Y;

                    foreach (var nested in _nestedWindows)
                    {
                        nested.Left += delta.X;
                        nested.Top += delta.Y;
                    }

                    if (ModalWindow != null)
                    {
                        ModalWindow.Left += delta.X;
                        ModalWindow.Top += delta.Y;
                    }

                    MouseHandler.Tracker.CurrentButtonReleasedEvents[MouseButton.Left]?.SetHandledBy(this, false);

                    //Debug.WriteLine($"{nameof(MGWindow)}: Drag End at: {e.EndPosition}");
                }
            }
            finally
            {
                _isDraggingWindowPosition = false;
                _dragWindowPositionOffset = null;
            }
        };

        //  Pre-emptively handle mouse release events if user was dragging this window
        //  so that the child content doesn't also react to the mouse release
        //  (such as if you end the mouse drag by releasing the mouse overtop of a button)
        OnBeginUpdateContents += (sender, e) =>
        {
            if (_isDraggingWindowPosition)
            {
                MouseHandler.Tracker.CurrentButtonReleasedEvents[MouseButton.Left]?.SetHandledBy(this, false);
            }
        };
    }
    #endregion Drag Window Position

    #region Indexed Elements
    private Dictionary<string, MGElement> ElementsByName { get; }

    public MGElement GetElementByName(string name) => ElementsByName[name];
    public T GetElementByName<T>(string name) where T : MGElement => ElementsByName[name] as T;

    public bool TryGetElementByName(string name, out MGElement element) => ElementsByName.TryGetValue(name, out element);
    public bool TryGetElementByName<T>(string name, out T element) where T : MGElement
    {
        if (TryGetElementByName(name, out var result))
        {
            element = result as T;
            return element != null;
        }
        else
        {
            element = default;
            return false;
        }
    }

    private void Element_Added(object sender, MGElement e)
    {
        if (e.Name != null)
            ElementsByName.Add(e.Name, e);

        if (e.ToolTip != null)
        {
            var tt = e.ToolTip;
            foreach (var element in tt.TraverseVisualTree(true, false, false, false, TreeTraversalMode.Preorder))
            {
                Element_Added(tt, element);
            }
        }

        if (e.ContextMenu != null)
        {
            var cm = e.ContextMenu;
            foreach (var element in cm.TraverseVisualTree(true, false, false, false, TreeTraversalMode.Preorder))
            {
                Element_Added(cm, element);
            }
        }

        e.ToolTipChanged += Element_ToolTipChanged;
        e.ContextMenuChanged += Element_ContextMenuChanged;
        e.OnNameChanged += Element_NameChanged;
    }

    private void Element_Removed(object sender, MGElement e)
    {
        if (e.Name != null)
            ElementsByName.Remove(e.Name);

        if (e.ToolTip != null)
        {
            var tt = e.ToolTip;
            foreach (var element in tt.TraverseVisualTree(true, false, false, false, TreeTraversalMode.Preorder))
            {
                Element_Removed(tt, element);
            }
        }

        if (e.ContextMenu != null)
        {
            var cm = e.ContextMenu;
            foreach (var element in cm.TraverseVisualTree(true, false, false, false, TreeTraversalMode.Preorder))
            {
                Element_Removed(cm, element);
            }
        }

        e.ToolTipChanged -= Element_ToolTipChanged;
        e.ContextMenuChanged -= Element_ContextMenuChanged;
        e.OnNameChanged -= Element_NameChanged;
    }

    private void Element_NameChanged(object sender, EventArgs<string> e)
    {
        if (e.PreviousValue != null)
            ElementsByName.Remove(e.PreviousValue);
        if (e.NewValue != null)
            ElementsByName.Add(e.NewValue, sender as MGElement);
    }

    private void Element_ToolTipChanged(object sender, EventArgs<MGToolTip> e) => Element_NestedElementChanged(e.PreviousValue, e.NewValue);
    private void Element_ContextMenuChanged(object sender, EventArgs<MGContextMenu> e) => Element_NestedElementChanged(e.PreviousValue, e.NewValue);

    private void Element_NestedElementChanged(MGSingleContentHost previous, MGSingleContentHost @new)
    {
        if (previous != null)
        {
            previous.OnDirectOrNestedContentAdded -= Element_Added;
            previous.OnDirectOrNestedContentRemoved -= Element_Removed;

            foreach (var element in previous.TraverseVisualTree(true, false, false, false, TreeTraversalMode.Preorder))
            {
                Element_Removed(previous, element);
            }
        }

        if (@new != null)
        {
            @new.OnDirectOrNestedContentAdded += Element_Added;
            @new.OnDirectOrNestedContentRemoved += Element_Removed;

            foreach (var element in @new.TraverseVisualTree(true, false, false, false, TreeTraversalMode.Preorder))
            {
                Element_Added(@new, element);
            }
        }
    }
    #endregion Indexed Elements

    private VisualStateFillBrush _previousBackgroundBrush = null;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private WindowStyle _windowStyle = WindowStyle.Default;
    public WindowStyle WindowStyle
    {
        get => _windowStyle;
        set
        {
            if (WindowStyle != value)
            {
                _windowStyle = value;

                switch (_windowStyle)
                {
                    case WindowStyle.Default:
                        IsTitleBarVisible = true;
                        IsCloseButtonVisible = true;
                        IsUserResizable = true;
                        Padding = DefaultWindowPadding;
                        BorderThickness = DefaultWindowBorderThickness;
                        BackgroundBrush = _previousBackgroundBrush ?? BackgroundBrush;
                        break;
                    case WindowStyle.None:
                        IsTitleBarVisible = false;
                        IsCloseButtonVisible = false;
                        IsUserResizable = false;
                        Padding = new(0);
                        BorderThickness = new(0);
                        _previousBackgroundBrush = BackgroundBrush.Copy();
                        BackgroundBrush.SetAll(MGSolidFillBrush.Transparent); // Set this to MGSolidFillBrush.White * 0.2f while testing the AllowsClickThrough issue below
                        //this.AllowsClickThrough = true;   //TODO we probably want AllowsClickThrough=false, but to then handle any unhandled events that occurred overtop of this window's content.
                        //That way, an invisible window with margin around the content (such as horizontally-centered content) won't auto-handle clicks within the
                        //window that are outside the content.
                        //For Example, make an invisible window at topleft=0,0, size=500,500
                        //Add content with size=200,200, centered in the window
                        //clicking at position=100,100 overlaps the window, but doesn't overlap the content of the window
                        //so the click should fall-through to whatever's under the window
                        break;
                    default: throw new NotImplementedException($"Unrecognized {nameof(WindowStyle)}: {value}");
                }

                Npc(nameof(WindowStyle));
            }
        }
    }

    public void Draw(DrawBaseArgs ba) => Draw(new ElementDrawArgs(ba, VisualState, Point.Zero));

    public event EventHandler<ElementDrawArgs> OnBeginDrawNestedWindows;
    public event EventHandler<ElementDrawArgs> OnEndDrawNestedWindows;

    private IEnumerable<MgWindow> ParentWindows
    {
        get
        {
            var current = ParentWindow;
            while (current != null)
            {
                yield return current;
                current = current.ParentWindow;
            }
        }
    }

    public override void Draw(ElementDrawArgs da)
    {
        if (!IsWindowScaled && !ParentWindows.Any(x => x.IsWindowScaled))
            base.Draw(da);
        else
        {
#if true
            using (da.DT.SetTransformTemporary(UnscaledScreenSpaceToScaledScreenSpace))
            {
                base.Draw(da);
            }
#else
                using (DA.DT.SetRenderTargetTemporary(RenderTarget, Color.Transparent))
                {
                    ElementDrawArgs Translated = DA with { Offset = DA.Offset - TopLeft };
                    base.Draw(Translated);
                }

                Rectangle LayoutSpaceBounds = new(Left, Top, WindowWidth, WindowHeight);
                Rectangle Destination = ConvertCoordinateSpace(CoordinateSpace.Layout, CoordinateSpace.Screen, LayoutSpaceBounds);
                DA.DT.DrawTextureTo(RenderTarget, null, Destination);
#endif
        }

        OnBeginDrawNestedWindows?.Invoke(this, da);

        //  Draw a transparent black overlay if there is a Modal window overtop of this window
        if (ModalWindow != null)
            da.DT.FillRectangle(da.Offset.ToVector2(), LayoutBounds, Color.Black * 0.5f);

        foreach (var nested in _nestedWindows.OrderBy(x => x.IsTopmost))
            nested.Draw(da);
        ModalWindow?.Draw(da);

        if (!_isDrawingDraggedWindowPreview && _isDraggingWindowPosition && _dragWindowPositionOffset.HasValue && _dragWindowPositionOffset.Value != Point.Zero)
        {
            try
            {
                _isDrawingDraggedWindowPreview = true;
                var tempOpacity = da.Opacity * 0.25f;
                var tempOffset = da.Offset + _dragWindowPositionOffset.Value;
                Draw(da.SetOpacity(tempOpacity) with { Offset = tempOffset });
            }
            finally { _isDrawingDraggedWindowPreview = false; }
        }

        OnEndDrawNestedWindows?.Invoke(this, da);
    }
}