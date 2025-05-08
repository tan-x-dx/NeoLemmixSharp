using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.Default.Sections;
using NeoLemmixSharp.Engine.LevelIo.Reading.Styles.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Levels.Sections;
using NeoLemmixSharp.Engine.LevelIo.Writing.Styles.Sections;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Versions;

public sealed class VersionHelper : IEqualityComparer<Version>
{
    public static readonly VersionHelper Instance = new();

    private readonly Dictionary<Version, ILevelDataSectionWriterVersionHelper> _levelWriterVersionHelpers;
    private readonly Dictionary<Version, ILevelDataSectionReaderVersionHelper> _levelReaderVersionHelpers;
    private readonly Dictionary<Version, IStyleDataSectionWriterVersionHelper> _styleWriterVersionHelpers;
    private readonly Dictionary<Version, IStyleDataSectionReaderVersionHelper> _styleReaderVersionHelpers;

    private VersionHelper()
    {
        _levelWriterVersionHelpers = GetLevelWriterLookup();
        _levelReaderVersionHelpers = GetLevelReaderLookup();
        _styleWriterVersionHelpers = GetStyleWriterLookup();
        _styleReaderVersionHelpers = GetStyleReaderLookup();
    }

    private Dictionary<Version, ILevelDataSectionWriterVersionHelper> GetLevelWriterLookup()
    {
        var result = new Dictionary<Version, ILevelDataSectionWriterVersionHelper>(this)
        {
            { new Version(1,0,0,0), new Writing.Levels.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private Dictionary<Version, ILevelDataSectionReaderVersionHelper> GetLevelReaderLookup()
    {
        var result = new Dictionary<Version, ILevelDataSectionReaderVersionHelper>(this)
        {
            { new Version(1,0,0,0), new Reading.Levels.Default.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private Dictionary<Version, IStyleDataSectionWriterVersionHelper> GetStyleWriterLookup()
    {
        var result = new Dictionary<Version, IStyleDataSectionWriterVersionHelper>(this)
        {
            { new Version(1,0,0,0), new Writing.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    private Dictionary<Version, IStyleDataSectionReaderVersionHelper> GetStyleReaderLookup()
    {
        var result = new Dictionary<Version, IStyleDataSectionReaderVersionHelper>(this)
        {
            { new Version(1,0,0,0), new Reading.Styles.Sections.Version1_0_0_0.VersionHelper() }
        };

        return result;
    }

    public LevelDataSectionWriter[] GetLevelDataSectionWritersForVersion(Version version)
    {
        if (_levelWriterVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetLevelDataSectionWriters();

        return ThrowUnknownVersionException<LevelDataSectionWriter>(version);
    }

    public LevelDataSectionReader[] GetLevelDataSectionReadersForVersion(Version version)
    {
        if (_levelReaderVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetLevelDataSectionReaders();

        return ThrowUnknownVersionException<LevelDataSectionReader>(version);
    }

    public StyleDataSectionWriter[] GetStyleDataSectionWritersForVersion(Version version)
    {
        if (_styleWriterVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetStyleDataSectionWriters();

        return ThrowUnknownVersionException<StyleDataSectionWriter>(version);
    }

    public StyleDataSectionReader[] GetStyleDataSectionReadersForVersion(Version version)
    {
        if (_styleReaderVersionHelpers.TryGetValue(version, out var helper))
            return helper.GetStyleDataSectionReaders();

        return ThrowUnknownVersionException<StyleDataSectionReader>(version);
    }

    private sealed class UnknownVersionException : Exception
    {
        public Version Version { get; }

        public UnknownVersionException(Version version)
            : base(GetExceptionMessage(version))
        {
            Version = version;
        }

        private static string GetExceptionMessage(Version version)
        {
            return $"Unknown version number: {version}";
        }
    }

    [DoesNotReturn]
    private static T[] ThrowUnknownVersionException<T>(Version version) => throw new UnknownVersionException(version);

    bool IEqualityComparer<Version>.Equals(Version? x, Version? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null) return false;
        if (y is null) return false;

        return x.Major == y.Major &&
               x.Minor == y.Minor &&
               x.Build == y.Build &&
               x.Revision == y.Revision;
    }

    int IEqualityComparer<Version>.GetHashCode(Version obj)
    {
        return 1410733 * obj.Major +
               3992171 * obj.Minor +
               5658031 * obj.Build +
               7851007 * obj.Revision +
               9472289;
    }
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
