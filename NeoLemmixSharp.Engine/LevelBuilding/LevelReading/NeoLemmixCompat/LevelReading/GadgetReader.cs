using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class GadgetReader : INeoLemmixDataReader
{
    private readonly ICollection<NeoLemmixGadgetData> _allGadgetData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";
    private NeoLemmixGadgetData? _currentGadgetData;

    public GadgetReader(ICollection<NeoLemmixGadgetData> allGadgetData)
    {
        _allGadgetData = allGadgetData;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentGadgetData = new NeoLemmixGadgetData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);

        switch (firstToken)
        {
            case "STYLE":
                var rest = line[(1 + firstTokenIndex + firstToken.Length)..];
                _currentGadgetData!.Style = rest.ToString();
                break;

            case "PIECE":
                var rest1 = line[(1 + firstTokenIndex + firstToken.Length)..];
                _currentGadgetData!.Piece = rest1.ToString();
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
                var rest2 = line[(1 + firstTokenIndex + firstToken.Length)..];
                _currentGadgetData!.Skill = rest2.ToString();
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