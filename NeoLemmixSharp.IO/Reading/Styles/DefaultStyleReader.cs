using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Reading.Styles.Sections;
using NeoLemmixSharp.IO.Versions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.IO.Reading.Styles;

internal readonly ref struct DefaultStyleReader : IStyleReader<DefaultStyleReader>
{
    private readonly StyleIdentifier _styleIdentifier;
    private readonly RawStyleFileDataReader _reader;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DefaultStyleReader Create(StyleIdentifier styleIdentifier) => new(styleIdentifier);

    private DefaultStyleReader(StyleIdentifier style)
    {
        if (!TryLocateStyleFile(style, out var styleFilePath))
            throw new FileReadingException($"Could not locate style file for style: {style}");

        _styleIdentifier = style;
        _reader = GetRawStyleFileDataReader(styleFilePath);
    }

    private static bool TryLocateStyleFile(
        StyleIdentifier style,
        [MaybeNullWhen(false)] out string foundFilePath)
    {
        var styleFolderPath = style.GetFolderFilePath();

        var allStyleFiles = RootDirectoryManager.GetFilePathsWithExtension(styleFolderPath, DefaultFileExtensions.StyleFileExtension);

        if (allStyleFiles.Length == 1)
        {
            foundFilePath = allStyleFiles[0];
            return true;
        }

        foundFilePath = null;
        return false;
    }

    private static RawStyleFileDataReader GetRawStyleFileDataReader(string styleFilePath)
    {
        using var fileStream = new FileStream(styleFilePath, FileMode.Open);
        return new RawStyleFileDataReader(fileStream);
    }

    public StyleData ReadStyle()
    {
        var sectionReaders = VersionHelper.GetStyleDataSectionReadersForVersion(_reader.FileFormatVersion);
        var result = new StyleData(_styleIdentifier, FileFormatType.Default);

        foreach (var sectionReader in sectionReaders)
        {
            ReadSection(result, sectionReader);
        }

        FileReadingException.ReaderAssert(!_reader.MoreToRead, "Finished reading but extra bytes still in file!");

        return result;
    }

    private void ReadSection(
        StyleData result,
        StyleDataSectionReader sectionReader)
    {
        if (!_reader.TryGetSectionInterval(sectionReader.SectionIdentifier, out var interval))
        {
            FileReadingException.ReaderAssert(
                !sectionReader.IsNecessary,
                "No data for necessary section!");
            return;
        }

        _reader.SetReaderPosition(interval.Start);

        ushort sectionIdentifierBytes = _reader.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(
            sectionIdentifierBytes == sectionReader.GetSectionIdentifier(),
            "Section Identifier mismatch!");

        int numberOfItemsInSection = _reader.Read16BitUnsignedInteger();

        sectionReader.ReadSection(_reader, result, numberOfItemsInSection);

        FileReadingException.ReaderAssert(
            interval.End == _reader.Position,
            "Byte reading mismatch!");
    }

    public void Dispose()
    {
        _reader?.Dispose();
    }
}
