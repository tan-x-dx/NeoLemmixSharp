using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Theme;
using System.Runtime.CompilerServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Data.Style;

internal static class DefaultStyleGenerator
{
    internal static StyleData GenerateDefaultStyle()
    {
        var themeData = GenerateDefaultThemeData();

        var result = new StyleData(StyleCache.DefaultStyleFormatPair)
        {
            ThemeData = themeData
        };

        return result;
    }

    private static ThemeData GenerateDefaultThemeData()
    {
        var result = new ThemeData
        {
            StyleIdentifier = StyleCache.DefaultStyleFormatPair.StyleIdentifier,

            Mask = new Color(),
            Minimap = new Color(),
            Background = new Color(),
            OneWayArrows = new Color(),
            PickupBorder = new Color(),
            PickupInside = new Color(),
        };

        SetUpActionSpriteData(result);
        SetUpTribeColorData(result);

        return result;
    }

    private static void SetUpActionSpriteData(ThemeData themeData)
    {
        LemmingActionSpriteLayerData[] oneLayerTrueColor =
        [
            new LemmingActionSpriteLayerData(0, LemmingActionSpriteLayerColorType.TrueColor)
        ];
        LemmingActionSpriteLayerData[] fourLayers =
        [
            new LemmingActionSpriteLayerData(0, LemmingActionSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(1, LemmingActionSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(2, LemmingActionSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(3, LemmingActionSpriteLayerColorType.LemmingFootColor)
        ];
        LemmingActionSpriteLayerData[] fiveLayers =
        [
            new LemmingActionSpriteLayerData(0, LemmingActionSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(1, LemmingActionSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(2, LemmingActionSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(3, LemmingActionSpriteLayerColorType.LemmingFootColor),
            new LemmingActionSpriteLayerData(4, LemmingActionSpriteLayerColorType.TribePaintColor)
        ];
        LemmingActionSpriteLayerData[] fiveLayersTrueColor =
        [
            new LemmingActionSpriteLayerData(0, LemmingActionSpriteLayerColorType.TrueColor),
            new LemmingActionSpriteLayerData(1, LemmingActionSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(2, LemmingActionSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(3, LemmingActionSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(4, LemmingActionSpriteLayerColorType.LemmingFootColor)
        ];
        LemmingActionSpriteLayerData[] sixLayersTrueColor =
        [
            new LemmingActionSpriteLayerData(0, LemmingActionSpriteLayerColorType.TrueColor),
            new LemmingActionSpriteLayerData(1, LemmingActionSpriteLayerColorType.LemmingHairColor),
            new LemmingActionSpriteLayerData(2, LemmingActionSpriteLayerColorType.LemmingSkinColor),
            new LemmingActionSpriteLayerData(3, LemmingActionSpriteLayerColorType.LemmingBodyColor),
            new LemmingActionSpriteLayerData(4, LemmingActionSpriteLayerColorType.LemmingFootColor),
            new LemmingActionSpriteLayerData(5, LemmingActionSpriteLayerColorType.TribePaintColor)
        ];

        SetLemmingActionSpriteData(EngineConstants.WalkerActionId, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.ClimberActionId, new Point(8, 12), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.FloaterActionId, new Point(4, 16), fiveLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.BlockerActionId, new Point(5, 13), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.BuilderActionId, new Point(3, 13), sixLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.BasherActionId, new Point(8, 10), fiveLayers);
        SetLemmingActionSpriteData(EngineConstants.MinerActionId, new Point(7, 13), fiveLayers);
        SetLemmingActionSpriteData(EngineConstants.DiggerActionId, new Point(7, 12), fiveLayers);
        SetLemmingActionSpriteData(EngineConstants.PlatformerActionId, new Point(3, 13), sixLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.StackerActionId, new Point(3, 13), sixLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.FencerActionId, new Point(3, 10), sixLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.GliderActionId, new Point(5, 16), fiveLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.JumperActionId, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.SwimmerActionId, new Point(6, 8), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.ShimmierActionId, new Point(3, 8), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.LasererActionId, new Point(3, 10), fiveLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.SliderActionId, new Point(4, 11), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.FallerActionId, new Point(3, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.AscenderActionId, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.ShruggerActionId, new Point(3, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.DrownerActionId, new Point(5, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.HoisterActionId, new Point(5, 12), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.DehoisterActionId, new Point(5, 13), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.ReacherActionId, new Point(3, 9), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.DisarmerActionId, new Point(1, 11), fiveLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.ExiterActionId, new Point(2, 16), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.ExploderActionId, new Point(17, 21), oneLayerTrueColor);
        SetLemmingActionSpriteData(EngineConstants.OhNoerActionId, new Point(3, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.SplatterActionId, new Point(7, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.StonerActionId, new Point(17, 21), oneLayerTrueColor);
        SetLemmingActionSpriteData(EngineConstants.VaporiserActionId, new Point(5, 14), fiveLayersTrueColor);
        SetLemmingActionSpriteData(EngineConstants.RotateClockwiseActionId, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.RotateCounterclockwiseActionId, new Point(2, 10), fourLayers);
        SetLemmingActionSpriteData(EngineConstants.RotateHalfActionId, new Point(2, 10), fourLayers);

        return;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetLemmingActionSpriteData(int lemmingActionId, Point anchorPoint, LemmingActionSpriteLayerData[] layers)
        {
            themeData.LemmingActionSpriteData[lemmingActionId] = new LemmingActionSpriteData
            {
                LemmingActionId = lemmingActionId,
                AnchorPoint = anchorPoint,
                Layers = layers
            };
        }
    }

    private static void SetUpTribeColorData(ThemeData themeData)
    {
        var defaultSkinColor = new Color(0xF0, 0xD0, 0xD0);
        var defaultAcidLemmingFootColor = new Color(0x00, 0xF0, 0x00);
        var defaultWaterLemmingFootColor = new Color(0x00, 0xF0, 0xF0);
        var defaultZombieSkinColor = new Color(0x77, 0x77, 0x77);
        var defaultNeutralBodyColor = new Color(0x99, 0x99, 0x99);

        var defaultPaintColor = new Color(0xff, 0x00, 0xff);

        var tribe0HairColor = new Color(0x04, 0xB0, 0x00);
        var tribe0BodyColor = new Color(0x40, 0x44, 0xDF);

        var tribe1HairColor = new Color(0x00, 0xB0, 0xA9);
        var tribe1BodyColor = new Color(0xD5, 0x3F, 0xDE);

        var tribe2HairColor = new Color(0x00, 0x04, 0xB0);
        var tribe2BodyColor = new Color(0xDE, 0x3F, 0x46);

        var tribe3HairColor = new Color(0xAD, 0x00, 0xB0);
        var tribe3BodyColor = new Color(0xDE, 0xD1, 0x3F);

        var tribe4HairColor = new Color(0xB0, 0x00, 0x00);
        var tribe4BodyColor = new Color(0x4A, 0xDE, 0x3F);

        var tribe5HairColor = new Color(0xB0, 0xA9, 0x00);
        var tribe5BodyColor = new Color(0x3F, 0xDE, 0xD5);

        themeData.TribeColorData[0] = new TribeColorData(
            tribe0HairColor,
            tribe0BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe0BodyColor,
            tribe0HairColor,
            defaultNeutralBodyColor,

            defaultPaintColor
        );

        themeData.TribeColorData[1] = new TribeColorData(
            tribe1HairColor,
            tribe1BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe1BodyColor,
            tribe1HairColor,
            defaultNeutralBodyColor,

            defaultPaintColor
        );

        themeData.TribeColorData[2] = new TribeColorData
        (
            tribe2HairColor,
            tribe2BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe2BodyColor,
            tribe2HairColor,
            defaultNeutralBodyColor,

            defaultPaintColor
        );

        themeData.TribeColorData[3] = new TribeColorData
        (
            tribe3HairColor,
            tribe3BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe3BodyColor,
            tribe3HairColor,
            defaultNeutralBodyColor,

            defaultPaintColor
        );

        themeData.TribeColorData[4] = new TribeColorData
        (
            tribe4HairColor,
            tribe4BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe4BodyColor,
            tribe4HairColor,
            defaultNeutralBodyColor,

            defaultPaintColor
        );

        themeData.TribeColorData[5] = new TribeColorData
        (
            tribe5HairColor,
            tribe5BodyColor,
            defaultSkinColor,
            defaultAcidLemmingFootColor,
            defaultWaterLemmingFootColor,
            defaultZombieSkinColor,
            tribe5BodyColor,
            tribe5HairColor,
            defaultNeutralBodyColor,

            defaultPaintColor
        );
    }
}