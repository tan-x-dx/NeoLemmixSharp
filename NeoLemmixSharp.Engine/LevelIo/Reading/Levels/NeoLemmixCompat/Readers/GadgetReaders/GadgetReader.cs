using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class GadgetReader : NeoLemmixDataReader
{
    private readonly UniqueStringSet _uniqueStringSet;

    private NeoLemmixGadgetData? _currentGadgetData;

    public List<NeoLemmixGadgetData> AllGadgetData { get; } = new();

    public GadgetReader(UniqueStringSet uniqueStringSet)
        : base("$GADGET")
    {
        _uniqueStringSet = uniqueStringSet;

        SetNumberOfTokens(26);

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
        _currentGadgetData!.Style = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
    }

    private void SetPiece(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        _currentGadgetData!.Piece = _uniqueStringSet.GetUniqueStringInstance(line[secondTokenIndex..]);
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
        var amount = TokensMatch(secondToken, "INFINITE")
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
}