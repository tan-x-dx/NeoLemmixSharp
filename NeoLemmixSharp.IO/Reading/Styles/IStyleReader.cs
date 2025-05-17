using NeoLemmixSharp.IO.Data.Style;

namespace NeoLemmixSharp.IO.Reading.Styles;

internal interface IStyleReader : IDisposable
{
    StyleData ReadStyle();
}
