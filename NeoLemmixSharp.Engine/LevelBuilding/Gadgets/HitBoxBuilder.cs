﻿using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets;

public static class HitBoxBuilder
{
    public static BitArrayDictionary<Orientation.OrientationHasher, BitBuffer32, Orientation, IHitBoxRegion> BuildHitBoxLookup(
        GadgetStateArchetypeData state,
        GadgetBounds gadgetBounds)
    {
        var result = Orientation.CreateBitArrayDictionary<IHitBoxRegion>();

        foreach (var hitBoxRegionData in state.RegionData)
        {
            var hitBoxRegion = BuildHitBoxRegion(hitBoxRegionData, gadgetBounds);

            if (hitBoxRegion is not null)
            {
                result.Add(hitBoxRegionData.Orientation, hitBoxRegion);
            }
        }

        return result;
    }

    private static IHitBoxRegion? BuildHitBoxRegion(
        HitBoxRegionData hitBoxRegionData,
        GadgetBounds gadgetBounds) => hitBoxRegionData.HitBoxType switch
        {
            HitBoxType.Empty => null,
            HitBoxType.ResizableRectangular => BuildResizableRectangularHitBoxRegion(hitBoxRegionData, gadgetBounds),
            HitBoxType.Rectangular => BuildRectangularHitBoxRegion(hitBoxRegionData),
            HitBoxType.PointSet => BuildPointSetHitBoxRegion(hitBoxRegionData),

            _ => Helpers.ThrowUnknownEnumValueException<HitBoxType, IHitBoxRegion>(hitBoxRegionData.HitBoxType),
        };

    private static ResizableRectangularHitBoxRegion BuildResizableRectangularHitBoxRegion(
        HitBoxRegionData hitBoxRegionData,
        GadgetBounds gadgetBounds)
    {
        Debug.Assert(hitBoxRegionData.HitBoxDefinitionData.Length == 2);

        var p0 = hitBoxRegionData.HitBoxDefinitionData[0];
        var p1 = hitBoxRegionData.HitBoxDefinitionData[1];

        return new ResizableRectangularHitBoxRegion(gadgetBounds, p0.X, p0.Y, p1.X, p1.Y);
    }

    private static RectangularHitBoxRegion BuildRectangularHitBoxRegion(
        HitBoxRegionData hitBoxRegionData)
    {
        Debug.Assert(hitBoxRegionData.HitBoxDefinitionData.Length == 2);

        return new RectangularHitBoxRegion(hitBoxRegionData.HitBoxDefinitionData[0], hitBoxRegionData.HitBoxDefinitionData[1]);
    }

    private static PointSetHitBoxRegion BuildPointSetHitBoxRegion(
        HitBoxRegionData hitBoxRegionData)
    {
        return new PointSetHitBoxRegion(hitBoxRegionData.HitBoxDefinitionData);
    }
}
