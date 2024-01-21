using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class GadgetReader : INeoLemmixDataReader
{
    private readonly List<NeoLemmixGadgetData> _allGadgetData = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";
    private NeoLemmixGadgetData? _currentGadgetData;

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentGadgetData = new NeoLemmixGadgetData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);
        var rest = secondToken.IsEmpty
            ? ReadOnlySpan<char>.Empty
            : line[(1 + firstTokenIndex + firstToken.Length)..];

        switch (firstToken)
        {
            case "STYLE":
                _currentGadgetData!.Style = rest.GetString();
                break;

            case "PIECE":
                _currentGadgetData!.Piece = rest.GetString();
                break;

            case "X":
                _currentGadgetData!.X = int.Parse(secondToken);
                break;

            case "Y":
                _currentGadgetData!.Y = int.Parse(secondToken);
                break;

            case "WIDTH":
                _currentGadgetData!.Width = int.Parse(secondToken);
                break;

            case "HEIGHT":
                _currentGadgetData!.Height = int.Parse(secondToken);
                break;

            case "SPEED":
                _currentGadgetData!.Speed = int.Parse(secondToken);
                break;

            case "ANGLE":
                _currentGadgetData!.Angle = 0;
                break;

            case "NO_OVERWRITE":
                _currentGadgetData!.NoOverwrite = true;
                break;

            case "ONLY_ON_TERRAIN":
                _currentGadgetData!.OnlyOnTerrain = true;
                break;

            case "ONE_WAY":
                break;

            case "FLIP_VERTICAL":
                _currentGadgetData!.FlipVertical = true;
                break;

            case "FLIP_HORIZONTAL":
                _currentGadgetData!.FlipHorizontal = true;
                break;

            case "ROTATE":
                _currentGadgetData!.Rotate = true;
                break;

            case "SKILL":
                _currentGadgetData!.Skill = rest.GetString();
                break;

            case "SKILL_COUNT":
                _currentGadgetData!.SkillCount = int.Parse(secondToken);
                break;

            case "$END":
                _allGadgetData.Add(_currentGadgetData!);
                _currentGadgetData = null;
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
        }

        return false;
    }

    public void ApplyToLevelData(LevelData levelData)
    {
        //    levelData.AllGadgetData.AddRange(_allGadgetData);
    }

    public void Dispose()
    {
        _allGadgetData.Clear();
    }
}