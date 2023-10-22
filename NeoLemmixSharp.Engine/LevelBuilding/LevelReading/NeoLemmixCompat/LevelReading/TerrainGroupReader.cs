using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class TerrainGroupReader : INeoLemmixDataReader
{
    private readonly ICollection<TerrainGroup> _allTerrainGroups;

    private TerrainReader? _terrainReader;
    private TerrainGroup? _currentTerrainGroup;

    public TerrainGroupReader(ICollection<TerrainGroup> allTerrainGroups)
    {
        _allTerrainGroups = allTerrainGroups;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$TERRAINGROUP";

    public void BeginReading(string[] tokens)
    {
        _currentTerrainGroup = new TerrainGroup();
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        if (_terrainReader != null)
        {
            _terrainReader.ReadNextLine(tokens);

            if (_terrainReader.FinishedReading)
            {
                _terrainReader = null;
            }

            return;
        }

        switch (tokens[0])
        {
            case "NAME":
                _currentTerrainGroup!.GroupId = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "$TERRAIN":
                _terrainReader = new TerrainReader(_currentTerrainGroup!.TerrainDatas);
                _terrainReader.BeginReading(tokens);
                break;

            case "$END":
                _allTerrainGroups.Add(_currentTerrainGroup!);
                _currentTerrainGroup = null;
                FinishedReading = true;
                break;
        }
    }
}