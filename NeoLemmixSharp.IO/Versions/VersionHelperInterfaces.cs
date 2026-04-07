using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Reading.Levels.Sections;
using NeoLemmixSharp.IO.Reading.Styles.Sections;
using NeoLemmixSharp.IO.Writing.Levels.Sections;
using NeoLemmixSharp.IO.Writing.Styles.Sections;

namespace NeoLemmixSharp.IO.Versions;

internal interface IVersionHelper
{
    FileFormatVersion FileFormatVersion { get; }
}

internal interface ILevelDataSectionReaderVersionHelper : IVersionHelper
{
    LevelDataSectionReader[] GetLevelDataSectionReaders();
}

internal interface IStyleDataSectionWriterVersionHelper : IVersionHelper
{
    StyleDataSectionWriter[] GetStyleDataSectionWriters();
}

internal interface ILevelDataSectionWriterVersionHelper : IVersionHelper
{
    LevelDataSectionWriter[] GetLevelDataSectionWriters();
}

internal interface IStyleDataSectionReaderVersionHelper : IVersionHelper
{
    StyleDataSectionReader[] GetStyleDataSectionReaders();
}
