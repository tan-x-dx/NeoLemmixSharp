using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;
using NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Versions;

public static class VersionHelper
{
    private static readonly Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper> _levelWriterVersionHelpers;
    private static readonly Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> _levelReaderVersionHelpers;
    private static readonly Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> _styleWriterVersionHelpers;
    private static readonly Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> _styleReaderVersionHelpers;

    static VersionHelper()
    {
        var equalityComparer = new FileFormatVersionEqualityComparer();

        _levelWriterVersionHelpers = GetLevelWriterLookup(equalityComparer);
        _levelReaderVersionHelpers = GetLevelReaderLookup(equalityComparer);
        _styleWriterVersionHelpers = GetStyleWriterLookup(equalityComparer);
        _styleReaderVersionHelpers = GetStyleReaderLookup(equalityComparer);
    }

    private static Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper> GetLevelWriterLookup(FileFormatVersionEqualityComparer equalityComparer)
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionWriterVersionHelper>(1, equalityComparer)
        {
            { new FileFormatVersion(1,0,0,0), new Writing.Levels.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> GetLevelReaderLookup(FileFormatVersionEqualityComparer equalityComparer)
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper>(1, equalityComparer)
        {
            { new FileFormatVersion(1,0,0,0), new Reading.Levels.Default.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> GetStyleWriterLookup(FileFormatVersionEqualityComparer equalityComparer)
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper>(1, equalityComparer)
        {
            { new FileFormatVersion(1,0,0,0), new Writing.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> GetStyleReaderLookup(FileFormatVersionEqualityComparer equalityComparer)
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper>(1, equalityComparer)
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

    private sealed class FileFormatVersionEqualityComparer : IEqualityComparer<FileFormatVersion>
    {
        public bool Equals(FileFormatVersion x, FileFormatVersion y) => x.Equals(y);
        public int GetHashCode([DisallowNull] FileFormatVersion obj) => obj.GetHashCode();
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
