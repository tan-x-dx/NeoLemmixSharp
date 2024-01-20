using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class TerrainGroupReader : INeoLemmixDataReader
{
    private readonly List<TerrainGroup> _allTerrainGroups = new();

    private TerrainReader? _terrainReader;
    private TerrainGroup? _currentTerrainGroup;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAINGROUP";

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

        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var rest = line[(1 + firstTokenIndex + firstToken.Length)..];

        switch (firstToken)
        {
            case "NAME":
                _currentTerrainGroup!.GroupId = rest.ToString();
                break;

            case "$TERRAIN":
                _terrainReader = new TerrainReader(_currentTerrainGroup!.TerrainDatas);
                _terrainReader.BeginReading(line);
                break;

            case "$END":
                _allTerrainGroups.Add(_currentTerrainGroup!);
                _currentTerrainGroup = null;
                FinishedReading = true;
                break;
        }

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        levelData.AllTerrainGroups.AddRange(_allTerrainGroups);
    }

    public void Dispose()
    {
        foreach (var terrainGroup in _allTerrainGroups)
        {
            terrainGroup.Dispose();
        }

        _allTerrainGroups.Clear();
    }
}