using NeoLemmixSharp.IO.FileFormats;
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
            [new FileFormatVersion(1, 0, 0, 0)] = new Writing.Levels.Sections.Version1_0_0_0.VersionHelper()
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper> GetLevelReaderLookup()
    {
        var result = new Dictionary<FileFormatVersion, ILevelDataSectionReaderVersionHelper>(1)
        {
            [new FileFormatVersion(1, 0, 0, 0)] = new Reading.Levels.Sections.Version1_0_0_0.VersionHelper()
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper> GetStyleWriterLookup()
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionWriterVersionHelper>(1)
        {
            [new FileFormatVersion(1, 0, 0, 0)] = new Writing.Styles.Sections.Version1_0_0_0.VersionHelper()
        };

        return result;
    }

    private static Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper> GetStyleReaderLookup()
    {
        var result = new Dictionary<FileFormatVersion, IStyleDataSectionReaderVersionHelper>(1)
        {
            [new FileFormatVersion(1, 0, 0, 0)] = new Reading.Styles.Sections.Version1_0_0_0.VersionHelper()
        };

        return result;
    }

    public static ReadOnlySpan<LevelDataSectionWriter> GetLevelDataSectionWritersForVersion(FileFormatVersion version)
    {
        if (!_levelWriterVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownFileFormatVersionException(version);

        var result = helper.GetLevelDataSectionWriters();
        Array.Sort(result);
        return result;
    }

    public static ReadOnlySpan<LevelDataSectionReader> GetLevelDataSectionReadersForVersion(FileFormatVersion version)
    {
        if (!_levelReaderVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownFileFormatVersionException(version);

        var result = helper.GetLevelDataSectionReaders();
        Array.Sort(result);
        return result;
    }

    public static ReadOnlySpan<StyleDataSectionWriter> GetStyleDataSectionWritersForVersion(FileFormatVersion version)
    {
        if (!_styleWriterVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownFileFormatVersionException(version);

        var result = helper.GetStyleDataSectionWriters();
        Array.Sort(result);
        return result;
    }

    public static ReadOnlySpan<StyleDataSectionReader> GetStyleDataSectionReadersForVersion(FileFormatVersion version)
    {
        if (!_styleReaderVersionHelpers.TryGetValue(version, out var helper))
            ThrowUnknownFileFormatVersionException(version);

        var result = helper.GetStyleDataSectionReaders();
        Array.Sort(result);
        return result;
    }

    [DoesNotReturn]
    private static void ThrowUnknownFileFormatVersionException(FileFormatVersion version)
    {
        var message = $"Unknown version number: {version}";

        throw new UnknownFileFormatVersionException(version, message);
    }

    private sealed class UnknownFileFormatVersionException(FileFormatVersion version, string message) : Exception(message)
    {
        public FileFormatVersion FileVersion { get; } = version;
    }
}
