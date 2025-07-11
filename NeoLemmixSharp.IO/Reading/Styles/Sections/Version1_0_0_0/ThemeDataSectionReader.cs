using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionReader : StyleDataSectionReader, IComparer<LemmingActionSpriteData>
{
    private readonly FileReaderStringIdLookup _stringIdLookup;

    public ThemeDataSectionReader(FileReaderStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        ReadStringData(rawFileData, styleData);

        var themeData = ReadThemeData(rawFileData, styleData.Identifier);

        styleData.ThemeData = themeData;
    }

    private void ReadStringData(RawStyleFileDataReader rawFileData, StyleData styleData)
    {
        var styleNameId = rawFileData.Read16BitUnsignedInteger();
        var authorId = rawFileData.Read16BitUnsignedInteger();
        var descriptionId = rawFileData.Read16BitUnsignedInteger();

        styleData.Name = _stringIdLookup[styleNameId];
        styleData.Author = _stringIdLookup[authorId];
        styleData.Description = _stringIdLookup[descriptionId];
    }

    private ThemeData ReadThemeData(RawStyleFileDataReader rawFileData, StyleIdentifier originalStyleIdentifier)
    {
        var maskColor = ReadRgbColor(rawFileData);
        var minimap = ReadRgbColor(rawFileData);
        var background = ReadRgbColor(rawFileData);
        var oneWayArrows = ReadRgbColor(rawFileData);
        var pickupBorder = ReadRgbColor(rawFileData);
        var pickupInside = ReadRgbColor(rawFileData);

        var lemmingSpriteData = ReadLemmingSpriteData(rawFileData, originalStyleIdentifier);

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

    private LemmingSpriteData ReadLemmingSpriteData(RawStyleFileDataReader rawFileData, StyleIdentifier originalStyleIdentifier)
    {
        int lemmingSpriteStyleIdentifierId = rawFileData.Read16BitUnsignedInteger();
        var lemmingSpriteStyleIdentifier = new StyleIdentifier(_stringIdLookup[lemmingSpriteStyleIdentifierId]);

        var result = new LemmingSpriteData(lemmingSpriteStyleIdentifier);

        ReadLemmingActionSpriteData(rawFileData, originalStyleIdentifier, lemmingSpriteStyleIdentifier, result);
        ReadTribeColorData(rawFileData, result._tribeColorData);

        return result;
    }

    private void ReadLemmingActionSpriteData(
        RawStyleFileDataReader rawFileData,
        StyleIdentifier originalStyleIdentifier,
        StyleIdentifier lemmingSpriteStyleIdentifier,
        LemmingSpriteData lemmingSpriteData)
    {
        if (originalStyleIdentifier != lemmingSpriteStyleIdentifier)
        {
            var styleFormatPair = new StyleFormatPair(lemmingSpriteStyleIdentifier, FileFormatType.Default);
            var deferredStyle = StyleCache.GetOrLoadStyleData(styleFormatPair);
            deferredStyle.ThemeData.LemmingSpriteData.LemmingActionSpriteData.CopyTo(lemmingSpriteData._lemmingActionSpriteData);
            return;
        }

        var spriteLayerArchetypes = ReadSpriteLayerArchetypes();

        for (var i = 0; i < LemmingActionConstants.NumberOfLemmingActions; i++)
        {
            lemmingSpriteData._lemmingActionSpriteData[i] = ReadLemmingActionSpriteData(i);
        }

        new Span<LemmingActionSpriteData>(lemmingSpriteData._lemmingActionSpriteData).Sort(this);

        return;

        LemmingActionSpriteLayerData[][] ReadSpriteLayerArchetypes()
        {
            int numberOfSpriteLayerArchetypes = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(numberOfSpriteLayerArchetypes > 0, "No sprite layer data specified!");

            var spriteLayerArchetypes = new LemmingActionSpriteLayerData[numberOfSpriteLayerArchetypes][];

            for (var i = 0; i < spriteLayerArchetypes.Length; i++)
            {
                spriteLayerArchetypes[i] = ReadSpriteLayerData();
            }

            return spriteLayerArchetypes;
        }

        LemmingActionSpriteLayerData[] ReadSpriteLayerData()
        {
            int numberOfSpriteLayers = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(numberOfSpriteLayers > 0, "No sprite layer data specified!");

            var spriteLayers = new LemmingActionSpriteLayerData[numberOfSpriteLayers];

            for (var i = 0; i < spriteLayers.Length; i++)
            {
                int layer = rawFileData.Read8BitUnsignedInteger();
                uint rawLayerColorTypeData = rawFileData.Read8BitUnsignedInteger();
                var colorType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawLayerColorTypeData);

                spriteLayers[i] = new LemmingActionSpriteLayerData(layer, colorType);
            }

            return spriteLayers;
        }

        LemmingActionSpriteData ReadLemmingActionSpriteData(int i)
        {
            int lemmingActionId = rawFileData.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(lemmingActionId == i, "Lemming action id mismatch!");

            int x = rawFileData.Read8BitUnsignedInteger();
            int y = rawFileData.Read8BitUnsignedInteger();

            int spriteLayerArchetypeId = rawFileData.Read8BitUnsignedInteger();

            return new LemmingActionSpriteData
            {
                LemmingActionId = lemmingActionId,
                AnchorPoint = new Point(x, y),
                Layers = spriteLayerArchetypes[spriteLayerArchetypeId]
            };
        }
    }

    private static void ReadTribeColorData(
        RawStyleFileDataReader rawFileData,
        TribeColorData[] tribeColorData)
    {
        for (var i = 0; i < tribeColorData.Length; i++)
        {
            tribeColorData[i] = ReadTribeColorData();
        }

        return;

        TribeColorData ReadTribeColorData()
        {
            var hairColor = ReadArgbColor(rawFileData);
            var permanentSkillHairColor = ReadArgbColor(rawFileData);
            var skinColor = ReadArgbColor(rawFileData);
            var zombieSkinColor = ReadArgbColor(rawFileData);
            var bodyColor = ReadArgbColor(rawFileData);
            var permanentSkillBodyColor = ReadArgbColor(rawFileData);
            var neutralBodyColor = ReadArgbColor(rawFileData);
            var acidLemmingFootColor = ReadArgbColor(rawFileData);
            var waterLemmingFootColor = ReadArgbColor(rawFileData);
            var paintColor = ReadArgbColor(rawFileData);

            return new TribeColorData(
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
    }

    private static Color ReadRgbColor(RawStyleFileDataReader rawFileData)
    {
        var bytes = rawFileData.ReadBytes(3);
        return ReadWriteHelpers.ReadRgbBytes(bytes);
    }

    private static Color ReadArgbColor(RawStyleFileDataReader rawFileData)
    {
        var bytes = rawFileData.ReadBytes(4);
        return ReadWriteHelpers.ReadArgbBytes(bytes);
    }

    int IComparer<LemmingActionSpriteData>.Compare(LemmingActionSpriteData? x, LemmingActionSpriteData? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        var gt = (x.LemmingActionId > y.LemmingActionId) ? 1 : 0;
        var lt = (x.LemmingActionId < y.LemmingActionId) ? 1 : 0;
        return gt - lt;
    }
}
