using NeoLemmixSharp.Engine.LevelBuilding.Data.Terrain;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.TerrainReaders;

public sealed class TerrainGroupReader : INeoLemmixDataReader
{
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;

    private TerrainReader? _terrainReader;
    private TerrainGroupData? _currentTerrainGroup;

    public List<TerrainGroupData> AllTerrainGroups { get; } = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAINGROUP";

    public TerrainGroupReader(Dictionary<string, TerrainArchetypeData> terrainArchetypes)
    {
        _terrainArchetypes = terrainArchetypes;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainGroup = new TerrainGroupData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
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

        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out _, out var secondTokenIndex);

        var currentTerrainGroup = _currentTerrainGroup!;

        switch (firstToken)
        {
            case "NAME":
                currentTerrainGroup.GroupName = line.TrimAfterIndex(secondTokenIndex).ToString();
                break;

            case "$TERRAIN":
                _terrainReader = new TerrainReader(_terrainArchetypes, currentTerrainGroup.AllBasicTerrainData);
                _terrainReader.BeginReading(firstToken);
                break;

            case "$END":
                AllTerrainGroups.Add(currentTerrainGroup);
                _currentTerrainGroup = null;
                FinishedReading = true;
                break;
        }

        return false;
    }
}