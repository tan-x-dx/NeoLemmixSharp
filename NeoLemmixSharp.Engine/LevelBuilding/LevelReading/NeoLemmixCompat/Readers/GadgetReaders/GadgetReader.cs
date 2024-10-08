﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class GadgetReader : INeoLemmixDataReader
{
    private readonly CaseInvariantCharEqualityComparer _charEqualityComparer;

    private NeoLemmixGadgetData? _currentGadgetData;
    private string? _currentStyle;
    private string? _currentFolder;

    public Dictionary<string, NeoLemmixGadgetArchetypeData> GadgetArchetypes { get; } = new();
    public List<NeoLemmixGadgetData> AllGadgetData { get; } = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";

    public GadgetReader(CaseInvariantCharEqualityComparer charEqualityComparer)
    {
        _charEqualityComparer = charEqualityComparer;
    }

    public void BeginReading(ReadOnlySpan<char> line)
    {
        _currentGadgetData = new NeoLemmixGadgetData();
        FinishedReading = false;
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

        var currentGadgetData = _currentGadgetData!;

        switch (firstToken)
        {
            case "STYLE":
                var rest = line.TrimAfterIndex(secondTokenIndex);
                if (_currentStyle is null)
                {
                    SetCurrentStyle(rest);
                }
                else
                {
                    var currentStyleSpan = _currentStyle.AsSpan();
                    if (!currentStyleSpan.SequenceEqual(rest))
                    {
                        SetCurrentStyle(rest);
                    }
                }

                break;

            case "PIECE":
                var gadgetArchetypeData = GetOrLoadGadgetArchetypeData(line.TrimAfterIndex(secondTokenIndex));
                currentGadgetData.GadgetArchetypeId = gadgetArchetypeData.GadgetArchetypeId;
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
                if (!NxlvReadingHelpers.GetSkillByName(secondToken, _charEqualityComparer, out var skill))
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

            #region Window properties

            case "CLIMBER":
                currentGadgetData.State |= 1U << LevelConstants.ClimberBitIndex;
                break;

            case "DISARMER":
                currentGadgetData.State |= 1U << LevelConstants.DisarmerBitIndex;
                break;

            case "FLOATER":
                currentGadgetData.State |= 1U << LevelConstants.FloaterBitIndex;
                currentGadgetData.State &= ~(1U << LevelConstants.GliderBitIndex); // Deliberately knock out the glider
                break;

            case "GLIDER":
                currentGadgetData.State |= 1U << LevelConstants.GliderBitIndex;
                currentGadgetData.State &= ~(1U << LevelConstants.FloaterBitIndex); // Deliberately knock out the floater
                break;

            case "NEUTRAL":
                currentGadgetData.State |= 1U << LevelConstants.NeutralBitIndex;
                break;

            case "SLIDER":
                currentGadgetData.State |= 1U << LevelConstants.SliderBitIndex;
                break;

            case "SWIMMER":
                currentGadgetData.State |= 1U << LevelConstants.SwimmerBitIndex;
                break;

            case "ZOMBIE":
                currentGadgetData.State |= 1U << LevelConstants.ZombieBitIndex;
                break;

            #endregion

            case "$END":
                AllGadgetData.Add(_currentGadgetData!);
                _currentGadgetData = null;
                FinishedReading = true;
                break;

            default:
                NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
        }

        return false;
    }

    private void SetCurrentStyle(ReadOnlySpan<char> style)
    {
        _currentStyle = style.ToString();
        _currentFolder = Path.Combine(
            RootDirectoryManager.RootDirectory,
            NeoLemmixFileExtensions.StyleFolderName,
            _currentStyle,
            NeoLemmixFileExtensions.GadgetFolderName);
    }

    private NeoLemmixGadgetArchetypeData GetOrLoadGadgetArchetypeData(ReadOnlySpan<char> piece)
    {
        ref var gadgetArchetypeData = ref NxlvReadingHelpers.GetArchetypeDataRef(
            _currentStyle!,
            piece,
            GadgetArchetypes,
            out var exists);

        if (exists)
            return gadgetArchetypeData!;

        var gadgetPiece = piece.ToString();

        gadgetArchetypeData = new NeoLemmixGadgetArchetypeData
        {
            GadgetArchetypeId = GadgetArchetypes.Count - 1,
            Style = _currentStyle!,
            GadgetPiece = gadgetPiece
        };

        ProcessGadgetArchetypeData(gadgetArchetypeData);

        return gadgetArchetypeData;
    }

    private void ProcessGadgetArchetypeData(NeoLemmixGadgetArchetypeData gadgetArchetypeData)
    {
        var dataReaders = new INeoLemmixDataReader[]
        {
            new GadgetArchetypeDataReader(gadgetArchetypeData),
            new PrimaryAnimationReader(gadgetArchetypeData),
            new SecondaryAnimationReader(gadgetArchetypeData)
        };

        var dataReaderList = new DataReaderList(dataReaders);

        var rootFilePath = Path.Combine(_currentFolder!, gadgetArchetypeData.GadgetPiece!);
        rootFilePath = Path.ChangeExtension(rootFilePath, NeoLemmixFileExtensions.GadgetFileExtension);

        dataReaderList.ReadFile(rootFilePath);
    }
}