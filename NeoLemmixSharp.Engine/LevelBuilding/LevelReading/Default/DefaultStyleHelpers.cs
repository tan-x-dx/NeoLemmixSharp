using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public static class DefaultStyleHelpers
{
    private readonly struct PieceAndTypePair(string pieceName, StylePieceType pieceType)
    {
        public readonly string PieceName = pieceName;
        public readonly StylePieceType PieceType = pieceType;
    }

    private sealed class PieceAndTypePairComparer : IEqualityComparer<PieceAndTypePair>
    {
        public bool Equals(PieceAndTypePair x, PieceAndTypePair y)
        {
            return x.PieceType == y.PieceType &&
                   string.Equals(x.PieceName, y.PieceName);
        }

        public int GetHashCode([DisallowNull] PieceAndTypePair obj)
        {
            return HashCode.Combine(obj.PieceName, obj.PieceType);
        }
    }

    public static void ProcessStyleArchetypeData(
        LevelData levelData)
    {
        var uniqueDataStyles = GetUniqueStyles(levelData);

        var numberOfUniqueTerrainArchetypes = 0;
        var numberOfUniqueGadgetArchetypes = 0;
        foreach (var group in uniqueDataStyles)
        {
            foreach (var pair in group)
            {
                var pieceType = pair.PieceType;
                if (pieceType == StylePieceType.Terrain)
                {
                    numberOfUniqueTerrainArchetypes++;
                }
                else
                {
                    numberOfUniqueGadgetArchetypes++;
                }
            }
        }

        levelData.TerrainArchetypeData.EnsureCapacity(numberOfUniqueTerrainArchetypes);
        levelData.AllGadgetArchetypeBuilders.EnsureCapacity(numberOfUniqueGadgetArchetypes);

        foreach (var styleGroup in uniqueDataStyles)
        {
            ProcessStyleData(levelData, styleGroup);
        }
    }

    private static HashSetLookup<string, PieceAndTypePair> GetUniqueStyles(
        LevelData levelData)
    {
        var uniqueStyles = new HashSetLookup<string, PieceAndTypePair>(valueComparer: new PieceAndTypePairComparer());

        foreach (var terrainData in levelData.AllTerrainData)
        {
            uniqueStyles.Add(terrainData.Style, new PieceAndTypePair(terrainData.TerrainPiece, StylePieceType.Terrain));
        }

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            uniqueStyles.Add(gadgetData.Style, new PieceAndTypePair(gadgetData.GadgetPiece, StylePieceType.Gadget));
        }

        if (uniqueStyles.KeyCount == 0)
            throw new LevelReadingException("No styles specified");

        return uniqueStyles;
    }

    private static void ProcessStyleData(
        LevelData levelData,
        HashSetLookup<string, PieceAndTypePair>.Group styleGroup)
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

    [SkipLocalsInit]
    private static void ReadLevelDataFromStyle(
        LevelData levelData,
        RawFileData rawFileData,
        HashSetLookup<string, PieceAndTypePair>.Group styleGroup)
    {
        Span<byte> utf8ByteBuffer = stackalloc byte[256];
        var utf8Encoding = Encoding.UTF8;

        foreach (var pair in styleGroup)
        {
            var requiredByteBufferSize = 1 + utf8Encoding.GetByteCount(pair.PieceName);

            if (utf8ByteBuffer.Length < requiredByteBufferSize)
            {
                utf8ByteBuffer = new byte[requiredByteBufferSize];
            }

            utf8Encoding.GetBytes(pair.PieceName, utf8ByteBuffer);
            utf8ByteBuffer[requiredByteBufferSize - 1] = (byte)pair.PieceType;

            ProcessPiece(
                levelData,
                rawFileData,
                styleGroup.Key,
                pair.PieceName,
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
            // This will ensure all terrain pieces receive archetype data

            TerrainArchetypeReadingHelpers.CreateTrivialTerrainArchetypeData(levelData, styleName, pieceName);

            return;
        }

        rawFileData.SetReaderPosition(index + pieceByteSpan.Length);

        var pieceType = (StylePieceType)rawFileData.Read8BitUnsignedInteger();

        switch (pieceType)
        {
            case StylePieceType.Terrain:
                TerrainArchetypeReadingHelpers.ReadTerrainArchetypeData(levelData, styleName, pieceName, rawFileData);
                break;

            case StylePieceType.Gadget:
                GadgetBuilderReadingHelpers.ReadGadgetBuilderData(levelData, styleName, pieceName, rawFileData);
                break;

            default:
                throw new LevelReadingException($"Unknown piece type byte: {pieceType:X}");
        }
    }
}
