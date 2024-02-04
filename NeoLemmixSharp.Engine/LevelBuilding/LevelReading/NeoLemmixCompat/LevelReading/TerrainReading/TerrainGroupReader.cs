using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.TerrainReading;

public sealed class TerrainGroupReader : INeoLemmixDataReader
{
    private readonly Dictionary<string, TerrainArchetypeData> _terrainArchetypes;
    private readonly List<TerrainGroup> _allTerrainGroups = new();

    private TerrainReader? _terrainReader;
    private TerrainGroup? _currentTerrainGroup;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAINGROUP";

    public TerrainGroupReader(Dictionary<string, TerrainArchetypeData> terrainArchetypes)
    {
        _terrainArchetypes = terrainArchetypes;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainGroup = new TerrainGroup();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        if (_terrainReader != null)
        {
            var result = _terrainReader.ReadNextLine(line);

            if (_terrainReader.FinishedReading)
            {
                _terrainReader.Dispose();
                _terrainReader = null;
            }

            return result;
        }

        ReadingHelpers.GetTokenPair(line, out var firstToken, out _, out var secondTokenIndex);

        var currentTerrainGroup = _currentTerrainGroup!;

        switch (firstToken)
        {
            case "NAME":
                currentTerrainGroup.GroupName = line.TrimAfterIndex(secondTokenIndex).GetString();
                break;

            case "$TERRAIN":
                _terrainReader = new TerrainReader(_terrainArchetypes, currentTerrainGroup.TerrainDatas);
                _terrainReader.BeginReading(firstToken);
                break;

            case "$END":
                _allTerrainGroups.Add(currentTerrainGroup);
                _currentTerrainGroup = null;
                FinishedReading = true;
                break;
        }

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.TerrainArchetypeData.EnsureCapacity(_terrainArchetypes.Count);
        levelData.TerrainArchetypeData.AddRange(_terrainArchetypes.Values.OrderBy(d => d.TerrainArchetypeId));

        levelData.AllTerrainGroups.AddRange(_allTerrainGroups);
    }

    public void Dispose()
    {
        _terrainArchetypes.Clear();
        _allTerrainGroups.Clear();
    }
}