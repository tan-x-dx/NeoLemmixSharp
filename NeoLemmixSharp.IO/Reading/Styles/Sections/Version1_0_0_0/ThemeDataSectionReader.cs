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
        var maskColor = ReadRgbColor(reader);
        var minimap = ReadRgbColor(reader);
        var background = ReadRgbColor(reader);
        var oneWayArrows = ReadRgbColor(reader);
        var pickupBorder = ReadRgbColor(reader);
        var pickupInside = ReadRgbColor(reader);

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

        var result = new LemmingSpriteData(lemmingSpriteStyleIdentifier);

        ReadLemmingActionSpriteData(reader, originalStyleIdentifier, lemmingSpriteStyleIdentifier, result);
        ReadTribeColorData(reader, result._tribeColorData);

        return result;
    }

    private void ReadLemmingActionSpriteData(
        RawStyleFileDataReader reader,
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
            int numberOfSpriteLayerArchetypes = reader.Read8BitUnsignedInteger();
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
            int numberOfSpriteLayers = reader.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(numberOfSpriteLayers > 0, "No sprite layer data specified!");

            var spriteLayers = new LemmingActionSpriteLayerData[numberOfSpriteLayers];

            for (var i = 0; i < spriteLayers.Length; i++)
            {
                int layer = reader.Read8BitUnsignedInteger();
                uint rawLayerColorTypeData = reader.Read8BitUnsignedInteger();
                var colorType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawLayerColorTypeData);

                spriteLayers[i] = new LemmingActionSpriteLayerData(layer, colorType);
            }

            return spriteLayers;
        }

        LemmingActionSpriteData ReadLemmingActionSpriteData(int i)
        {
            int lemmingActionId = reader.Read8BitUnsignedInteger();
            FileReadingException.ReaderAssert(lemmingActionId == i, "Lemming action id mismatch!");

            int x = reader.Read8BitUnsignedInteger();
            int y = reader.Read8BitUnsignedInteger();

            int spriteLayerArchetypeId = reader.Read8BitUnsignedInteger();

            return new LemmingActionSpriteData
            {
                LemmingActionId = lemmingActionId,
                AnchorPoint = new Point(x, y),
                Layers = spriteLayerArchetypes[spriteLayerArchetypeId]
            };
        }
    }

    private static void ReadTribeColorData(
        RawStyleFileDataReader reader,
        TribeColorData[] tribeColorData)
    {
        for (var i = 0; i < tribeColorData.Length; i++)
        {
            tribeColorData[i] = ReadTribeColorData();
        }

        return;

        TribeColorData ReadTribeColorData()
        {
            var hairColor = ReadArgbColor(reader);
            var permanentSkillHairColor = ReadArgbColor(reader);
            var skinColor = ReadArgbColor(reader);
            var zombieSkinColor = ReadArgbColor(reader);
            var bodyColor = ReadArgbColor(reader);
            var permanentSkillBodyColor = ReadArgbColor(reader);
            var neutralBodyColor = ReadArgbColor(reader);
            var acidLemmingFootColor = ReadArgbColor(reader);
            var waterLemmingFootColor = ReadArgbColor(reader);
            var paintColor = ReadArgbColor(reader);

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

    private static Color ReadRgbColor(RawStyleFileDataReader reader)
    {
        var bytes = reader.ReadBytes(3);
        return ReadWriteHelpers.ReadRgbBytes(bytes);
    }

    private static Color ReadArgbColor(RawStyleFileDataReader reader)
    {
        var bytes = reader.ReadBytes(4);
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
