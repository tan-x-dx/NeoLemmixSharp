using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Io.LevelReading.Reading;

public sealed class GadgetReader : IDataReader
{
    private readonly ICollection<GadgetData> _allGadgetData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";
    private GadgetData? _currentGadgetData;

    public GadgetReader(ICollection<GadgetData> allGadgetData)
    {
        _allGadgetData = allGadgetData;
    }

    public void BeginReading(string[] tokens)
    {
        _currentGadgetData = new GadgetData();
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "STYLE":
                _currentGadgetData!.Style = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "PIECE":
                _currentGadgetData!.Piece = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "X":
                _currentGadgetData!.X = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "Y":
                _currentGadgetData!.Y = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "WIDTH":
                _currentGadgetData!.Width = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "HEIGHT":
                _currentGadgetData!.Height = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "SPEED":
                _currentGadgetData!.Speed = ReadingHelpers.ReadInt(tokens[1]);
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
                _currentGadgetData!.Skill = ReadingHelpers.ReadFormattedString(tokens[1..]);
                break;

            case "SKILL_COUNT":
                _currentGadgetData!.SkillCount = ReadingHelpers.ReadInt(tokens[1]);
                break;

            case "$END":
                _allGadgetData.Add(_currentGadgetData!);
                _currentGadgetData = null;
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}