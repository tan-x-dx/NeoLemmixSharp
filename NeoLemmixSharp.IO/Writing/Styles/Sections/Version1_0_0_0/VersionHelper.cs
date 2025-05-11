using NeoLemmixSharp.IO.Versions;
using NeoLemmixSharp.IO.Writing.Styles.Sections;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class VersionHelper : IStyleDataSectionWriterVersionHelper
{
    public StyleDataSectionWriter[] GetStyleDataSectionWriters()
    {
        return [];
    }
}
