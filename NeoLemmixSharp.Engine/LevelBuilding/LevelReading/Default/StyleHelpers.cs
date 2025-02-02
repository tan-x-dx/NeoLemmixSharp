using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public static class StyleHelpers
{
    public static void ProcessStyleArchetypeData(
        LevelData levelData)
    {
        var uniqueStyles = GetUniqueStyles(levelData);

        foreach (var styleGroup in uniqueStyles)
        {
            ProcessStyleData(levelData, styleGroup);
        }
    }

    private static HashSetLookup<string, string> GetUniqueStyles(LevelData levelData)
    {
        var uniqueStyles = new HashSetLookup<string, string>();

        foreach (var terrainData in levelData.AllTerrainData)
        {
            uniqueStyles.Add(terrainData.Style, terrainData.TerrainPiece);
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            uniqueStyles.Add(gadgetData.Style, gadgetData.GadgetPiece);
        }

        if (uniqueStyles.Count == 0)
            throw new LevelReadingException("No styles specified");

        return uniqueStyles;
    }

    private static void ProcessStyleData(
        LevelData levelData,
        HashSetLookup<string, string>.Group styleGroup)
    {
        var styleFolderPath = Path.Combine(
            RootDirectoryManager.StyleFolderDirectory,
            styleGroup.Key);

        var files = Directory.GetFiles(styleFolderPath);
        if (!TryLocateStyleFile(files, out var styleFilePath))
            throw new LevelReadingException($"Could not locate style file in folder: {styleFolderPath}");

        var rawFileData = new RawFileData(styleFilePath);

        ReadLevelDataFromStyle(levelData, rawFileData, styleGroup);
    }

    private static bool TryLocateStyleFile(
        ReadOnlySpan<string> allFiles,
        [MaybeNullWhen(false)] out string foundFilePath)
    {
        foreach (var file in allFiles)
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

    private static void ReadLevelDataFromStyle(
        LevelData levelData,
        RawFileData rawFileData,
        HashSetLookup<string, string>.Group styleGroup)
    {
        Span<byte> utf8ByteBuffer = stackalloc byte[256];
        var utf8Encoding = Encoding.UTF8;

        foreach (var pieceName in styleGroup)
        {
            var requiredByteBufferSize = utf8Encoding.GetByteCount(pieceName);

            if (utf8ByteBuffer.Length < requiredByteBufferSize)
            {
                utf8ByteBuffer = new byte[requiredByteBufferSize];
            }

            utf8Encoding.GetBytes(pieceName, utf8ByteBuffer);

            ProcessPiece(
                levelData,
                rawFileData,
                styleGroup.Key,
                pieceName,
                utf8ByteBuffer[..requiredByteBufferSize]);
        }
    }

    private static void ProcessPiece(
        LevelData levelData,
        RawFileData rawFileData,
        string styleName,
        string pieceName,
        ReadOnlySpan<byte> pieceByteSpan)
    {
        var pieceExists = rawFileData.TryLocateSpan(pieceByteSpan, out var index);

        if (!pieceExists)
        {
            // If the piece cannot be located within the style file,
            // assume it's a boring terrain piece, and treat it as such.
            // This will ensure all terrain pieces receive archetype
            // data. If it turns out that the piece is supposed to be a
            // gadget, then it's a gadget that we know nothing about,
            // and therefore cannot create a level featuring it. In such
            // a case, an exception will be thrown when trying to build
            // a level with this dud data.

            TerrainArchetypeReadingHelpers.CreateTrivialTerrainArchetypeData(levelData, styleName, pieceName);

            return;
        }

        rawFileData.SetReaderPosition(index + pieceByteSpan.Length);

        byte pieceType = rawFileData.Read8BitUnsignedInteger();

        switch (pieceType)
        {
            case LevelReadWriteHelpers.StylePieceTerrainType:
                TerrainArchetypeReadingHelpers.ReadTerrainArchetypeData(levelData, rawFileData);
                break;

            case LevelReadWriteHelpers.StylePieceGadgetType:
                GadgetBuilderReadingHelpers.ReadGadgetBuilderData(levelData, rawFileData);
                break;

            default:
                throw new LevelReadingException($"Unknown piece type byte: {pieceType:X}");
        }
    }
}
