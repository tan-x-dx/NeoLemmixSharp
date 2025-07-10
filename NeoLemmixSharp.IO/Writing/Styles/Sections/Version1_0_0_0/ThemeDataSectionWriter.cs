using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using NeoLemmixSharp.IO.Util;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionWriter : StyleDataSectionWriter, IEqualityComparer<LemmingActionSpriteLayerData[]>
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
        Span<byte> bytes = [0, 0, 0];

        WriteRgbColor(themeData.Minimap, bytes);
        WriteRgbColor(themeData.Background, bytes);
        WriteRgbColor(themeData.OneWayArrows, bytes);
        WriteRgbColor(themeData.PickupBorder, bytes);
        WriteRgbColor(themeData.PickupInside, bytes);

        return;

        void WriteRgbColor(Color color, Span<byte> buffer)
        {
            ReadWriteHelpers.WriteRgbBytes(color, buffer);
            writer.WriteBytes(buffer);
        }
    }

    private void WriteLemmingSpriteData(RawStyleFileDataWriter writer, StyleIdentifier originalStyleIdentifier, LemmingSpriteData lemmingSpriteData)
    {
        writer.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(lemmingSpriteData.LemmingSpriteStyleIdentifier));

        if (lemmingSpriteData.LemmingSpriteStyleIdentifier == originalStyleIdentifier)
        {
            WriteLemmingActionSpriteData(writer, lemmingSpriteData);
        }

        WriteTribeColorData(writer, lemmingSpriteData._tribeColorData);
    }

    private void WriteLemmingActionSpriteData(RawStyleFileDataWriter writer, LemmingSpriteData lemmingSpriteData)
    {
        var lemmingActionSpriteLayerDataLookup = GetSpriteLayerLookup();

        writer.Write8BitUnsignedInteger((byte)lemmingActionSpriteLayerDataLookup.Count);

        WriteSpriteLayerData();
        WriteLemmingActionSpriteData();

        return;

        Dictionary<LemmingActionSpriteLayerData[], int> GetSpriteLayerLookup()
        {
            var result = new Dictionary<LemmingActionSpriteLayerData[], int>(8, this);

            foreach (var lemmingActionSprite in lemmingSpriteData.LemmingActionSpriteData)
            {
                var count = result.Count;

                ref var id = ref CollectionsMarshal.GetValueRefOrAddDefault(result, lemmingActionSprite.Layers, out var exists);
                if (!exists)
                {
                    id = count;
                }
            }

            return result;
        }

        void WriteSpriteLayerData()
        {
            foreach (var kvp in lemmingActionSpriteLayerDataLookup.OrderBy(x => x.Value))
            {
                var array = kvp.Key;
                writer.Write8BitUnsignedInteger((byte)array.Length);

                foreach (var spriteLayerData in array)
                {
                    writer.Write8BitUnsignedInteger((byte)spriteLayerData.Layer);
                    writer.Write8BitUnsignedInteger((byte)spriteLayerData.ColorType);
                }
            }
        }

        void WriteLemmingActionSpriteData()
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

                var correspondingId = lemmingActionSpriteLayerDataLookup[spriteData.Layers];

                writer.Write8BitUnsignedInteger((byte)correspondingId);
            }
        }
    }

    private static void WriteTribeColorData(RawStyleFileDataWriter writer, TribeColorData[] tribeColorData)
    {
        for (var i = 0; i < tribeColorData.Length; i++)
        {
            ref var colorData = ref tribeColorData[i];

            WriteTribeColorDatum(in colorData);
        }

        return;

        void WriteTribeColorDatum(in TribeColorData tribeColorDatum)
        {
            Span<byte> bytes = [0, 0, 0, 0];

            WriteArgbColor(tribeColorDatum.HairColor, bytes);
            WriteArgbColor(tribeColorDatum.PermanentSkillHairColor, bytes);
            WriteArgbColor(tribeColorDatum.SkinColor, bytes);
            WriteArgbColor(tribeColorDatum.ZombieSkinColor, bytes);
            WriteArgbColor(tribeColorDatum.BodyColor, bytes);
            WriteArgbColor(tribeColorDatum.PermanentSkillBodyColor, bytes);
            WriteArgbColor(tribeColorDatum.NeutralBodyColor, bytes);
            WriteArgbColor(tribeColorDatum.AcidLemmingFootColor, bytes);
            WriteArgbColor(tribeColorDatum.WaterLemmingFootColor, bytes);
            WriteArgbColor(tribeColorDatum.PaintColor, bytes);
        }

        void WriteArgbColor(Color color, Span<byte> buffer)
        {
            ReadWriteHelpers.WriteArgbBytes(color, buffer);
            writer.WriteBytes(buffer);
        }
    }

    bool IEqualityComparer<LemmingActionSpriteLayerData[]>.Equals(LemmingActionSpriteLayerData[]? x, LemmingActionSpriteLayerData[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        if (x.Length != y.Length) return false;

        for (var i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i])
                return false;
        }

        return true;
    }

    int IEqualityComparer<LemmingActionSpriteLayerData[]>.GetHashCode([DisallowNull] LemmingActionSpriteLayerData[] array)
    {
        var hashCode = new HashCode();
        foreach (var item in array)
        {
            hashCode.Add(item);
        }

        return hashCode.ToHashCode();
    }
}
