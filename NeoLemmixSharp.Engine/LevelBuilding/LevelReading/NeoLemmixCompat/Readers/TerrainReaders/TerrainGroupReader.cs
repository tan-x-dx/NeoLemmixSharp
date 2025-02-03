using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainGroupReader : NeoLemmixDataReader
{
    private readonly UniqueStringSet _uniqueStringSet;

    private TerrainReader? _terrainReader;
    private TerrainGroupData? _currentTerrainGroup;

    public List<TerrainGroupData> AllTerrainGroups { get; } = new();

    public TerrainGroupReader(
        UniqueStringSet uniqueStringSet)
        : base("$TERRAINGROUP")
    {
        _uniqueStringSet = uniqueStringSet;

        RegisterTokenAction("NAME", SetName);
        RegisterTokenAction("$TERRAIN", ReadTerrainData);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainGroup = new TerrainGroupData();
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        if (_terrainReader != null)
        {
            var result = _terrainReader.ReadNextLine(line);

            if (_terrainReader.FinishedReading)
            {
                _terrainReader = null;
            }

            return result;
        }

        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        var alternateLookup = _tokenActions.GetAlternateLookup<ReadOnlySpan<char>>();

        if (alternateLookup.TryGetValue(firstToken, out var tokenAction))
        {
            tokenAction(line, secondToken, secondTokenIndex);
        }

        return false;
    }

    private void SetName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentTerrainGroup!.GroupName = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
    }

    private void ReadTerrainData(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _terrainReader = new TerrainReader(_uniqueStringSet, _currentTerrainGroup!.AllBasicTerrainData);
        _terrainReader.BeginReading("$TERRAIN");
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        AllTerrainGroups.Add(_currentTerrainGroup!);
        _currentTerrainGroup = null;
        FinishedReading = true;
    }
}