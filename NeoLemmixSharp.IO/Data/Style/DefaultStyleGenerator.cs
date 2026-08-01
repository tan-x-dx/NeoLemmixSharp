using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style.Theme;
using System.Runtime.CompilerServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Data.Style;

internal static class DefaultStyleGenerator
{
    internal static StyleData GenerateDefaultStyle()
    {
        var themeData = GenerateDefaultThemeData();

        var result = new StyleData(IoConstants.DefaultStyleIdentifier, FileFormats.FileFormatType.Default)
        {
            Name = IoConstants.DefaultStyleName,
            Author = IoConstants.DefaultStyleAuthor,
            Description = IoConstants.DefaultStyleDescription,
            ThemeData = themeData
        };

        return result;
    }

    private static ThemeData GenerateDefaultThemeData()
    {
        var defaultActionSprites = CreateDefaultActionSpriteData();
        var defaultTribeColors = CreateDefaultTribeColorData();

        var lemmingSpriteData = new LemmingSpriteData(IoConstants.DefaultStyleIdentifier, defaultTribeColors, defaultActionSprites);

        var result = new ThemeData
        {
            Mask = 0xFFD08020.AsAbgrColor(),
            Minimap = EngineConstants.PanelWhite,
            Background = Color.Black,
            OneWayArrows = EngineConstants.PanelBlue,
            PickupBorder = 0xFF4040E0.AsAbgrColor(),
            PickupInside = 0xFFD08020.AsAbgrColor(),

            LemmingSpriteData = lemmingSpriteData
        };

        return result;
    }

    private static LemmingActionSpriteData[] CreateDefaultActionSpriteData()
    {
        LemmingActionSpriteLayerData[] oneLayerTrueColor =
        [
            new LemmingActionSpriteLayerData(0, TribeSpriteLayerColorType.TrueColor)
        ];
        LemmingActionSpriteLayerData[] fourLayers =
        [
            new LemmingActionSpriteLayerData(0, TribeSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(1, TribeSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(2, TribeSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(3, TribeSpriteLayerColorType.LemmingFootColor)
        ];
        LemmingActionSpriteLayerData[] fiveLayers =
        [
            new LemmingActionSpriteLayerData(0, TribeSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(1, TribeSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(2, TribeSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(3, TribeSpriteLayerColorType.LemmingFootColor),
            new LemmingActionSpriteLayerData(4, TribeSpriteLayerColorType.TribePaintColor)
        ];
        LemmingActionSpriteLayerData[] fiveLayersTrueColor =
        [
            new LemmingActionSpriteLayerData(0, TribeSpriteLayerColorType.TrueColor),
            new LemmingActionSpriteLayerData(1, TribeSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(2, TribeSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(3, TribeSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(4, TribeSpriteLayerColorType.LemmingFootColor)
        ];
        LemmingActionSpriteLayerData[] sixLayersTrueColor =
        [
            new LemmingActionSpriteLayerData(0, TribeSpriteLayerColorType.TrueColor),
            new LemmingActionSpriteLayerData(1, TribeSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(2, TribeSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(3, TribeSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(4, TribeSpriteLayerColorType.LemmingFootColor),
            new LemmingActionSpriteLayerData(5, TribeSpriteLayerColorType.TribePaintColor)
        ];

        var result = new LemmingActionSpriteData[LemmingActionConstants.NumberOfLemmingActions];

        SetLemmingActionSpriteData(LemmingActionType.WalkerAction, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.ClimberAction, new Point(8, 12), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.FloaterAction, new Point(4, 16), fiveLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.BlockerAction, new Point(5, 13), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.BuilderAction, new Point(3, 13), sixLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.BasherAction, new Point(8, 10), fiveLayers);
        SetLemmingActionSpriteData(LemmingActionType.MinerAction, new Point(7, 13), fiveLayers);
        SetLemmingActionSpriteData(LemmingActionType.DiggerAction, new Point(7, 12), fiveLayers);
        SetLemmingActionSpriteData(LemmingActionType.PlatformerAction, new Point(3, 13), sixLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.StackerAction, new Point(3, 13), sixLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.FencerAction, new Point(3, 10), sixLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.GliderAction, new Point(5, 16), fiveLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.JumperAction, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.SwimmerAction, new Point(6, 8), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.ShimmierAction, new Point(3, 8), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.LasererAction, new Point(3, 10), fiveLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.SliderAction, new Point(4, 11), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.FallerAction, new Point(3, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.AscenderAction, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.ShruggerAction, new Point(3, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.DrownerAction, new Point(5, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.HoisterAction, new Point(5, 12), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.DehoisterAction, new Point(5, 13), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.ReacherAction, new Point(3, 9), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.DisarmerAction, new Point(1, 11), fiveLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.ExiterAction, new Point(2, 16), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.ExploderAction, new Point(17, 21), oneLayerTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.OhNoerAction, new Point(3, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.SplatterAction, new Point(7, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.StonerAction, new Point(17, 21), oneLayerTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.VaporiserAction, new Point(5, 14), fiveLayersTrueColor);
        SetLemmingActionSpriteData(LemmingActionType.RotateClockwiseAction, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.RotateCounterclockwiseAction, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(LemmingActionType.RotateHalfAction, new Point(2, 10), fourLayers);

        return result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetLemmingActionSpriteData(LemmingActionType lemmingActionType, Point anchorPoint, LemmingActionSpriteLayerData[] layers)
        {
            result[(int)lemmingActionType] = new LemmingActionSpriteData(lemmingActionType, anchorPoint, layers);
        }
    }

    private static TribeColorData[] CreateDefaultTribeColorData()
    {
        var defaultSkinColor = 0xFFF0D0D0.AsAbgrColor();
        var defaultAcidLemmingFootColor = 0xFF00F000.AsAbgrColor();
        var defaultWaterLemmingFootColor = 0xFF00F0F0.AsAbgrColor();
        var defaultZombieSkinColor = 0xFF777777.AsAbgrColor();
        var defaultNeutralBodyColor = 0xFF999999.AsAbgrColor();

        var defaultPaintColor = 0xFFFF00FF.AsAbgrColor();

        var tribe0HairColor = 0xFF04B000.AsAbgrColor();
        var tribe0BodyColor = 0xFF4044DF.AsAbgrColor();

        var tribe1HairColor = 0xFF00B0A9.AsAbgrColor();
        var tribe1BodyColor = 0xFFD53FDE.AsAbgrColor();

        var tribe2HairColor = 0xFF0004B0.AsAbgrColor();
        var tribe2BodyColor = 0xFFDE3F46.AsAbgrColor();

        var tribe3HairColor = 0xFFAD00B0.AsAbgrColor();
        var tribe3BodyColor = 0xFFDED13F.AsAbgrColor();

        var tribe4HairColor = 0xFFB00000.AsAbgrColor();
        var tribe4BodyColor = 0xFF4ADE3F.AsAbgrColor();

        var tribe5HairColor = 0xFFB0A900.AsAbgrColor();
        var tribe5BodyColor = 0xFF3FDED5.AsAbgrColor();

        var result = new TribeColorData[EngineConstants.MaxNumberOfTribes];

        result[0] = new TribeColorData(
            tribe0HairColor,
            tribe0BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe0BodyColor,
            tribe0HairColor,
            defaultNeutralBodyColor,
            defaultPaintColor);

        result[1] = new TribeColorData(
            tribe1HairColor,
            tribe1BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe1BodyColor,
            tribe1HairColor,
            defaultNeutralBodyColor,
            defaultPaintColor);

        result[2] = new TribeColorData(
            tribe2HairColor,
            tribe2BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe2BodyColor,
            tribe2HairColor,
            defaultNeutralBodyColor,
            defaultPaintColor);

        result[3] = new TribeColorData(
            tribe3HairColor,
            tribe3BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe3BodyColor,
            tribe3HairColor,
            defaultNeutralBodyColor,
            defaultPaintColor);

        result[4] = new TribeColorData(
            tribe4HairColor,
            tribe4BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe4BodyColor,
            tribe4HairColor,
            defaultNeutralBodyColor,
            defaultPaintColor);

        result[5] = new TribeColorData(
            tribe5HairColor,
            tribe5BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe5BodyColor,
            tribe5HairColor,
            defaultNeutralBodyColor,
            defaultPaintColor);

        return result;
    }
}
