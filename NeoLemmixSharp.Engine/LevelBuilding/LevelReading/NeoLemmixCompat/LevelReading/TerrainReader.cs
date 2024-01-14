using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class TerrainReader : INeoLemmixDataReader
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

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentTerrainData = new TerrainData();
        FinishedReading = false;
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        switch (firstToken)
        {
            case "STYLE":
                if (secondToken[0] == '*')
                {
                    _settingDataForGroup = true;
                }
                else
                {
                    var rest = line[(1 + firstTokenIndex + firstToken.Length)..];
                    _currentTerrainData!.Style = rest.ToString();
                }

                break;

            case "PIECE":
                if (_settingDataForGroup)
                {
                    _currentTerrainData!.GroupId = secondToken.ToString();
                }
                else
                {
                    var rest = line[(1 + firstTokenIndex + firstToken.Length)..];
                    _currentTerrainData!.TerrainName = rest.ToString();
                }
                break;

            case "X":
                _currentTerrainData!.X = ReadingHelpers.ReadInt(secondToken);
                break;

            case "Y":
                _currentTerrainData!.Y = ReadingHelpers.ReadInt(secondToken);
                break;

            case "RGB":
                _currentTerrainData!.Tint = ReadingHelpers.ReadUint(secondToken, true);
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
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
        }
    }
}