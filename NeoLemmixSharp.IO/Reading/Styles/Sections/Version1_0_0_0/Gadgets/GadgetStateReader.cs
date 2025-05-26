using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal readonly ref struct GadgetStateReader(RawStyleFileDataReader rawFileData)
{
    private readonly RawStyleFileDataReader _rawFileData = rawFileData;

    internal GadgetStateArchetypeData ReadStateData()
    {
        int offsetX = _rawFileData.Read16BitUnsignedInteger();
        int offsetY = _rawFileData.Read16BitUnsignedInteger();

        var hitBoxData = ReadHitBoxData();
        var regionData = ReadRegionData();
        var spriteData = ReadSpriteData();

        var result = new GadgetStateArchetypeData
        {
            HitBoxOffset = new Point(offsetX, offsetY),
            HitBoxData = hitBoxData,
            RegionData = regionData,
            SpriteData = spriteData,
        };

        return result;
    }

    private HitBoxData[] ReadHitBoxData()
    {
        int numberOfDefinedHitBoxes = _rawFileData.Read8BitUnsignedInteger();

        var result = CollectionsHelper.GetArrayForSize<HitBoxData>(numberOfDefinedHitBoxes);

        return result;
    }

    private HitBoxRegionData[] ReadRegionData()
    {
        int numberOfHitBoxRegions = _rawFileData.Read8BitUnsignedInteger();

        var result = CollectionsHelper.GetArrayForSize<HitBoxRegionData>(numberOfHitBoxRegions);

        return result;
    }

    private SpriteArchetypeData ReadSpriteData()
    {
        throw new NotImplementedException();
    }
}
