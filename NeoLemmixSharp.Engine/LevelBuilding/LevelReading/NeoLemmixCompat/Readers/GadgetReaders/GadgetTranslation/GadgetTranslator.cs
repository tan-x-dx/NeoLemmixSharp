using System.Runtime.InteropServices;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Readers.GadgetReaders.GadgetTranslation;

public readonly ref partial struct GadgetTranslator
{
    private readonly LevelData _levelData;

    public GadgetTranslator(LevelData levelData)
    {
        _levelData = levelData;
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
            var archetype = GetMatchingArchetype(neoLemmixGadgetArchetypeData, prototype);

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

    /// <summary>
    /// Helper method to avoid using LINQ and hence avoiding heap allocations/boxing in iterators.
    /// </summary>
    private static NeoLemmixGadgetArchetypeData GetMatchingArchetype(
        Dictionary<string, NeoLemmixGadgetArchetypeData>.ValueCollection collection,
        NeoLemmixGadgetData prototype)
    {
        // ReSharper disable once NotDisposedResource
        var enumerator = collection.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var archetypeData = enumerator.Current;
            if (archetypeData.GadgetArchetypeId == prototype.GadgetArchetypeId)
                return archetypeData;
        }

        throw new InvalidOperationException("No matching archetype data found!");
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