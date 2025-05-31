using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionReader : StyleDataSectionReader, IComparer<LemmingActionSpriteData>
{
    private readonly StringIdLookup _stringIdLookup;

    public ThemeDataSectionReader(StringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, false)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override void ReadSection(RawStyleFileDataReader rawFileData, StyleData styleData, int numberOfItemsInSection)
    {
        var themeData = ReadThemeData(rawFileData);

        styleData.ThemeData = themeData;
    }

    private ThemeData ReadThemeData(RawStyleFileDataReader rawFileData)
    {
        var maskColor = ReadRgbColor(rawFileData);
        var minimap = ReadRgbColor(rawFileData);
        var background = ReadRgbColor(rawFileData);
        var oneWayArrows = ReadRgbColor(rawFileData);
        var pickupBorder = ReadRgbColor(rawFileData);
        var pickupInside = ReadRgbColor(rawFileData);

        var lemmingSpriteData = ReadLemmingSpriteData(rawFileData);

        var themeData = new ThemeData
        {
            Mask = maskColor,
            Minimap = minimap,
            Background = background,
            OneWayArrows = oneWayArrows,
            PickupBorder = pickupBorder,
            PickupInside = pickupInside,

            LemmingSpriteData = lemmingSpriteData ?? StyleCache.DefaultStyleData.ThemeData.LemmingSpriteData
        };

        return themeData;
    }

    private LemmingSpriteData? ReadLemmingSpriteData(RawStyleFileDataReader rawFileData)
    {
        int customLemmingSpriteDataDefinition = rawFileData.Read8BitUnsignedInteger();
        if (customLemmingSpriteDataDefinition == 0)
            return null;

        int styleIdentifierId = rawFileData.Read16BitUnsignedInteger();
        var result = new LemmingSpriteData(new StyleIdentifier(_stringIdLookup[styleIdentifierId]));

        ReadLemmingActionSpriteData(rawFileData, result);
        ReadTribeColorData(rawFileData, result.TribeColorData);

        return result;
    }

    private void ReadLemmingActionSpriteData(RawStyleFileDataReader rawFileData, LemmingSpriteData lemmingSpriteData)
    {
        var spriteLayerArchetypes = ReadSpriteLayerArchetypes();

        for (var i = 0; i < LemmingActionConstants.NumberOfLemmingActions; i++)
        {
            lemmingSpriteData.LemmingActionSpriteData[i] = ReadLemmingActionSpriteData(i);
        }

        Span<LemmingActionSpriteData> allLemmingActionSpriteData = lemmingSpriteData.LemmingActionSpriteData;
        allLemmingActionSpriteData.Sort(this);

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
                var colorType = LemmingActionSpriteLayerColorTypeHelpers.GetEnumValue(rawLayerColorTypeData);

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

    private static void ReadTribeColorData(RawStyleFileDataReader rawFileData, TribeColorData[] tribeColorData)
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

        var hashX = x.LemmingActionId;
        var hashY = y.LemmingActionId;

        var gt = (hashX > hashY) ? 1 : 0;
        var lt = (hashX < hashY) ? 1 : 0;
        return gt - lt;
    }
}
