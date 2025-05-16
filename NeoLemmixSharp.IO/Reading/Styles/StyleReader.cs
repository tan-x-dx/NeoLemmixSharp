using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Reading.Styles.Sections;
using NeoLemmixSharp.IO.Versions;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Reading.Styles;

internal readonly ref struct StyleReader
{
    private readonly StyleIdentifier _styleIdentifier;
    private readonly RawStyleFileDataReader _rawFileData;

    internal StyleReader(StyleIdentifier style)
    {
        if (!TryLocateStyleFile(style, out var styleFilePath))
            throw new FileReadingException($"Could not locate style file for style: {style}");

        _styleIdentifier = style;
        _rawFileData = GetRawStyleFileDataReader(styleFilePath);
    }

    private static bool TryLocateStyleFile(
        StyleIdentifier style,
        [MaybeNullWhen(false)] out string foundFilePath)
    {
        var styleFolderPath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            style.ToString());

        var files = Directory.GetFiles(styleFolderPath);

        foreach (var file in files)
        {
            var fileExtension = Path.GetExtension(file.AsSpan());

            if (fileExtension.Equals(DefaultFileExtensions.LevelStyleExtension, StringComparison.OrdinalIgnoreCase))
            {
                foundFilePath = file;
                return true;
            }
        }

        foundFilePath = null;
        return false;
    }

    private static RawStyleFileDataReader GetRawStyleFileDataReader(string styleFilePath)
    {
        using var fileStream = new FileStream(styleFilePath, FileMode.Open);
        return new RawStyleFileDataReader(fileStream);
    }

    internal StyleData LoadStyle()
    {
        var sectionReaders = VersionHelper.GetStyleDataSectionReadersForVersion(_rawFileData.Version);
        var result = new StyleData(_styleIdentifier);

        foreach (var sectionReader in sectionReaders)
        {
            ReadSection(result, sectionReader);
        }

        return result;
    }

    private void ReadSection(
        StyleData result,
        StyleDataSectionReader sectionReader)
    {
        var sectionIdentifier = sectionReader.SectionIdentifier;

        if (!_rawFileData.TryGetSectionInterval(sectionIdentifier, out var interval))
        {
            FileReadingException.ReaderAssert(
                !sectionReader.IsNecessary,
                "No data for necessary section!");
            return;
        }

        _rawFileData.SetReaderPosition(interval.Start);

        var sectionIdentifierBytes = _rawFileData.ReadBytes(StyleFileSectionIdentifierHasher.NumberOfBytesForStyleSectionIdentifier);

        FileReadingException.ReaderAssert(
            sectionIdentifierBytes.SequenceEqual(sectionReader.GetSectionIdentifierBytes()),
            "Section Identifier mismatch!");

        sectionReader.ReadSection(_rawFileData, result);

        FileReadingException.ReaderAssert(
            interval.Start + interval.Length == _rawFileData.Position,
            "Byte reading mismatch!");
    }
}
