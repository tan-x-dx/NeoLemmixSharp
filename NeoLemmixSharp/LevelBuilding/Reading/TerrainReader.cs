using NeoLemmixSharp.LevelBuilding.Data;
using System;
using System.Collections.Generic;

namespace NeoLemmixSharp.LevelBuilding.Reading;

public sealed class TerrainReader : IDataReader
{
    private readonly ICollection<TerrainData> _allTerrainData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAIN";

    private TerrainData? _currentTerrainData;
    private bool _settingDataForGroup;

    public TerrainReader(ICollection<TerrainData> allTerrainData)
    {
        _allTerrainData = allTerrainData;
    }

    public void BeginReading(string[] tokens)
    {
        _currentTerrainData = new TerrainData(_allTerrainData.Count);
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "STYLE":
                if (tokens[1][0] == '*')
                {
                    _settingDataForGroup = true;
                }
                else
                {
                    _currentTerrainData!.Style = ReadingHelpers.ReadFormattedString(tokens[1..]);
                }

                break;

            case "PIECE":
                if (_settingDataForGroup)
                {
                    _currentTerrainData!.GroupId = tokens[1];
                }
                else
                {
                    _currentTerrainData!.TerrainName = ReadingHelpers.ReadFormattedString(tokens[1..]);
                }
                break;

            case "X":
                _currentTerrainData!.X = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "Y":
                _currentTerrainData!.Y = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "RGB":
                _currentTerrainData!.Tint = ReadingHelpers.ReadUint(tokens[1], true);
                break;

            case "NO_OVERWRITE":
                _currentTerrainData!.NoOverwrite = true;
                break;

            case "ONE_WAY":
                break;

            case "FLIP_VERTICAL":
                _currentTerrainData!.FlipVertical = true;
                break;

            case "FLIP_HORIZONTAL":
                _currentTerrainData!.FlipHorizontal = true;
                break;

            case "ROTATE":
                _currentTerrainData!.Rotate = true;
                break;

            case "ERASE":
                _currentTerrainData!.Erase = true;
                break;

            case "$END":
                _allTerrainData.Add(_currentTerrainData!);
                _currentTerrainData = null;
                _settingDataForGroup = false;
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}