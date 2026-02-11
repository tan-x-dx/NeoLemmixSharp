using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionWriter : StyleDataSectionWriter
{
    private readonly FileWriterStringIdLookup _stringIdLookup;

    public ThemeDataSectionWriter(FileWriterStringIdLookup stringIdLookup)
        : base(StyleFileSectionIdentifier.ThemeDataSection, true)
    {
        _stringIdLookup = stringIdLookup;
    }

    public override ushort CalculateNumberOfItemsInSection(StyleData styleData)
    {
        return 1;
    }

    public override void WriteSection(RawStyleFileDataWriter writer, StyleData styleData)
    {
        WriteStringData(writer, styleData);

        var themeData = styleData.ThemeData;

        WriteColorData(writer, themeData);
        WriteLemmingSpriteData(writer, styleData.Identifier, themeData.LemmingSpriteData);
    }

    private void WriteStringData(RawStyleFileDataWriter writer, StyleData styleData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(styleData.Name));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(styleData.Author));
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(styleData.Description));
    }

    private static void WriteColorData(RawStyleFileDataWriter writer, ThemeData themeData)
    {
        writer.WriteRgbColor(themeData.Mask);
        writer.WriteRgbColor(themeData.Minimap);
        writer.WriteRgbColor(themeData.Background);
        writer.WriteRgbColor(themeData.OneWayArrows);
        writer.WriteRgbColor(themeData.PickupBorder);
        writer.WriteRgbColor(themeData.PickupInside);
    }

    private void WriteLemmingSpriteData(RawStyleFileDataWriter writer, StyleIdentifier originalStyleIdentifier, LemmingSpriteData lemmingSpriteData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(lemmingSpriteData.LemmingSpriteStyleIdentifier));

        if (lemmingSpriteData.LemmingSpriteStyleIdentifier == originalStyleIdentifier)
        {
            WriteLemmingActionSpriteData(writer, lemmingSpriteData);
        }

        WriteTribeColorData(writer, lemmingSpriteData.TribeColorData);
    }

    private static void WriteLemmingActionSpriteData(RawStyleFileDataWriter writer, LemmingSpriteData lemmingSpriteData)
    {
        var lemmingActionSpriteDataSpan = lemmingSpriteData.LemmingActionSpriteData;

        for (var i = 0; i < lemmingActionSpriteDataSpan.Length; i++)
        {
            var spriteData = lemmingActionSpriteDataSpan[i];
            var lemmingActionId = spriteData.LemmingActionId;
            FileWritingException.WriterAssert(lemmingActionId == i, "Lemming action id mismatch!");

            writer.Write8BitUnsignedInteger((byte)lemmingActionId);
            writer.Write8BitUnsignedInteger((byte)spriteData.AnchorPoint.X);
            writer.Write8BitUnsignedInteger((byte)spriteData.AnchorPoint.Y);

            var spriteLayerSpan = spriteData.Layers;
            writer.Write8BitUnsignedInteger((byte)spriteLayerSpan.Length);

            foreach (var spriteLayerData in spriteLayerSpan)
            {
                writer.Write8BitUnsignedInteger((byte)spriteLayerData.Layer);
                writer.Write8BitUnsignedInteger((byte)spriteLayerData.ColorType);
            }
        }
    }

    private static void WriteTribeColorData(RawStyleFileDataWriter writer, ReadOnlySpan<TribeColorData> tribeColorData)
    {
        for (var i = 0; i < tribeColorData.Length; i++)
        {
            ref readonly var colorData = ref tribeColorData[i];

            writer.WriteArgbColor(colorData.HairColor);
            writer.WriteArgbColor(colorData.PermanentSkillHairColor);
            writer.WriteArgbColor(colorData.SkinColor);
            writer.WriteArgbColor(colorData.ZombieSkinColor);
            writer.WriteArgbColor(colorData.BodyColor);
            writer.WriteArgbColor(colorData.PermanentSkillBodyColor);
            writer.WriteArgbColor(colorData.NeutralBodyColor);
            writer.WriteArgbColor(colorData.AcidLemmingFootColor);
            writer.WriteArgbColor(colorData.WaterLemmingFootColor);
            writer.WriteArgbColor(colorData.PaintColor);
        }
    }
}
