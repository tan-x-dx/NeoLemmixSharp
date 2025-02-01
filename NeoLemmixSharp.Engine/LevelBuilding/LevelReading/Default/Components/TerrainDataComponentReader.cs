﻿using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Components;

public sealed class TerrainDataComponentReader : ILevelDataReader, IComparer<TerrainArchetypeData>
{
    private readonly List<string> _stringIdLookup;

    public bool AlreadyUsed { get; private set; }
    public ReadOnlySpan<byte> GetSectionIdentifier() => LevelReadWriteHelpers.TerrainDataSectionIdentifier;

    public TerrainDataComponentReader(
        Version version,
        List<string> stringIdLookup)
    {
        _stringIdLookup = stringIdLookup;
    }

    public void ReadSection(RawFileData rawFileData, LevelData levelData)
    {
        AlreadyUsed = true;
        int numberOfItemsInSection = rawFileData.Read16BitUnsignedInteger();

        while (numberOfItemsInSection-- > 0)
        {
            var newTerrainDatum = ReadNextTerrainData(rawFileData);
            levelData.AllTerrainData.Add(newTerrainDatum);
        }
    }

    private TerrainData ReadNextTerrainData(RawFileData rawFileData)
    {
        int numberOfBytesToRead = rawFileData.Read8BitUnsignedInteger();
        int initialBytesRead = rawFileData.BytesRead;

        int styleId = rawFileData.Read16BitUnsignedInteger();
        int pieceId = rawFileData.Read16BitUnsignedInteger();

        int x = rawFileData.Read16BitUnsignedInteger();
        int y = rawFileData.Read16BitUnsignedInteger();

        byte orientationByte = rawFileData.Read8BitUnsignedInteger();
        var (orientation, facingDirection) = LevelReadWriteHelpers.DecipherOrientationByte(orientationByte);

        byte terrainDataMiscByte = rawFileData.Read8BitUnsignedInteger();
        var decipheredTerrainDataMisc = LevelReadWriteHelpers.DecipherTerrainDataMiscByte(terrainDataMiscByte);

        Color? tintColor = null;
        if (decipheredTerrainDataMisc.HasTintSpecified)
        {
            tintColor = ReadTerrainDataTintColor(rawFileData);
        }

        int? width = null;
        if (decipheredTerrainDataMisc.HasWidthSpecified)
        {
            width = ReadTerrainDataDimension(rawFileData);
        }

        int? height = null;
        if (decipheredTerrainDataMisc.HasHeightSpecified)
        {
            height = ReadTerrainDataDimension(rawFileData);
        }

        AssertTerrainDataBytesMakeSense(
            rawFileData.BytesRead,
            initialBytesRead,
            numberOfBytesToRead);

        return new TerrainData
        {
            GroupName = null,
            Style = _stringIdLookup[styleId],
            TerrainPiece = _stringIdLookup[pieceId],

            X = x - LevelReadWriteHelpers.PositionOffset,
            Y = y - LevelReadWriteHelpers.PositionOffset,

            NoOverwrite = decipheredTerrainDataMisc.NoOverwrite,
            RotNum = orientation.RotNum,
            Flip = facingDirection == FacingDirection.Left,
            Erase = decipheredTerrainDataMisc.Erase,

            Tint = tintColor,

            Width = width,
            Height = height,
        };
    }

    private static Color ReadTerrainDataTintColor(RawFileData rawFileData)
    {
        var byteBuffer = rawFileData.ReadBytes(3);

        return new Color(r: byteBuffer[0], g: byteBuffer[1], b: byteBuffer[2], alpha: (byte)0xff);
    }

    private static int ReadTerrainDataDimension(RawFileData rawFileData)
    {
        return rawFileData.Read16BitUnsignedInteger();
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