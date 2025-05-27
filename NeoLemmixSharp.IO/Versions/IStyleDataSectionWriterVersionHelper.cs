using NeoLemmixSharp.IO.Writing.Styles.Sections;

namespace NeoLemmixSharp.IO.Versions;

internal interface IStyleDataSectionWriterVersionHelper
{
    StyleDataSectionWriter[] GetStyleDataSectionWriters();
}
