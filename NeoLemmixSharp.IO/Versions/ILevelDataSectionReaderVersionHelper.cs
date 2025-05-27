using NeoLemmixSharp.IO.Reading.Levels.Sections;

namespace NeoLemmixSharp.IO.Versions;

internal interface ILevelDataSectionReaderVersionHelper
{
    LevelDataSectionReader[] GetLevelDataSectionReaders();
}
