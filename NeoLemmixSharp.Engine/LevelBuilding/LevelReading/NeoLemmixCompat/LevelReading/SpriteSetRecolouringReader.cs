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
                _lemmingSpriteSetRecoloring.Mask = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_HAIR":
                _lemmingSpriteSetRecoloring.LemmingHair = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingClothes = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_SKIN":
                _lemmingSpriteSetRecoloring.LemmingSkin = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_BUILDER_SACK":
                _lemmingSpriteSetRecoloring.LemmingBuilderSack = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_UMBRELLA":
                _lemmingSpriteSetRecoloring.LemmingUmbrella = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_ZOMBIE_SKIN":
                _lemmingSpriteSetRecoloring.LemmingZombieSkin = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_ATHLETE_HAIR":
                _lemmingSpriteSetRecoloring.LemmingAthleteHair = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_ATHLETE_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingAthleteClothes = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_NEUTRAL_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingNeutralClothes = ReadingHelpers.ReadUint(secondToken, false);
                break;

            case "LEMMING_SELECTED_CLOTHES":
                _lemmingSpriteSetRecoloring.LemmingSelectedClothes = ReadingHelpers.ReadUint(secondToken, false);
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