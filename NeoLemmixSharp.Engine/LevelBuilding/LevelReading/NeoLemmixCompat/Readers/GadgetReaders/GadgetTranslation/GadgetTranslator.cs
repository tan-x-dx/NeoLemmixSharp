using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public readonly ref partial struct GadgetTranslator
{
    private readonly LevelData _levelData;
    private readonly GraphicsDevice _graphicsDevice;

    public GadgetTranslator(LevelData levelData, GraphicsDevice graphicsDevice)
    {
        _levelData = levelData;
        _graphicsDevice = graphicsDevice;
    }

    public void TranslateNeoLemmixGadgets(
        Dictionary<string, NeoLemmixGadgetArchetypeData> gadgetArchetypes,
        List<NeoLemmixGadgetData> prototypes)
    {
        var gadgetDataSpan = CollectionsMarshal.AsSpan(prototypes);
        var neoLemmixGadgetArchetypeData = gadgetArchetypes.Values;

        var id = 0;
        _levelData.AllGadgetData.Capacity = gadgetDataSpan.Length;
        _levelData.AllGadgetBuilders.EnsureCapacity(gadgetArchetypes.Count);

        foreach (var prototype in gadgetDataSpan)
        {
            var archetype = GetMatchingArchetypeData(neoLemmixGadgetArchetypeData, prototype);

            ProcessBuilder(
                archetype,
                prototype,
                id++);
        }
    }

    private static NeoLemmixGadgetArchetypeData GetMatchingArchetypeData(
        Dictionary<string, NeoLemmixGadgetArchetypeData>.ValueCollection neoLemmixGadgetArchetypeData,
        NeoLemmixGadgetData prototype)
    {
        foreach (var archetypeData in neoLemmixGadgetArchetypeData)
        {
            if (archetypeData.GadgetArchetypeId == prototype.GadgetArchetypeId)
                return archetypeData;
        }

        throw new InvalidOperationException("No matching archetype data exists");
    }

    private void ProcessBuilder(
        NeoLemmixGadgetArchetypeData archetypeData,
        NeoLemmixGadgetData prototype,
        int gadgetId)
    {
        if (archetypeData.ResizeType != ResizeType.None)
        {
            if (archetypeData.Behaviour.IsOneWayArrows())
            {
                ProcessOneWayArrows(archetypeData, prototype, gadgetId);
                return;
            }

            ProcessResizeableGadgetBuilder(archetypeData, prototype, gadgetId);
            return;
        }

        if (archetypeData.Behaviour == NeoLemmixGadgetBehaviour.Entrance)
        {
            ProcessHatchGadgetBuilder(archetypeData, prototype, gadgetId);
            return;
        }

        ProcessStatefulGadgetBuilder(archetypeData, prototype, gadgetId);
    }

    private static void GetOrientationData(
        NeoLemmixGadgetData prototype,
        out Orientation orientation,
        out FacingDirection facingDirection)
    {
        var (rotNum, flip) = DihedralTransformation.Simplify(
            prototype.FlipHorizontal,
            prototype.FlipVertical,
            prototype.Rotate);

        orientation = Orientation.AllItems[rotNum];
        facingDirection = flip
            ? FacingDirection.LeftInstance
            : FacingDirection.RightInstance;
    }

    private static GadgetRenderMode GetGadgetRenderMode(NeoLemmixGadgetData prototype)
    {
        if (prototype.NoOverwrite)
            return GadgetRenderMode.BehindTerrain;
        return prototype.OnlyOnTerrain
            ? GadgetRenderMode.OnlyOnTerrain
            : GadgetRenderMode.InFrontOfTerrain;
    }
}