﻿using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public enum HitBoxType
{
    Empty,
    ResizableRectangular,
    Rectangular,
    PointSet
}

public static class HitBoxTypeHelpers
{
    public static HitBoxType GetGadgetHitBoxType(int rawValue)
    {
        var enumValue = (HitBoxType)rawValue;

        return enumValue switch
        {
            HitBoxType.Empty => HitBoxType.Empty,
            HitBoxType.ResizableRectangular => HitBoxType.ResizableRectangular,
            HitBoxType.Rectangular => HitBoxType.Rectangular,
            HitBoxType.PointSet => HitBoxType.PointSet,

            _ => Helpers.ThrowUnknownEnumValueException<HitBoxType>(rawValue)
        };
    }
}
