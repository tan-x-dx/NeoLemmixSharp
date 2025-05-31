using NeoLemmixSharp.IO.Data.Style;
using NeoLemmixSharp.IO.Data.Style.Theme;
using NeoLemmixSharp.IO.FileFormats;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0;

internal sealed class ThemeDataSectionWriter : StyleDataSectionWriter, IEqualityComparer<LemmingActionSpriteLayerData[]>
{
    private readonly StringIdLookup _stringIdLookup;

    public ThemeDataSectionWriter(StringIdLookup stringIdLookup)
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
        var themeData = styleData.ThemeData;

        if (StyleCache.DefaultStyleIdentifier.Equals(themeData.StyleIdentifier))
        {
            writer.Write((byte)0);
            return;
        }

        writer.Write((byte)1);

        WriteThemeData(writer, themeData);
    }

    private void WriteThemeData(RawStyleFileDataWriter writer, ThemeData themeData)
    {
        writer.Write(_stringIdLookup.GetStringId(themeData.StyleIdentifier.ToString()));

        WriteColorData(writer, themeData);
        WriteLemmingActionSpriteData(writer, themeData);
        WriteTribeColorData(writer, themeData.TribeColorData);
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
            writer.Write(buffer);
        }
    }

    private void WriteLemmingActionSpriteData(RawStyleFileDataWriter writer, ThemeData themeData)
    {
        var lemmingActionSpriteLayerDataLookup = GetSpriteLayerLookup();

        writer.Write((byte)lemmingActionSpriteLayerDataLookup.Count);

        WriteSpriteLayerData();
        WriteLemmingActionSpriteData();

        return;

        Dictionary<LemmingActionSpriteLayerData[], int> GetSpriteLayerLookup()
        {
            var result = new Dictionary<LemmingActionSpriteLayerData[], int>(8, this);

            foreach (var x in themeData.LemmingActionSpriteData)
            {
                var count = result.Count;

                ref var id = ref CollectionsMarshal.GetValueRefOrAddDefault(result, x.Layers, out var exists);
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
                writer.Write((byte)array.Length);

                foreach (var spriteLayerData in array)
                {
                    writer.Write((byte)spriteLayerData.Layer);
                    writer.Write((byte)spriteLayerData.ColorType);
                }
            }
        }

        void WriteLemmingActionSpriteData()
        {
            ReadOnlySpan<LemmingActionSpriteData> lemmingActionSpriteDataSpan = themeData.LemmingActionSpriteData;

            for (var i = 0; i < lemmingActionSpriteDataSpan.Length; i++)
            {
                var spriteData = lemmingActionSpriteDataSpan[i];
                var lemmingActionId = spriteData.LemmingActionId;
                FileWritingException.WriterAssert(lemmingActionId == i, "Lemming action id mismatch!");

                writer.Write((byte)lemmingActionId);
                writer.Write((byte)spriteData.AnchorPoint.X);
                writer.Write((byte)spriteData.AnchorPoint.Y);

                var correspondingId = lemmingActionSpriteLayerDataLookup[spriteData.Layers];

                writer.Write((byte)correspondingId);
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
            writer.Write(buffer);
        }
    }

    bool IEqualityComparer<LemmingActionSpriteLayerData[]>.Equals(LemmingActionSpriteLayerData[]? x, LemmingActionSpriteLayerData[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        if (x.Length != y.Length) return false;

        for (var i = 0; i < x.Length; i++)
        {
            if (!x[i].Equals(y[i]))
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
