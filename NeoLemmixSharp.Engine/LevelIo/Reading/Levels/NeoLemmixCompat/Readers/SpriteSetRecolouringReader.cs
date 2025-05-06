namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels.NeoLemmixCompat.Readers;

public sealed class SpriteSetRecoloringReader : NeoLemmixDataReader
{
    //private readonly LemmingSpriteSetRecoloring _lemmingSpriteSetRecoloring = new();

    public SpriteSetRecoloringReader()
        : base("$SPRITESET_RECOLORING")
    {

    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;
        return false;
    }

    public override bool ReadNextLine(ReadOnlySpan<char> line)
    {
        NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out _);

        /* switch (firstToken)
         {
             case "MASK":
                 _lemmingSpriteSetRecoloring.Mask = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_HAIR":
                 _lemmingSpriteSetRecoloring.LemmingHair = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_CLOTHES":
                 _lemmingSpriteSetRecoloring.LemmingClothes = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_SKIN":
                 _lemmingSpriteSetRecoloring.LemmingSkin = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_BUILDER_SACK":
                 _lemmingSpriteSetRecoloring.LemmingBuilderSack =
                     0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_UMBRELLA":
                 _lemmingSpriteSetRecoloring.LemmingUmbrella = 0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_ZOMBIE_SKIN":
                 _lemmingSpriteSetRecoloring.LemmingZombieSkin =
                     0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_ATHLETE_HAIR":
                 _lemmingSpriteSetRecoloring.LemmingAthleteHair =
                     0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_ATHLETE_CLOTHES":
                 _lemmingSpriteSetRecoloring.LemmingAthleteClothes =
                     0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_NEUTRAL_CLOTHES":
                 _lemmingSpriteSetRecoloring.LemmingNeutralClothes =
                     0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "LEMMING_SELECTED_CLOTHES":
                 _lemmingSpriteSetRecoloring.LemmingSelectedClothes =
                     0xff000000U | NxlvReadingHelpers.ParseHex<uint>(secondToken);
                 break;

             case "$END":
                 FinishedReading = true;
                 break;

             default:
                 NxlvReadingHelpers.ThrowUnknownTokenException("Gadget Archetype Data", firstToken, line);
                 break;
         }*/

        return false;
    }
}