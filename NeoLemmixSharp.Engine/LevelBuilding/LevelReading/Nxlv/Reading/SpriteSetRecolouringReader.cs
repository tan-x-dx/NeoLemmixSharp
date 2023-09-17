using NeoLemmixSharp.Engine.LevelBuilding.Data.SpriteSet;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Nxlv.Reading;

public sealed class SpriteSetRecoloringReader : IDataReader
{
    private readonly LemmingSpriteSetRecoloring _lemmingSpriteSetRecoloring;

    public SpriteSetRecoloringReader(LemmingSpriteSetRecoloring lemmingSpriteSetRecoloring)
    {
        _lemmingSpriteSetRecoloring = lemmingSpriteSetRecoloring;
    }

    public bool FinishedReading { get; private set; }
    public string IdentifierToken => "$SPRITESET_RECOLORING";

    public void BeginReading(string[] tokens)
    {
        FinishedReading = false;
    }

    public void ReadNextLine(string[] tokens)
    {
        switch (tokens[0])
        {
            case "MASK":
                _lemmingSpriteSetRecoloring.Mask = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_HAIR":
                _lemmingSpriteSetRecoloring.LemmingHair = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_SKIN":
                _lemmingSpriteSetRecoloring.LemmingSkin = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_BUILDER_SACK":
                _lemmingSpriteSetRecoloring.LemmingBuilderSack = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_UMBRELLA":
                _lemmingSpriteSetRecoloring.LemmingUmbrella = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_ZOMBIE_SKIN":
                _lemmingSpriteSetRecoloring.LemmingZombieSkin = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_ATHLETE_HAIR":
                _lemmingSpriteSetRecoloring.LemmingAthleteHair = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_ATHLETE_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingAthleteClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_NEUTRAL_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingNeutralClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_SELECTED_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingSelectedClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "$END":
                FinishedReading = true;
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown token when parsing {IdentifierToken}: [{tokens[0]}] line: \"{string.Join(' ', tokens)}\"");
        }
    }
}