using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public enum GadgetRenderMode
{
    NoRender,
    BehindTerrain,
    InFrontOfTerrain,
    OnlyOnTerrain
}

public static class GadgetRenderModeHelpers
{
    private const int NumberOfEnumValues = 4;

    public static GadgetRenderMode GetEnumValue(uint rawValue) => Helpers.GetEnumValue<GadgetRenderMode>(rawValue, NumberOfEnumValues);
}
