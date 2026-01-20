using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBoxGadget;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;

public static class HitBoxBuilder
{
    public static BitArrayDictionary<Orientation.OrientationHasher, BitBuffer32, Orientation, HitBoxRegion> BuildHitBoxLookup(
        HitBoxGadgetStateArchetypeData gadgetStateArchetypeData,
        GadgetBounds gadgetBounds)
    {
        var result = Orientation.CreateBitArrayDictionary<HitBoxRegion>();

        foreach (var hitBoxRegionData in gadgetStateArchetypeData.RegionData)
        {
            var hitBoxRegion = BuildHitBoxRegion(hitBoxRegionData, gadgetBounds);

            if (hitBoxRegion is not null)
            {
                result.Add(hitBoxRegionData.Orientation, hitBoxRegion);
            }
        }

        return result;
    }

    private static HitBoxRegion? BuildHitBoxRegion(
        HitBoxRegionData hitBoxRegionData,
        GadgetBounds gadgetBounds) => hitBoxRegionData.HitBoxType switch
        {
            HitBoxType.Empty => null,
            HitBoxType.ResizableRectangular => BuildResizableRectangularHitBoxRegion(hitBoxRegionData, gadgetBounds),
            HitBoxType.Rectangular => BuildRectangularHitBoxRegion(hitBoxRegionData),
            HitBoxType.PointSet => BuildPointSetHitBoxRegion(hitBoxRegionData),

            _ => Helpers.ThrowUnknownEnumValueException<HitBoxType, HitBoxRegion>(hitBoxRegionData.HitBoxType),
        };

    private static ResizableRectangularHitBoxRegion BuildResizableRectangularHitBoxRegion(
        HitBoxRegionData hitBoxRegionData,
        GadgetBounds gadgetBounds)
    {
        Debug.Assert(hitBoxRegionData.HitBoxDefinitionData.Length == 2);

        var p0 = hitBoxRegionData.HitBoxDefinitionData.At(0);
        var p1 = hitBoxRegionData.HitBoxDefinitionData.At(1);

        return new ResizableRectangularHitBoxRegion(gadgetBounds, p0.X, p0.Y, p1.X, p1.Y);
    }

    private static RectangularHitBoxRegion BuildRectangularHitBoxRegion(
        HitBoxRegionData hitBoxRegionData)
    {
        Debug.Assert(hitBoxRegionData.HitBoxDefinitionData.Length == 2);

        return new RectangularHitBoxRegion(
            hitBoxRegionData.HitBoxDefinitionData.At(0),
            hitBoxRegionData.HitBoxDefinitionData.At(1));
    }

    private static PointSetHitBoxRegion BuildPointSetHitBoxRegion(
        HitBoxRegionData hitBoxRegionData)
    {
        return new PointSetHitBoxRegion(hitBoxRegionData.HitBoxDefinitionData);
    }
}
