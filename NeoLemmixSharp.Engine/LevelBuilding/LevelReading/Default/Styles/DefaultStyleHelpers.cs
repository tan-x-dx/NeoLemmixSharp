using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles;

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
        var uniqueDataStyles = GetUniqueStyles(
            levelData,
            out var numberOfUniqueTerrainArchetypes,
            out var numberOfUniqueGadgetArchetypes);

        levelData.TerrainArchetypeData.EnsureCapacity(numberOfUniqueTerrainArchetypes);
        levelData.AllGadgetArchetypeBuilders.EnsureCapacity(numberOfUniqueGadgetArchetypes);

        foreach (var styleGroup in uniqueDataStyles)
        {
            ProcessStyleData(levelData, styleGroup);
        }
    }

    private static HashSetLookup<string, PieceAndTypePair> GetUniqueStyles(
        LevelData levelData,
        out int numberOfUniqueTerrainArchetypes,
        out int numberOfUniqueGadgetArchetypes)
    {
        var uniqueStyles = new HashSetLookup<string, PieceAndTypePair>(valueComparer: new PieceAndTypePairComparer());

        var count = 0;
        foreach (var terrainData in levelData.AllTerrainData)
        {
            if (uniqueStyles.Add(terrainData.Style, new PieceAndTypePair(terrainData.TerrainPiece, StylePieceType.Terrain)))
            {
                count++;
            }
        }

        numberOfUniqueTerrainArchetypes = count;
        count = 0;

        foreach (var gadgetData in levelData.AllGadgetData)
        {
            if (uniqueStyles.Add(gadgetData.Style, new PieceAndTypePair(gadgetData.GadgetPiece, StylePieceType.Gadget)))
            {
                count++;
            }
        }

        numberOfUniqueGadgetArchetypes = count;

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
            // Add one for the type identifier byte at the end
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
                utf8ByteBuffer[..requiredByteBufferSize],
                pair.PieceType);
        }
    }

    private static void ProcessPiece(
        LevelData levelData,
        RawFileData rawFileData,
        string styleName,
        string pieceName,
        ReadOnlySpan<byte> pieceByteSpan,
        StylePieceType pieceType)
    {
        var pieceExists = rawFileData.TryLocateSpan(pieceByteSpan, out var index);

        rawFileData.SetReaderPosition(index + pieceByteSpan.Length);

        switch (pieceType)
        {
            case StylePieceType.Terrain:

                var newTerrainArchetypeData = TerrainArchetypeReadingHelpers.GetTerrainArchetypeData(styleName, pieceName, rawFileData, pieceExists);

                levelData.TerrainArchetypeData.Add(
                    new LevelData.StylePiecePair(styleName, pieceName),
                    newTerrainArchetypeData);
                break;

            case StylePieceType.Gadget:
                var newGadgetArchetypeBuilder = GadgetBuilderReadingHelpers.GetGadgetBuilderData(styleName, pieceName, rawFileData, pieceExists);

                levelData.AllGadgetArchetypeBuilders.Add(
                    new LevelData.StylePiecePair(styleName, pieceName),
                    newGadgetArchetypeBuilder);
                break;

            default:
                throw new LevelReadingException($"Unknown piece type byte: {pieceType:X}");
        }
    }
}
