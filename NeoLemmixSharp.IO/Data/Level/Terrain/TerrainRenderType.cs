using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.IO.Data.Level.Terrain;

public enum TerrainRenderType
{
    Normal,
    Erase,
    NoOverwrite,

    VALUE_MAX
}

public static class TerrainRenderTypeHelpers
{
    private const uint NumberOfEnumValues = (uint)TerrainRenderType.VALUE_MAX;

    public static TerrainRenderType GetEnumValue(uint rawValue) => Helpers.GetEnumValue<TerrainRenderType>(rawValue, NumberOfEnumValues);
}
