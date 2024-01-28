using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class GadgetReader : INeoLemmixDataReader
{
    private readonly CaseInvariantCharEqualityComparer _charEqualityComparer = new();
    private readonly Dictionary<string, GadgetArchetypeData> _gadgetArchetypes = new();
    private readonly List<NeoLemmixGadgetData> _allGadgetData = new();

    private NeoLemmixGadgetData? _currentGadgetData;
    private string? _currentStyle;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentGadgetData = new NeoLemmixGadgetData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out _);
        var secondToken = ReadingHelpers.GetToken(line, 1, out var secondTokenIndex);
        var rest = secondToken.IsEmpty
            ? ReadOnlySpan<char>.Empty
            : line[secondTokenIndex..];

        var currentGadgetData = _currentGadgetData!;

        switch (firstToken)
        {
            case "STYLE":
                if (_currentStyle is null)
                {
                    _currentStyle = rest.GetString();
                }
                else
                {
                    var currentStyleSpan = _currentStyle.AsSpan();
                    if (!currentStyleSpan.SequenceEqual(rest))
                    {
                        _currentStyle = rest.ToString();
                    }
                }

                break;

            case "PIECE":
                var terrainArchetypeData = GetOrLoadGadgetArchetypeData(rest);
                currentGadgetData.GadgetArchetypeId = terrainArchetypeData.GadgetArchetypeId;
                break;

            case "X":
                currentGadgetData.X = int.Parse(secondToken);
                break;

            case "Y":
                currentGadgetData.Y = int.Parse(secondToken);
                break;

            case "WIDTH":
                currentGadgetData.Width = int.Parse(secondToken);
                break;

            case "HEIGHT":
                currentGadgetData.Height = int.Parse(secondToken);
                break;

            case "SPEED":
                currentGadgetData.Speed = int.Parse(secondToken);
                break;

            case "ANGLE":
                currentGadgetData.Angle = int.Parse(secondToken);
                break;

            case "NO_OVERWRITE":
                currentGadgetData.NoOverwrite = true;
                break;

            case "ONLY_ON_TERRAIN":
                currentGadgetData.OnlyOnTerrain = true;
                break;

            case "ONE_WAY":
                break;

            case "FLIP_VERTICAL":
                currentGadgetData.FlipVertical = true;
                break;

            case "FLIP_HORIZONTAL":
                currentGadgetData.FlipHorizontal = true;
                break;

            case "ROTATE":
                currentGadgetData.Rotate = true;
                break;

            case "SKILL":
                if (!ReadingHelpers.GetSkillByName(secondToken, _charEqualityComparer, out var skill))
                    throw new Exception($"Unknown token: {secondToken}");

                currentGadgetData.Skill = skill;
                break;

            case "SKILL_COUNT":
                var amount = secondToken is "INFINITE"
                    ? LevelConstants.InfiniteSkillCount
                    : int.Parse(secondToken);

                currentGadgetData.SkillCount = amount;
                break;

            case "LEMMINGS":
                currentGadgetData.LemmingCount = int.Parse(secondToken);
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

    private GadgetArchetypeData GetOrLoadGadgetArchetypeData(ReadOnlySpan<char> piece)
    {
        var currentStyleLength = _currentStyle!.Length;

        var bufferSize = currentStyleLength + piece.Length + 1;
        Span<char> terrainArchetypeDataKeySpan = bufferSize > ReadingHelpers.MaxStackallocSize
            ? new char[bufferSize]
            : stackalloc char[bufferSize];

        _currentStyle.AsSpan().CopyTo(terrainArchetypeDataKeySpan);
        piece.CopyTo(terrainArchetypeDataKeySpan[(currentStyleLength + 1)..]);
        terrainArchetypeDataKeySpan[currentStyleLength] = ':';

        var terrainArchetypeDataKey = terrainArchetypeDataKeySpan.ToString();

        if (_gadgetArchetypes.TryGetValue(terrainArchetypeDataKey, out var gadgetArchetypeData))
            return gadgetArchetypeData;

        var terrainPiece = piece.ToString();

        gadgetArchetypeData = new GadgetArchetypeData
        {
            GadgetArchetypeId = _gadgetArchetypes.Count,
            Style = _currentStyle,
            Gadget = terrainPiece
        };

        var key = terrainArchetypeDataKeySpan.ToString();
        _gadgetArchetypes[key] = gadgetArchetypeData;

        return gadgetArchetypeData;
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