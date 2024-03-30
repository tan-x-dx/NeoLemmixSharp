using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using MGUI.Shared.Helpers;

#if UseWPF
using System.Xaml;
#else
using Portable.Xaml;
#endif

namespace MGUI.Core.UI.XAML;

public class XamlParser
{
    private const string XmlNameSpaceBaseUri = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";

    private static readonly char[] NewLineChars = ['\n', '\r'];

    public const string XmlLocalNameSpacePrefix = "MGUI";
    public static readonly string XmlLocalNameSpaceUri = $"clr-namespace:{nameof(MGUI)}.{nameof(Core)}.{nameof(UI)}.{nameof(XAML)};assembly={nameof(MGUI)}.{nameof(Core)}";

    private static readonly string XmlNameSpaces =
        $"xmlns=\"{XmlNameSpaceBaseUri}\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:{XmlLocalNameSpacePrefix}=\"{XmlLocalNameSpaceUri}\"";
    //$"xmlns=\"{XMLLocalNameSpaceUri}\" xmlns:x=\"{XMLNameSpaceBaseUri}\""; // This URI avoids using a prefix for the MGUI namespace

    private static readonly Dictionary<string, string> ElementNameAliases = new()
    {
        { "ContentPresenter", nameof(ContentPresenter) },
        { "HeaderedContentPresenter", nameof(HeaderedContentPresenter) },

        { "Border", nameof(Border) },
        { "Button", nameof(Button) },
        { "CheckBox", nameof(CheckBox) },
        { "ComboBox", nameof(ComboBox) },

        { "ContextMenu", nameof(ContextMenu) },
        { "ContextMenuButton", nameof(ContextMenuButton) },
        { "ContextMenuToggle", nameof(ContextMenuToggle) },
        { "ContextMenuSeparator", nameof(ContextMenuSeparator) },

        { "Expander", nameof(Expander) },
        { "GroupBox", nameof(GroupBox) },
        { "Image", nameof(Image) },
        { "ListBox", nameof(ListBox) },
        { "ListView", nameof(ListView) },
        { "ListViewColumn", nameof(ListViewColumn) },

        { "OverlayHost", nameof(OverlayHost) },
        { "Overlay", nameof(Overlay) },

        { "PasswordBox", nameof(PasswordBox) },
        { "ProgressBar", nameof(ProgressBar) },
        { "RadioButton", nameof(RadioButton) },
        { "RatingControl", nameof(RatingControl) },
        { "Rectangle", nameof(Rectangle) },
        { "ResizeGrip", nameof(ResizeGrip) },
        { "ScrollViewer", nameof(ScrollViewer) },
        { "Separator", nameof(Separator) },
        { "Slider", nameof(Slider) },
        { "Spacer", nameof(Spacer) },
        { "Spoiler", nameof(Spoiler) },
        { "Stopwatch", nameof(Stopwatch) },
        { "TabControl", nameof(TabControl) },
        { "TabItem", nameof(TabItem) },
        { "TextBlock", nameof(TextBlock) },
        { "TextBox", nameof(TextBox) },
        { "Timer", nameof(Timer) },
        { "ToggleButton", nameof(ToggleButton) },
        { "ToolTip", nameof(ToolTip) },
        { "Window", nameof(Window) },

        { "RowDefinition", nameof(RowDefinition) },
        { "ColumnDefinition", nameof(ColumnDefinition) },
        { "GridSplitter", nameof(GridSplitter) },
        { "Grid", nameof(Grid) },
        { "UniformGrid", nameof(UniformGrid) },
        { "DockPanel", nameof(DockPanel) },
        { "StackPanel", nameof(StackPanel) },
        { "OverlayPanel", nameof(OverlayPanel) },

        { "Style", nameof(Style) },
        { "Setter", nameof(Setter) },

        //  Abbreviated names
        { "CP", nameof(ContentPresenter) },
        { "HCP", nameof(HeaderedContentPresenter) },
        { "CM", nameof(ContextMenu) },
        { "CMB", nameof(ContextMenuButton) },
        { "CMT", nameof(ContextMenuToggle) },
        { "CMS", nameof(ContextMenuSeparator) },
        { "GB", nameof(GroupBox) },
        { "LB", nameof(ListBox) },
        { "LV", nameof(ListView) },
        { "LVC", nameof(ListViewColumn) },
        { "RB", nameof(RadioButton) },
        { "RC", nameof(RatingControl) },
        { "RG", nameof(ResizeGrip) },
        { "SV", nameof(ScrollViewer) },
        { "TC", nameof(TabControl) },
        { "TI", nameof(TabItem) },
        { "TB", nameof(TextBlock) },
        { "TT", nameof(ToolTip) },
        { "RD", nameof(RowDefinition) },
        { "CD", nameof(ColumnDefinition) },
        { "GS", nameof(GridSplitter) },
        { "UG", nameof(UniformGrid) },
        { "DP", nameof(DockPanel) },
        { "SP", nameof(StackPanel) },
        { "OP", nameof(OverlayPanel) }
    };

    private static string ValidateXamlString(string xamlString)
    {
        xamlString = xamlString.Trim();

        //  TODO: First line might not be an XElement. Could be something like a comment,
        //  so we should use more robust logic to insert the namespaces

        var firstLineBreakIndex = xamlString.IndexOfAny(NewLineChars);
        var firstLine = firstLineBreakIndex < 0 ? xamlString : xamlString[..firstLineBreakIndex];
        if (!firstLine.Contains(XmlNameSpaces))
        {
            //  Insert the required xml namespaces into the XAML string
            var spaceIndex = xamlString.IndexOf(' ');
            if (spaceIndex >= 0)
                xamlString = $"{xamlString[..spaceIndex]} {XmlNameSpaces} {xamlString[(spaceIndex + 1)..]}";
            else
            {
                var insertionIndex = xamlString.IndexOf('>');
                xamlString = $"{xamlString[..insertionIndex]} {XmlNameSpaces} {xamlString[insertionIndex..]}";
            }
        }

        //  Replace all element names with their fully-qualified name, such as:
        //  "<Button/>"                 --> "<MGUI:Button/>"
        //  "<Button Content="Foo" />"  --> "<MGUI:Button Content="Foo" />"
        //  "<Button.Content>"          --> "<MGUI:Button.Content>"
        //  Where the "MGUI" XML namespace prefix refers to the XMLLocalNameSpaceUri static string
        var document = XDocument.Parse(xamlString);

        foreach (var element in document.Descendants())
        {
            var elementName = element.Name.LocalName;
            foreach (var kvp in ElementNameAliases)
            {
                if (elementName.StartsWith(kvp.Key))
                {
                    element.Name = XName.Get(elementName.ReplaceFirstOccurrence(kvp.Key, kvp.Value), XmlLocalNameSpaceUri);
                    break;
                }
            }
        }

        using StringWriter sw = new();
        using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
        {
            document.WriteTo(xw);
        }

        var result = sw.ToString();
        return result;
    }

    /// <param name="sanitizeXamlString">If true, the given <paramref name="xamlString"/> will be pre-processed via the following logic:<para/>
    /// 1. Trim leading and trailing whitespace<br/>
    /// 2. Insert required XML namespaces (such as "xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation")<br/>
    /// 3. Replace type names with their fully-qualified names, such as "Button" -> "MGUI:Button" where the "MGUI" namespace prefix points to the URI defined by <see cref="XmlLocalNameSpaceUri"/><para/>
    /// If your XAML already contains fully-qualified types, you probably should set this to false.</param>
    /// <param name="replaceLinebreakLiterals">If true, the literal string @"\n" will be replaced with "&#38;#x0a;", which is the XAML encoding of the linebreak character '\n'.<br/>
    /// If false, setting the text of an <see cref="MGTextBlock"/> requires encoding the '\n' character as "&#38;#x0a;"<para/>
    /// See also: <see href="https://stackoverflow.com/a/183435/11689514"/></param>
    public static T Load<T>(MgWindow window, string xamlString, bool sanitizeXamlString = false, bool replaceLinebreakLiterals = true)
        where T : MGElement
    {
        if (sanitizeXamlString)
            xamlString = ValidateXamlString(xamlString);
        if (replaceLinebreakLiterals)
            xamlString = xamlString.Replace(@"\n", "&#x0a;");
        var parsed = (Element)XamlServices.Parse(xamlString);
        parsed.ProcessStyles(window.GetResources());
        return parsed.ToElement<T>(window, null);
    }

    /// <param name="sanitizeXamlString">If true, the given <paramref name="xamlString"/> will be pre-processed via the following logic:<para/>
    /// 1. Trim leading and trailing whitespace<br/>
    /// 2. Insert required XML namespaces (such as "xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation")<br/>
    /// 3. Replace type names with their fully-qualified names, such as "Button" -> "MGUI:Button" where the "MGUI" namespace prefix points to the URI defined by <see cref="XmlLocalNameSpaceUri"/><para/>
    /// If your XAML already contains fully-qualified types, you probably should set this to false.</param>
    /// <param name="replaceLinebreakLiterals">If true, the literal string @"\n" will be replaced with "&#38;#x0a;", which is the XAML encoding of the linebreak character '\n'.<br/>
    /// If false, setting the text of an <see cref="MGTextBlock"/> requires encoding the '\n' character as "&#38;#x0a;"<para/>
    /// See also: <see href="https://stackoverflow.com/a/183435/11689514"/></param>
    public static MgWindow LoadRootWindow(MGDesktop desktop, string xamlString, bool sanitizeXamlString = false, bool replaceLinebreakLiterals = true)
    {
        if (sanitizeXamlString)
            xamlString = ValidateXamlString(xamlString);
        if (replaceLinebreakLiterals)
            xamlString = xamlString.Replace(@"\n", "&#x0a;");
        var parsed = (Window)XamlServices.Parse(xamlString);
        parsed.ProcessStyles(desktop.Resources);
        return parsed.ToElement(desktop);
    }
}