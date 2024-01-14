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

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainGroup = new TerrainGroup();
        FinishedReading = false;
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        if (_terrainReader != null)
        {
            _terrainReader.ReadNextLine(line);

            if (_terrainReader.FinishedReading)
            {
                _terrainReader = null;
            }

            return;
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
    }
}