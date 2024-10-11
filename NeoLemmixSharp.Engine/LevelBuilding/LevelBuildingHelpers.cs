using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Level.Objectives.Requirements;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport;
using NeoLemmixSharp.Engine.Rendering.Viewport.BackgroundRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public static class LevelBuildingHelpers
{
    public static LevelTimer GetLevelTimer(LevelData levelData)
    {
        var primaryObjective = levelData.LevelObjectives.Find(lo => lo.LevelObjectiveId == 0)!;
        foreach (var requirement in primaryObjective.Requirements)
        {
            if (requirement is TimeRequirement timeRequirement)
                return new LevelTimer(timeRequirement.TimeLimitInSeconds);
        }

        return new LevelTimer();
    }

    public static IBackgroundRenderer GetBackgroundRenderer(
        LevelData levelData)
    {
        var backgroundData = levelData.LevelBackground;
        if (backgroundData is null)
            return new SolidColorBackgroundRenderer(new Color(24, 24, 60));

        if (backgroundData.IsSolidColor)
            return new SolidColorBackgroundRenderer(backgroundData.Color);

        return new SolidColorBackgroundRenderer(new Color(24, 24, 60));
    }

    public static List<IViewportObjectRenderer> GetSortedRenderables(
        List<IViewportObjectRenderer> behindTerrainSprites,
        List<IViewportObjectRenderer> inFrontOfTerrainSprites,
        TerrainRenderer terrainRenderer,
        List<IViewportObjectRenderer> lemmingSprites)
    {
        var listCapacity = behindTerrainSprites.Count +
                           1 + // Terrain renderer
                           inFrontOfTerrainSprites.Count +
                           lemmingSprites.Count;
        var result = new List<IViewportObjectRenderer>(listCapacity);

        var comparer = new ViewportObjectRendererComparer();

        behindTerrainSprites.Sort(comparer);
        inFrontOfTerrainSprites.Sort(comparer);
        lemmingSprites.Sort(comparer);

        result.AddRange(behindTerrainSprites);
        result.Add(terrainRenderer);
        result.AddRange(inFrontOfTerrainSprites);
        result.AddRange(lemmingSprites);

        var resultSpan = CollectionsMarshal.AsSpan(result);

        var i = 0;
        foreach (var renderer in resultSpan)
        {
            renderer.RendererId = i++;
        }

        return result;
    }

    private sealed class ViewportObjectRendererComparer : IComparer<IViewportObjectRenderer>
    {
        public int Compare(IViewportObjectRenderer? x, IViewportObjectRenderer? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (y is null) return 1;
            if (x is null) return -1;
            return x.ItemId.CompareTo(y.ItemId);
        }
    }
}