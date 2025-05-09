using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;
using NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Versions;

public static class VersionHelper
{
    private static readonly Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper> _levelWriterVersionHelpers = GetLevelWriterLookup();
    private static readonly Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> _levelReaderVersionHelpers = GetLevelReaderLookup();
    private static readonly Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> _styleWriterVersionHelpers = GetStyleWriterLookup();
    private static readonly Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> _styleReaderVersionHelpers = GetStyleReaderLookup();

    private static Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper> GetLevelWriterLookup()
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper>()
        {
            { new FileFormatVersion(1,0,0,0), new Writing.Levels.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> GetLevelReaderLookup()
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper>()
        {
            { new FileFormatVersion(1,0,0,0), new Reading.Levels.Default.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> GetStyleWriterLookup()
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper>()
        {
            { new FileFormatVersion(1,0,0,0), new Writing.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> GetStyleReaderLookup()
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper>()
        {
            { new FileFormatVersion(1,0,0,0), new Reading.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    public static LevelDataSectionWriter[] GetLevelDataSectionWritersForVersion(FileFormatVersion version)
    {
        if (_levelWriterVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetLevelDataSectionWriters();

        return ThrowUnknownVersionException<LevelDataSectionWriter>(version);
    }

    public static LevelDataSectionReader[] GetLevelDataSectionReadersForVersion(FileFormatVersion version)
    {
        if (_levelReaderVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetLevelDataSectionReaders();

        return ThrowUnknownVersionException<LevelDataSectionReader>(version);
    }

    public static StyleDataSectionWriter[] GetStyleDataSectionWritersForVersion(FileFormatVersion version)
    {
        if (_styleWriterVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetStyleDataSectionWriters();

        return ThrowUnknownVersionException<StyleDataSectionWriter>(version);
    }

    public static StyleDataSectionReader[] GetStyleDataSectionReadersForVersion(FileFormatVersion version)
    {
        if (_styleReaderVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetStyleDataSectionReaders();

        return ThrowUnknownVersionException<StyleDataSectionReader>(version);
    }

    private sealed class UnknownVersionException : Exception
    {
        public FileFormatVersion FileVersion { get; }

        public UnknownVersionException(FileFormatVersion version)
            : base(GetExceptionMessage(version))
        {
            FileVersion = version;
        }

        private static string GetExceptionMessage(FileFormatVersion version)
        {
            return $"Unknown version number: {version}";
        }
    }

    [DoesNotReturn]
    private static T[] ThrowUnknownVersionException<T>(FileFormatVersion version) => throw new UnknownVersionException(version);
}

public interface ILevelDataSectionWriterVersionHelper
{
    LevelDataSectionWriter[] GetLevelDataSectionWriters();
}

public interface ILevelDataSectionReaderVersionHelper
{
    LevelDataSectionReader[] GetLevelDataSectionReaders();
}

public interface IStyleDataSectionWriterVersionHelper
{
    StyleDataSectionWriter[] GetStyleDataSectionWriters();
}

public interface IStyleDataSectionReaderVersionHelper
{
    StyleDataSectionReader[] GetStyleDataSectionReaders();
}
