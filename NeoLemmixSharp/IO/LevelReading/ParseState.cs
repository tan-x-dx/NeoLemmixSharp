using System;

namespace NeoLemmixSharp.IO.LevelReading;

[Flags]
public enum ParseState
{
    General = 1 << 0,
    ParseTerrain = 1 << 1,
    ParseTerrainGroup = 1 << 2,
    ParseObject = 1 << 3
}