using NeoLemmixSharp.Engine.LevelBuilding.Data.SpriteSet;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class SpriteSetRecoloringReader : INeoLemmixDataReader
{
    private readonly LemmingSpriteSetRecoloring _lemmingSpriteSetRecoloring;

    public SpriteSetRecoloringReader(LemmingSpriteSetRecoloring lemmingSpriteSetRecoloring)
    {
        _lemmingSpriteSetRecoloring = lemmingSpriteSetRecoloring;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SPRITESET_RECOLORING";

    public void BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(ReadOnlySpan<char> line)
    {
        var firstToken = ReadingHelpers.GetToken(line, 0, out var firstTokenIndex);
        var secondToken = ReadingHelpers.GetToken(line, 1, out _);
        var rest = line[(1 + firstTokenIndex + firstToken.Length)..];

        switch (firstToken)
        {
            case "MASK":
                _lemmingSpriteSetRecoloring.Mask = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_HAIR":
                _lemmingSpriteSetRecoloring.LemmingHair = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingClothes = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_SKIN":
                _lemmingSpriteSetRecoloring.LemmingSkin = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_BUILDER_SACK":
                _lemmingSpriteSetRecoloring.LemmingBuilderSack = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_UMBRELLA":
                _lemmingSpriteSetRecoloring.LemmingUmbrella = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_ZOMBIE_SKIN":
                _lemmingSpriteSetRecoloring.LemmingZombieSkin = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_ATHLETE_HAIR":
                _lemmingSpriteSetRecoloring.LemmingAthleteHair = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_ATHLETE_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingAthleteClothes = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_NEUTRAL_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingNeutralClothes = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "LEMMING_SELECTED_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingSelectedClothes = 0xff000000U | ReadingHelpers.ParseUnsignedNumericalValue<uint>(secondToken);
                break;

            case "$END":
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{firstToken}] line: \"{line}\"");
        }
    }
}