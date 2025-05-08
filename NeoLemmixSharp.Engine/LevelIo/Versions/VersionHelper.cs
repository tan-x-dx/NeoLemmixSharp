using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;
using NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Versions;

public sealed class VersionHelper
{
    public static readonly VersionHelper Instance = new();

    private readonly Dictionary<FileVersion, ILevelDataSectionWriterVersionHelper> _levelWriterVersionHelpers;
    private readonly Dictionary<FileVersion, ILevelDataSectionReaderVersionHelper> _levelReaderVersionHelpers;
    private readonly Dictionary<FileVersion, IStyleDataSectionWriterVersionHelper> _styleWriterVersionHelpers;
    private readonly Dictionary<FileVersion, IStyleDataSectionReaderVersionHelper> _styleReaderVersionHelpers;

    private VersionHelper()
    {
        _levelWriterVersionHelpers = GetLevelWriterLookup();
        _levelReaderVersionHelpers = GetLevelReaderLookup();
        _styleWriterVersionHelpers = GetStyleWriterLookup();
        _styleReaderVersionHelpers = GetStyleReaderLookup();
    }

    private static Dictionary<FileVersion, ILevelDataSectionWriterVersionHelper> GetLevelWriterLookup()
    {
        var result = new Dictionary<FileVersion, ILevelDataSectionWriterVersionHelper>()
        {
            { new FileVersion(1,0,0,0), new Writing.Levels.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileVersion, ILevelDataSectionReaderVersionHelper> GetLevelReaderLookup()
    {
        var result = new Dictionary<FileVersion, ILevelDataSectionReaderVersionHelper>()
        {
            { new FileVersion(1,0,0,0), new Reading.Levels.Default.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileVersion, IStyleDataSectionWriterVersionHelper> GetStyleWriterLookup()
    {
        var result = new Dictionary<FileVersion, IStyleDataSectionWriterVersionHelper>()
        {
            { new FileVersion(1,0,0,0), new Writing.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileVersion, IStyleDataSectionReaderVersionHelper> GetStyleReaderLookup()
    {
        var result = new Dictionary<FileVersion, IStyleDataSectionReaderVersionHelper>()
        {
            { new FileVersion(1,0,0,0), new Reading.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    public LevelDataSectionWriter[] GetLevelDataSectionWritersForVersion(FileVersion version)
    {
        if (_levelWriterVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetLevelDataSectionWriters();

        return ThrowUnknownVersionException<LevelDataSectionWriter>(version);
    }

    public LevelDataSectionReader[] GetLevelDataSectionReadersForVersion(FileVersion version)
    {
        if (_levelReaderVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetLevelDataSectionReaders();

        return ThrowUnknownVersionException<LevelDataSectionReader>(version);
    }

    public StyleDataSectionWriter[] GetStyleDataSectionWritersForVersion(FileVersion version)
    {
        if (_styleWriterVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetStyleDataSectionWriters();

        return ThrowUnknownVersionException<StyleDataSectionWriter>(version);
    }

    public StyleDataSectionReader[] GetStyleDataSectionReadersForVersion(FileVersion version)
    {
        if (_styleReaderVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetStyleDataSectionReaders();

        return ThrowUnknownVersionException<StyleDataSectionReader>(version);
    }

    private sealed class UnknownVersionException : Exception
    {
        public FileVersion FileVersion { get; }

        public UnknownVersionException(FileVersion version)
            : base(GetExceptionMessage(version))
        {
            FileVersion = version;
        }

        private static string GetExceptionMessage(FileVersion version)
        {
            return $"Unknown version number: {version}";
        }
    }

    [DoesNotReturn]
    private static T[] ThrowUnknownVersionException<T>(FileVersion version) => throw new UnknownVersionException(version);
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
