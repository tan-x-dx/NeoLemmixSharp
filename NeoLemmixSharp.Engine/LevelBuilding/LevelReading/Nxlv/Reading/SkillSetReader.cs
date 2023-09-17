using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Io.LevelReading.Data;

namespace NeoLemmixSharp.Io.LevelReading.Nxlv.Reading;

public sealed class SkillSetReader : IDataReader
{
    private readonly Dictionary<string, LemmingAction> _actionLookup = new();
    private readonly List<SkillSetData> _skillSetData;

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SKILLSET";

    public SkillSetReader(LevelData levelData)
    {
        _skillSetData = levelData.SkillSetData;
    }

    public void BeginReading(string[] tokens)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "WALKER":
                _skillSetData.Add(CreateSkillSetData("walker", tokens[1]));
                break;



            case "$END":
                FinishedReading = true;
                break;

            default:
                throw new Exception($"Unknown token: {tokens[0]}");
        }
    }

    private static SkillSetData CreateSkillSetData(string skillName, string amountString)
    {
        var amount = ReadingHelpers.ReadInt(amountString);

        return new SkillSetData
        {
            SkillName = skillName,
            NumberOfSkills = amount,
            TeamId = 0
        };
    }

    private static void Foo(string[] tokens)
    {

    }
}