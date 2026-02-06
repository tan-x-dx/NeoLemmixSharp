using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionReader : StyleDataSectionReader
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public ThemeDataSectionReader(FileReaderStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader reader, StyleData styleData, int numberOfItemsInSection)
    {
        ReadStringData(reader, styleData);

        var themeData = ReadThemeData(reader, styleData.Identifier);

        styleData.ThemeData = themeData;
    }

    private void ReadStringData(RawStyleFileDataReader reader, StyleData styleData)
    {
        var styleNameId = reader.Read16BitUnsignedInteger();
        var authorId = reader.Read16BitUnsignedInteger();
        var descriptionId = reader.Read16BitUnsignedInteger();

        styleData.Name = _stringIdLookup[styleNameId];
        styleData.Author = _stringIdLookup[authorId];
        styleData.Description = _stringIdLookup[descriptionId];
    }

    private ThemeData ReadThemeData(RawStyleFileDataReader reader, StyleIdentifier originalStyleIdentifier)
    {
        var maskColor = reader.ReadRgbColor();
        var minimap = reader.ReadRgbColor();
        var background = reader.ReadRgbColor();
        var oneWayArrows = reader.ReadRgbColor();
        var pickupBorder = reader.ReadRgbColor();
        var pickupInside = reader.ReadRgbColor();

        var lemmingSpriteData = ReadLemmingSpriteData(reader, originalStyleIdentifier);

        var themeData = new ThemeData
        {
            Mask = maskColor,
            Minimap = minimap,
            Background = background,
            OneWayArrows = oneWayArrows,
            PickupBorder = pickupBorder,
            PickupInside = pickupInside,

            LemmingSpriteData = lemmingSpriteData
        };

        return themeData;
    }

    private LemmingSpriteData ReadLemmingSpriteData(RawStyleFileDataReader reader, StyleIdentifier originalStyleIdentifier)
    {
        int lemmingSpriteStyleIdentifierId = reader.Read16BitUnsignedInteger();
        var lemmingSpriteStyleIdentifier = new StyleIdentifier(_stringIdLookup[lemmingSpriteStyleIdentifierId]);

        var lemmingActionSpriteData = ReadLemmingActionSpriteData(reader, originalStyleIdentifier, lemmingSpriteStyleIdentifier);
        var tribeColorData = ReadTribeColorData(reader);

        return new LemmingSpriteData(lemmingSpriteStyleIdentifier, tribeColorData, lemmingActionSpriteData);
    }

    private static LemmingActionSpriteData[] ReadLemmingActionSpriteData(
        RawStyleFileDataReader reader,
        StyleIdentifier originalStyleIdentifier,
        StyleIdentifier lemmingSpriteStyleIdentifier)
    {
        if (originalStyleIdentifier != lemmingSpriteStyleIdentifier)
        {
            var styleFormatPair = new StyleFormatPair(lemmingSpriteStyleIdentifier, FileFormatType.Default);
            var deferredStyle = StyleCache.GetOrLoadStyleData(styleFormatPair);
            return deferredStyle.ThemeData.LemmingSpriteData.CloneLemmingActionSpriteData();
        }

        var result = new LemmingActionSpriteData[LemmingActionConstants.NumberOfLemmingActions];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadLemmingActionSpriteData(i);
        }

        return result;

        LemmingActionSpriteData ReadLemmingActionSpriteData(int i)
        {
            int lemmingActionId = reader.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(lemmingActionId == i, "Lemming action id mismatch!");

            int x = reader.Read8BitUnsignedInteger();
            int y = reader.Read8BitUnsignedInteger();

            var layers = ReadSpriteLayers();

            return new LemmingActionSpriteData(lemmingActionId, new Point(x, y), layers);
        }

        LemmingActionSpriteLayerData[] ReadSpriteLayers()
        {
            int numberOfLayers = reader.Read8BitUnsignedInteger();

            var result = Helpers.GetArrayForSize<LemmingActionSpriteLayerData>(numberOfLayers);

            for (var i = 0; i < result.Length; i++)
            {
                int layer = reader.Read8BitUnsignedInteger();
                uint rawLayerColorTypeData = reader.Read8BitUnsignedInteger();
                var colorType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawLayerColorTypeData);

                result[i] = new LemmingActionSpriteLayerData(layer, colorType);
            }

            return result;
        }
    }

    private static TribeColorData[] ReadTribeColorData(RawStyleFileDataReader reader)
    {
        var result = new TribeColorData[EngineConstants.MaxNumberOfTribes];
        for (var i = 0; i < result.Length; i++)
        {
            var hairColor = reader.ReadArgbColor();
            var permanentSkillHairColor = reader.ReadArgbColor();
            var skinColor = reader.ReadArgbColor();
            var zombieSkinColor = reader.ReadArgbColor();
            var bodyColor = reader.ReadArgbColor();
            var permanentSkillBodyColor = reader.ReadArgbColor();
            var neutralBodyColor = reader.ReadArgbColor();
            var acidLemmingFootColor = reader.ReadArgbColor();
            var waterLemmingFootColor = reader.ReadArgbColor();
            var paintColor = reader.ReadArgbColor();

            result[i] = new TribeColorData(
                hairColor,
                permanentSkillHairColor,
                skinColor,
                zombieSkinColor,
                bodyColor,
                permanentSkillBodyColor,
                neutralBodyColor,
                acidLemmingFootColor,
                waterLemmingFootColor,
                paintColor);
        }

        return result;
    }
}
