using NeoLemmixSharp.IO.Reading.Levels.Sections;
using NeoLemmixSharp.IO.Reading.Styles.Sections;
using NeoLemmixSharp.IO.Writing.Levels.Sections;
using NeoLemmixSharp.IO.Writing.Styles.Sections;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Versions;

internal static class VersionHelper
{
    private static readonly Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper> _levelWriterVersionHelpers = GetLevelWriterLookup();
    private static readonly Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> _levelReaderVersionHelpers = GetLevelReaderLookup();
    private static readonly Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> _styleWriterVersionHelpers = GetStyleWriterLookup();
    private static readonly Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> _styleReaderVersionHelpers = GetStyleReaderLookup();

    private static Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper> GetLevelWriterLookup()
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper>(1)
        {
            { new FileFormatVersion(1,0,0,0), new Writing.Levels.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> GetLevelReaderLookup()
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper>(1)
        {
            { new FileFormatVersion(1,0,0,0), new Reading.Levels.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> GetStyleWriterLookup()
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper>(1)
        {
            { new FileFormatVersion(1,0,0,0), new Writing.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> GetStyleReaderLookup()
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper>(1)
        {
            { new FileFormatVersion(1,0,0,0), new Reading.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    public static LevelDataSectionWriter[] GetLevelDataSectionWritersForVersion(FileFormatVersion version)
    {
        if (!_levelWriterVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownVersionException(version);

        return helper.GetLevelDataSectionWriters();
    }

    public static LevelDataSectionReader[] GetLevelDataSectionReadersForVersion(FileFormatVersion version)
    {
        if (!_levelReaderVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownVersionException(version);

        return helper.GetLevelDataSectionReaders();
    }

    public static StyleDataSectionWriter[] GetStyleDataSectionWritersForVersion(FileFormatVersion version)
    {
        if (!_styleWriterVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownVersionException(version);

        return helper.GetStyleDataSectionWriters();
    }

    public static StyleDataSectionReader[] GetStyleDataSectionReadersForVersion(FileFormatVersion version)
    {
        if (!_styleReaderVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownVersionException(version);

        return helper.GetStyleDataSectionReaders();
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
    private static void ThrowUnknownVersionException(FileFormatVersion version) => throw new UnknownVersionException(version);
}

internal interface ILevelDataSectionWriterVersionHelper
{
    LevelDataSectionWriter[] GetLevelDataSectionWriters();
}

internal interface ILevelDataSectionReaderVersionHelper
{
    LevelDataSectionReader[] GetLevelDataSectionReaders();
}

internal interface IStyleDataSectionWriterVersionHelper
{
    StyleDataSectionWriter[] GetStyleDataSectionWriters();
}

internal interface IStyleDataSectionReaderVersionHelper
{
    StyleDataSectionReader[] GetStyleDataSectionReaders();
}
