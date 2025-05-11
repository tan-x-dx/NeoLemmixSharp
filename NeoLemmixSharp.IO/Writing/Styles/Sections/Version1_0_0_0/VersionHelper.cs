using NeoLemmixSharp.IO.Versions;
using NeoLemmixSharp.IO.Writing.Styles.Sections;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

public sealed class VersionHelper : IStyleDataSectionWriterVersionHelper
{
    public StyleDataSectionWriter[] GetStyleDataSectionWriters()
    {
        return [];
    }
}
