using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public sealed partial class GadgetTranslator
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
        _levelData.AllGadgetData.EnsureCapacity(gadgetDataSpan.Length);
        _levelData.AllGadgetBuilders.EnsureCapacity(gadgetArchetypes.Count);

        foreach (var prototype in gadgetDataSpan)
        {
            var archetype = neoLemmixGadgetArchetypeData
                .First(a => a.GadgetArchetypeId == prototype.GadgetArchetypeId);

            ProcessBuilder(
                archetype,
                prototype,
                id++);
        }
    }

    private void ProcessBuilder(
        NeoLemmixGadgetArchetypeData archetypeData,
        NeoLemmixGadgetData prototype,
        int gadgetId)
    {
        if (archetypeData.ResizeType != ResizeType.None)
        {
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
        DihedralTransformation.Simplify(
            prototype.FlipHorizontal,
            prototype.FlipVertical,
            prototype.Rotate,
            out var rotNum,
            out var flip);

        orientation = Orientation.AllItems[rotNum];
        facingDirection = flip
            ? FacingDirection.LeftInstance
            : FacingDirection.RightInstance;
    }
}