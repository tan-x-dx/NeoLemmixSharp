using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public static class StyleHelpers
{
    public static void ProcessTerrainArchetypeData(
        LevelData levelData)
    {
        var uniqueTerrainDataStyles = CommonHelpers.GetUniqueStyles(levelData, StylePieceType.Terrain);
        levelData.TerrainArchetypeData.EnsureCapacity(uniqueTerrainDataStyles.ValueCount);

        foreach (var styleGroup in uniqueTerrainDataStyles)
        {
            ProcessTerrainStyleData(levelData, styleGroup);
        }
    }

    public static void ProcessGadgetArchetypeData(
        LevelData levelData)
    {
        var uniqueStyles = CommonHelpers.GetUniqueStyles(levelData, StylePieceType.Gadget);

        foreach (var styleGroup in uniqueStyles)
        {
            ProcessGadgetStyleData(levelData, styleGroup);
        }
    }

    private static void ProcessTerrainStyleData(
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

        ReadLevelDataFromStyle(levelData, rawFileData, styleGroup, StylePieceType.Terrain);
    }

    private static void ProcessGadgetStyleData(
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

        ReadLevelDataFromStyle(levelData, rawFileData, styleGroup, StylePieceType.Gadget);
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

    [SkipLocalsInit]
    private static void ReadLevelDataFromStyle(
        LevelData levelData,
        RawFileData rawFileData,
        HashSetLookup<string, string>.Group styleGroup,
        StylePieceType typeIdentifier)
    {
        Span<byte> utf8ByteBuffer = stackalloc byte[256];
        var utf8Encoding = Encoding.UTF8;

        foreach (var pieceName in styleGroup)
        {
            var requiredByteBufferSize = 1 + utf8Encoding.GetByteCount(pieceName);

            if (utf8ByteBuffer.Length < requiredByteBufferSize)
            {
                utf8ByteBuffer = new byte[requiredByteBufferSize];
            }

            utf8Encoding.GetBytes(pieceName, utf8ByteBuffer);
            utf8ByteBuffer[requiredByteBufferSize - 1] = (byte)typeIdentifier;

            ProcessPiece(
                levelData,
                rawFileData,
                styleGroup.Key,
                pieceName,
                utf8ByteBuffer[..requiredByteBufferSize],
                typeIdentifier);
        }
    }

    private static void ProcessPiece(
        LevelData levelData,
        RawFileData rawFileData,
        string styleName,
        string pieceName,
        ReadOnlySpan<byte> pieceByteSpan,
        StylePieceType typeIdentifier)
    {
        var pieceExists = rawFileData.TryLocateSpan(pieceByteSpan, out var index);

        if (!pieceExists)
        {
            // If the piece cannot be located within the style file,
            // assume it's a boring terrain piece, and treat it as such.
            // This will ensure all terrain pieces receive archetype data

            TerrainArchetypeReadingHelpers.CreateTrivialTerrainArchetypeData(levelData, styleName, pieceName);

            return;
        }

        rawFileData.SetReaderPosition(index + pieceByteSpan.Length);

        var pieceType = (StylePieceType)rawFileData.Read8BitUnsignedInteger();
        Debug.Assert(pieceType == typeIdentifier);

        switch (pieceType)
        {
            case StylePieceType.Terrain:
                TerrainArchetypeReadingHelpers.ReadTerrainArchetypeData(levelData, rawFileData);
                break;

            case StylePieceType.Gadget:
                GadgetBuilderReadingHelpers.ReadGadgetBuilderData(levelData, rawFileData);
                break;

            default:
                throw new LevelReadingException($"Unknown piece type byte: {pieceType:X}");
        }
    }
}
