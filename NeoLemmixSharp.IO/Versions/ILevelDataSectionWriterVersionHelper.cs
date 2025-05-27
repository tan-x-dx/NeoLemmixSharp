using NeoLemmixSharp.IO.Writing.Levels.Sections;

namespace NeoLemmixSharp.IO.Versions;

internal interface ILevelDataSectionWriterVersionHelper
{
    LevelDataSectionWriter[] GetLevelDataSectionWriters();
}
