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
    public static GadgetRenderMode GetGadgetRenderMode(int rawValue)
    {
        var enumValue = (GadgetRenderMode)rawValue;

        return enumValue switch
        {
            GadgetRenderMode.NoRender => GadgetRenderMode.NoRender,
            GadgetRenderMode.BehindTerrain => GadgetRenderMode.BehindTerrain,
            GadgetRenderMode.InFrontOfTerrain => GadgetRenderMode.InFrontOfTerrain,
            GadgetRenderMode.OnlyOnTerrain => GadgetRenderMode.OnlyOnTerrain,

            _ => Helpers.ThrowUnknownEnumValueException<GadgetRenderMode>(rawValue)
        };
    }
}
