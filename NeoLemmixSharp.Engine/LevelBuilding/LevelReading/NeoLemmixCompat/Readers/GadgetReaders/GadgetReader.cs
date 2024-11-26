﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class GadgetReader : INeoLemmixDataReader
{
    private readonly IEqualityComparer<char> _charEqualityComparer;

    private NeoLemmixGadgetData? _currentGadgetData;
    private string? _currentStyle;
    private string? _currentFolder;

    public Dictionary<string, NeoLemmixGadgetArchetypeData> GadgetArchetypes { get; } = new();
    public List<NeoLemmixGadgetData> AllGadgetData { get; } = new();

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$GADGET";

    public GadgetReader(IEqualityComparer<char> charEqualityComparer)
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
                    ? EngineConstants.InfiniteSkillCount
                    : int.Parse(secondToken);

                currentGadgetData.SkillCount = amount;
                break;

            case "LEMMINGS":
                currentGadgetData.LemmingCount = int.Parse(secondToken);
                break;

            #region Window properties

            case "CLIMBER":
                currentGadgetData.State |= 1U << EngineConstants.ClimberBitIndex;
                break;

            case "DISARMER":
                currentGadgetData.State |= 1U << EngineConstants.DisarmerBitIndex;
                break;

            case "FLOATER":
                currentGadgetData.State |= 1U << EngineConstants.FloaterBitIndex;
                currentGadgetData.State &= ~(1U << EngineConstants.GliderBitIndex); // Deliberately knock out the glider
                break;

            case "GLIDER":
                currentGadgetData.State |= 1U << EngineConstants.GliderBitIndex;
                currentGadgetData.State &= ~(1U << EngineConstants.FloaterBitIndex); // Deliberately knock out the floater
                break;

            case "NEUTRAL":
                currentGadgetData.State |= 1U << EngineConstants.NeutralBitIndex;
                break;

            case "SLIDER":
                currentGadgetData.State |= 1U << EngineConstants.SliderBitIndex;
                break;

            case "SWIMMER":
                currentGadgetData.State |= 1U << EngineConstants.SwimmerBitIndex;
                break;

            case "ZOMBIE":
                currentGadgetData.State |= 1U << EngineConstants.ZombieBitIndex;
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
        var currentStyleLength = _currentStyle!.Length;

        // Safeguard against potential stack overflow.
        // Will almost certainly be a small buffer
        // allocated on the stack, but still...
        var bufferSize = currentStyleLength + piece.Length + 1;
        Span<char> archetypeDataKeySpan = bufferSize > NxlvReadingHelpers.MaxStackallocSize
            ? new char[bufferSize]
            : stackalloc char[bufferSize];

        _currentStyle.CopyTo(archetypeDataKeySpan);
        archetypeDataKeySpan[currentStyleLength] = ':';
        piece.CopyTo(archetypeDataKeySpan[(currentStyleLength + 1)..]);

        var alternateLookup = GadgetArchetypes.GetAlternateLookup<ReadOnlySpan<char>>();

        ref var dictionaryEntry = ref CollectionsMarshal.GetValueRefOrAddDefault(
            alternateLookup,
            archetypeDataKeySpan,
            out var exists);

        if (!exists)
        {
            dictionaryEntry = CreateNewGadgetArchetypeData(piece);
        }

        return dictionaryEntry!;
    }

    private NeoLemmixGadgetArchetypeData CreateNewGadgetArchetypeData(
        ReadOnlySpan<char> piece)
    {
        var gadgetPiece = piece.ToString();

        var gadgetArchetypeData = new NeoLemmixGadgetArchetypeData
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

        var rootFilePath = Path.Combine(_currentFolder!, gadgetArchetypeData.GadgetPiece!);
        rootFilePath = Path.ChangeExtension(rootFilePath, NeoLemmixFileExtensions.GadgetFileExtension);

        using var dataReaderList = new DataReaderList(rootFilePath, dataReaders);
        dataReaderList.ReadFile();
    }
}