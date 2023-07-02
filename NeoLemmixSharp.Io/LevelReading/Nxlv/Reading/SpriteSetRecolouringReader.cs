using NeoLemmixSharp.Io.LevelReading.Data.SpriteSet;

namespace NeoLemmixSharp.Io.LevelReading.Nxlv.Reading;

public sealed class SpriteSetRecolouringReader : IDataReader
{
    private readonly LemmingSpriteSetRecolouring _lemmingSpriteSetRecolouring;

    public SpriteSetRecolouringReader(LemmingSpriteSetRecolouring lemmingSpriteSetRecolouring)
    {
        _lemmingSpriteSetRecolouring = lemmingSpriteSetRecolouring;
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
                _lemmingSpriteSetRecolouring.Mask = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_HAIR":
                _lemmingSpriteSetRecolouring.LemmingHair = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_CLOTHES":
                _lemmingSpriteSetRecolouring.LemmingClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_SKIN":
                _lemmingSpriteSetRecolouring.LemmingSkin = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_BUILDER_SACK":
                _lemmingSpriteSetRecolouring.LemmingBuilderSack = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_UMBRELLA":
                _lemmingSpriteSetRecolouring.LemmingUmbrella = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_ZOMBIE_SKIN":
                _lemmingSpriteSetRecolouring.LemmingZombieSkin = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_ATHLETE_HAIR":
                _lemmingSpriteSetRecolouring.LemmingAthleteHair = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_ATHLETE_CLOTHES":
                _lemmingSpriteSetRecolouring.LemmingAthleteClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_NEUTRAL_CLOTHES":
                _lemmingSpriteSetRecolouring.LemmingNeutralClothes = ReadingHelpers.ReadUint(tokens[1], false);
                break;

            case "LEMMING_SELECTED_CLOTHES":
                _lemmingSpriteSetRecolouring.LemmingSelectedClothes = ReadingHelpers.ReadUint(tokens[1], false);
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