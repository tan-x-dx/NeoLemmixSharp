﻿using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Levels.Sections.Version1_0_0_0;

internal sealed class TerrainGroupDataSectionReader : LevelDataSectionReader
{
    private readonly StringIdLookup _stringIdLookup;
    private readonly TerrainDataSectionReader _terrainDataComponentReader;

    public TerrainGroupDataSectionReader(
        StringIdLookup stringIdLookup,
        TerrainDataSectionReader terrainDataComponentReader)
        : base(LevelFileSectionIdentifier.TerrainGroupDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
        _terrainDataComponentReader = terrainDataComponentReader;
    }


    public override void ReadSection(RawLevelFileDataReader rawFileData, LevelData levelData, int numberOfItemsInSection)
    {
        throw new NotImplementedException();
    }
}
