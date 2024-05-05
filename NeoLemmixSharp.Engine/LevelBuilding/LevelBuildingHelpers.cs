using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LevelBuildingHelpers
{
    public static IBackgroundRenderer GetBackgroundRenderer(
        LevelData levelData,
        Viewport viewport)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
            return new SolidColorBackgroundRenderer(viewport, Color.Black);

        if (backgroundData.IsSolidColor)
            return new SolidColorBackgroundRenderer(viewport, backgroundData.Color);

        throw new NotImplementedException();
    }
}