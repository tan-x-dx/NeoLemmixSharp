using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class GadgetReader : NeoLemmixDataReader
{
    private NeoLemmixGadgetData? _currentGadgetData;
    private string? _currentStyle;
    private string? _currentFolder;

    public Dictionary<string, NeoLemmixGadgetArchetypeData> GadgetArchetypes { get; } = new(StringComparer.OrdinalIgnoreCase);
    public List<NeoLemmixGadgetData> AllGadgetData { get; } = new();

    public GadgetReader()
        : base("$GADGET")
    {
        RegisterTokenAction("STYLE", SetStyle);
        RegisterTokenAction("PIECE", SetPiece);
        RegisterTokenAction("X", SetX);
        RegisterTokenAction("Y", SetY);
        RegisterTokenAction("WIDTH", SetWidth);
        RegisterTokenAction("HEIGHT", SetHeight);
        RegisterTokenAction("SPEED", SetSpeed);
        RegisterTokenAction("ANGLE", SetAngle);
        RegisterTokenAction("NO_OVERWRITE", SetNoOverwrite);
        RegisterTokenAction("ONLY_ON_TERRAIN", SetOnlyOnTerrain);
        RegisterTokenAction("ONE_WAY", SetOneWay);
        RegisterTokenAction("FLIP_VERTICAL", SetFlipVertical);
        RegisterTokenAction("FLIP_HORIZONTAL", SetFlipHorizontal);
        RegisterTokenAction("ROTATE", SetRotate);
        RegisterTokenAction("SKILL", SetSkill);
        RegisterTokenAction("SKILL_COUNT", SetSkillCount);
        RegisterTokenAction("LEMMINGS", SetLemmingCount);
        RegisterTokenAction("CLIMBER", SetClimber);
        RegisterTokenAction("DISARMER", SetDisarmer);
        RegisterTokenAction("FLOATER", SetFloater);
        RegisterTokenAction("GLIDER", SetGlider);
        RegisterTokenAction("NEUTRAL", SetNeutral);
        RegisterTokenAction("SLIDER", SetSlider);
        RegisterTokenAction("SWIMMER", SetSwimmer);
        RegisterTokenAction("ZOMBIE", SetZombie);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        _currentGadgetData = new NeoLemmixGadgetData();
        FinishedReading = false;
        return false;
    }

    private void SetStyle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var rest = line[secondTokenIndex..].Trim();
        if (_currentStyle is null)
        {
            SetCurrentStyle(rest);
        }
        else
        {
            var currentStyleSpan = _currentStyle.AsSpan();
            if (!currentStyleSpan.Equals(rest, StringComparison.OrdinalIgnoreCase))
            {
                SetCurrentStyle(rest);
            }
        }
    }

    private void SetPiece(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var gadgetArchetypeData = GetOrLoadGadgetArchetypeData(line[secondTokenIndex..].Trim());
        _currentGadgetData!.GadgetArchetypeId = gadgetArchetypeData.GadgetArchetypeId;
    }

    private void SetX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.X = int.Parse(secondToken);
    }

    private void SetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Y = int.Parse(secondToken);
    }

    private void SetWidth(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Width = int.Parse(secondToken);
    }

    private void SetHeight(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Height = int.Parse(secondToken);
    }

    private void SetSpeed(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Speed = int.Parse(secondToken);
    }

    private void SetAngle(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Angle = int.Parse(secondToken);
    }

    private void SetNoOverwrite(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.NoOverwrite = true;
    }

    private void SetOnlyOnTerrain(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.OnlyOnTerrain = true;
    }

    private void SetOneWay(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetFlipVertical(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.FlipVertical = true;
    }

    private void SetFlipHorizontal(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.FlipHorizontal = true;
    }

    private void SetRotate(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Rotate = true;
    }

    private void SetSkill(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        if (!NxlvReadingHelpers.TryGetSkillByName(secondToken, out var skill))
            throw new Exception($"Unknown token: {secondToken}");

        _currentGadgetData!.Skill = skill;
    }

    private void SetSkillCount(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var amount = secondToken is "INFINITE"
            ? EngineConstants.InfiniteSkillCount
            : int.Parse(secondToken);

        _currentGadgetData!.SkillCount = amount;
    }

    private void SetLemmingCount(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.LemmingCount = int.Parse(secondToken);
    }

    private void SetClimber(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.ClimberBitIndex;
    }

    private void SetDisarmer(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.DisarmerBitIndex;
    }

    private void SetFloater(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.FloaterBitIndex;
        _currentGadgetData.State &= ~(1U << EngineConstants.GliderBitIndex); // Deliberately knock out the glider
    }

    private void SetGlider(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.GliderBitIndex;
        _currentGadgetData.State &= ~(1U << EngineConstants.FloaterBitIndex); // Deliberately knock out the floater
    }

    private void SetNeutral(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.NeutralBitIndex;
    }

    private void SetSlider(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.SliderBitIndex;
    }

    private void SetSwimmer(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.SwimmerBitIndex;
    }

    private void SetZombie(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.State |= 1U << EngineConstants.ZombieBitIndex;
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        AllGadgetData.Add(_currentGadgetData!);
        _currentGadgetData = null;
        FinishedReading = true;
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
        var dataReaders = new NeoLemmixDataReader[]
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