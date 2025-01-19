using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class TerrainDataComponentReader : ILevelDataReader, IComparer<TerrainArchetypeData>
{
    private readonly Dictionary<int, TerrainArchetypeData> _terrainArchetypeDataLookup = new();
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainDataSectionIdentifier;

    public TerrainDataComponentReader(List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(BinaryReaderWrapper reader, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = reader.Read16BitUnsignedInteger();

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainDatum = ReadTerrainData(reader);
            levelData.AllTerrainData.Add(newTerrainDatum);
        }

        ProcessTerrainArchetypeData();

        levelData.TerrainArchetypeData.AddRange(_terrainArchetypeDataLookup.Values);
        levelData.TerrainArchetypeData.Sort(this);
    }

    private TerrainData ReadTerrainData(BinaryReaderWrapper reader)
    {
        int numberOfBytesToRead = reader.Read8BitUnsignedInteger();
        int initialBytesRead = reader.BytesRead;

        int styleId = reader.Read16BitUnsignedInteger();
        int pieceId = reader.Read16BitUnsignedInteger();

        var terrainArchetypeData = GetOrAddTerrainArchetypeData(styleId, pieceId);

        int x = reader.Read16BitUnsignedInteger();
        int y = reader.Read16BitUnsignedInteger();

        byte orientationByte = reader.Read8BitUnsignedInteger();
        var (orientation, facingDirection) = LevelReadWriteHelpers.DecipherOrientationByte(orientationByte);

        byte terrainDataMiscByte = reader.Read8BitUnsignedInteger();
        var decipheredTerrainDataMisc = LevelReadWriteHelpers.DecipherTerrainDataMiscByte(terrainDataMiscByte);

        Color? tintColor = null;
        if (decipheredTerrainDataMisc.HasTintSpecified)
        {
            tintColor = ReadTerrainDataTintColor(reader);
        }

        int? width = null;
        if (decipheredTerrainDataMisc.HasWidthSpecified)
        {
            width = ReadTerrainDataDimension(reader);
        }

        int? height = null;
        if (decipheredTerrainDataMisc.HasHeightSpecified)
        {
            height = ReadTerrainDataDimension(reader);
        }

        AssertTerrainDataBytesMakeSense(
            reader.BytesRead,
            initialBytesRead,
            numberOfBytesToRead);

        return new TerrainData
        {
            TerrainArchetypeId = terrainArchetypeData.TerrainArchetypeId,

            X = x - LevelReadWriteHelpers.PositionOffset,
            Y = y - LevelReadWriteHelpers.PositionOffset,

            NoOverwrite = decipheredTerrainDataMisc.NoOverwrite,
            RotNum = orientation.RotNum,
            Flip = facingDirection == FacingDirection.LeftInstance,
            Erase = decipheredTerrainDataMisc.Erase,

            Tint = tintColor,

            GroupName = null,

            Width = width,
            Height = height,
        };
    }

    private static Color ReadTerrainDataTintColor(BinaryReaderWrapper reader)
    {
        var byteBuffer = reader.ReadBytes(3);

        return new Color(r: byteBuffer[0], g: byteBuffer[1], b: byteBuffer[2], alpha: (byte)0xff);
    }

    private static int ReadTerrainDataDimension(BinaryReaderWrapper reader)
    {
        return reader.Read16BitUnsignedInteger();
    }

    private TerrainArchetypeData GetOrAddTerrainArchetypeData(int styleId, int pieceId)
    {
        var terrainArchetypeDataLookupKey = (styleId << 16) | pieceId;

        ref var terrainArchetypeData = ref CollectionsMarshal.GetValueRefOrAddDefault(_terrainArchetypeDataLookup, terrainArchetypeDataLookupKey, out var exists);

        if (exists)
            return terrainArchetypeData!;

        terrainArchetypeData = new TerrainArchetypeData
        {
            TerrainArchetypeId = _terrainArchetypeDataLookup.Count - 1,

            Style = _stringIdLookup[styleId],
            TerrainPiece = _stringIdLookup[pieceId]
        };

        return terrainArchetypeData;
    }

    private void ProcessTerrainArchetypeData()
    {
        throw new NotImplementedException();
    }

    int IComparer<TerrainArchetypeData>.Compare(TerrainArchetypeData? x, TerrainArchetypeData? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (y is null) return 1;
        if (x is null) return -1;
        return x.TerrainArchetypeId.CompareTo(y.TerrainArchetypeId);
    }

    private static void AssertTerrainDataBytesMakeSense(
        int bytesRead,
        int initialBytesRead,
        int numberOfBytesToRead)
    {
        if (bytesRead - initialBytesRead == numberOfBytesToRead)
            return;

        throw new LevelReadingException(
            "Wrong number of bytes read for terrain data! " +
            $"Expected: {numberOfBytesToRead}, Actual: {bytesRead - initialBytesRead}");
    }
}