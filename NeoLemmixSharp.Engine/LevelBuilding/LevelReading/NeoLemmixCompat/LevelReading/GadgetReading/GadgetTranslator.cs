using NeoLemmixSharp.Engine.LevelBuilding.Data;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.Data;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading.GadgetReading;

public readonly ref struct GadgetTranslator
{
    private readonly List<IGadgetBuilder> _gadgetBuilders;
    private readonly List<GadgetData> _gadgetDatas;

    public GadgetTranslator(LevelData levelData)
    {
        _gadgetBuilders = levelData.AllGadgetBuilders;
        _gadgetDatas = levelData.AllGadgetData;
    }

    public void TranslateNeoLemmixGadgets(
        Dictionary<string, NeoLemmixGadgetArchetypeData> gadgetArchetypes,
        List<NeoLemmixGadgetData> prototypes)
    {
        var gadgetDataSpan = CollectionsMarshal.AsSpan(prototypes);
        var neoLemmixGadgetArchetypeData = gadgetArchetypes.Values;

        foreach (var prototype in gadgetDataSpan)
        {
            var archetype = GetMatchingArchetype(neoLemmixGadgetArchetypeData, prototype);

            ProcessBuilder(archetype, prototype);
        }
    }

    private void ProcessBuilder(NeoLemmixGadgetArchetypeData archetypeData, NeoLemmixGadgetData prototype)
    {
        if (archetypeData.ResizeType != ResizeType.None)
        {
            ProcessResizeableGadgetBuilder(archetypeData, prototype);
            return;
        }

        if (archetypeData.Behaviour == NeoLemmixGadgetBehaviour.Entrance)
        {
            ProcessHatchGadgetBuilder(archetypeData, prototype);
            return;
        }

        ProcessStatefulGadgetBuilder(archetypeData, prototype);
    }

    private void ProcessResizeableGadgetBuilder(NeoLemmixGadgetArchetypeData archetypeData, NeoLemmixGadgetData prototype)
    {
        if (!prototype.Width.HasValue ||
            !prototype.Height.HasValue)
            throw new InvalidOperationException("Dimensions not specified for resizeable gadget!");

        var prototypeWidth = prototype.Width.Value;
        var prototypeHeight = prototype.Height.Value;

    }

    private void ProcessHatchGadgetBuilder(NeoLemmixGadgetArchetypeData archetypeData, NeoLemmixGadgetData prototype)
    {
        // return new HatchGadgetBuilder();
    }

    private void ProcessStatefulGadgetBuilder(NeoLemmixGadgetArchetypeData archetypeData, NeoLemmixGadgetData prototype)
    {
        var result = new StatefulGadgetBuilder
        {
            GadgetBuilderId = archetypeData.GadgetArchetypeId
        };

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
}