using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders;

public sealed class SecondaryAnimationReader : INeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;

    private AnimationData? _secondaryAnimationData;
    private AnimationTriggerReader? _triggerReader;

    public SecondaryAnimationReader(NeoLemmixGadgetArchetypeData gadgetArchetypeData)
    {
        _gadgetArchetypeData = gadgetArchetypeData;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$ANIMATION";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        _secondaryAnimationData = new AnimationData();
    }

    public bool ReadNextLine(ReadOnlySpan<char> line)
    {
        if (_triggerReader != null)
        {
            var result = _triggerReader.ReadNextLine(line);

            if (_triggerReader.FinishedReading)
            {
                _triggerReader = null;
            }

            return result;
        }

        ReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        var secondaryAnimationData = _secondaryAnimationData!;

        switch (firstToken)
        {
            case "INITIAL_FRAME":
                var initialFrame = secondToken is "RANDOM"
                    ? -1
                    : int.Parse(secondToken);

                secondaryAnimationData.InitialFrame = initialFrame;
                break;

            case "FRAMES":
                secondaryAnimationData.NumberOfFrames = int.Parse(secondToken);
                break;

            case "NAME":
                secondaryAnimationData.Name = secondToken.ToString();
                break;

            case "HIDE":
                secondaryAnimationData.Hide = true;
                break;

            case "OFFSET_X":
                secondaryAnimationData.OffsetX = int.Parse(secondToken);
                break;

            case "OFFSET_Y":
                secondaryAnimationData.OffsetY = int.Parse(secondToken);
                break;

            case "COLOR":

                break;

            case "$TRIGGER":
                _triggerReader = new AnimationTriggerReader(secondaryAnimationData.TriggerData);
                _triggerReader.BeginReading(firstToken);
                break;

            case "$END":
                _secondaryAnimationData = null;
                _gadgetArchetypeData.AnimationData.Add(secondaryAnimationData);
                FinishedReading = true;
                break;

            default:
                ReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
                break;
        }

        return false;
    }
}