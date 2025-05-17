using NeoLemmixSharp.IO.Data.Style;

namespace NeoLemmixSharp.IO.Reading.Styles;

internal interface IStyleReader<TReaderType> : IDisposable
    where TReaderType : IStyleReader<TReaderType>, allows ref struct
{
    static abstract TReaderType Create(StyleIdentifier styleIdentifier);

    StyleData ReadStyle();
}
