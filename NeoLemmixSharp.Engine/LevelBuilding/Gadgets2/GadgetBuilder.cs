using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HatchGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.HitBoxGadgets;
using NeoLemmixSharp.Engine.LevelBuilding.Gadgets2.LogicGateGadgets;
using NeoLemmixSharp.IO.Data;
using NeoLemmixSharp.IO.Data.Level;
using NeoLemmixSharp.IO.Data.Level.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.Engine.LevelBuilding.Gadgets2;

public sealed class GadgetBuilder
{
    /// <summary>
    /// Assumption: Each gadget instance will have about this many states defined.
    /// </summary>
    private const int GadgetStateCapacityMultiplier = 3;
    /// <summary>
    /// Assumption: Each gadget state will have about this many triggers defined.
    /// </summary>
    private const int GadgetTriggerCapacityMultiplier = 4;
    /// <summary>
    /// Assumption: Each gadget state will have about this many behvaviours defined.
    /// </summary>
    private const int GadgetBehaviourCapacityMultiplier = 4;

    private readonly LevelData _levelData;
    private readonly List<GadgetBase> _gadgets;
    private readonly List<GadgetTrigger> _gadgetTriggers;
    private readonly List<GadgetBehaviour> _gadgetBehaviours;

    public RawArray GadgetDataBuffer { get; }

    public GadgetBuilder(
        LevelData levelData,
        SafeBufferAllocator safeBufferAllocator,
        int numberOfLemmingsInLevel)
    {
        _levelData = levelData;

        var numberOfGadgetInstances = _levelData.AllGadgetInstanceData.Count;

        // This list's required capacity is known - perfect fit
        _gadgets = new List<GadgetBase>(numberOfGadgetInstances);

        // Preallocate these lists with large(ish) initial capacities to reduce realloactions further down the line
        _gadgetTriggers = new List<GadgetTrigger>(numberOfGadgetInstances * GadgetStateCapacityMultiplier * GadgetTriggerCapacityMultiplier);
        _gadgetBehaviours = new List<GadgetBehaviour>(numberOfGadgetInstances * GadgetStateCapacityMultiplier * GadgetBehaviourCapacityMultiplier);

        var gadgetDataBufferLength = CalculateGadgetBufferSizes(levelData, numberOfLemmingsInLevel);

        GadgetDataBuffer = safeBufferAllocator.AllocateRawArray(gadgetDataBufferLength);
    }

    private static int CalculateGadgetBufferSizes(
        LevelData levelData,
        int numberOfLemmingsInLevel)
    {
        var lemmingTrackerByteRequirement = BitArrayHelpers.CalculateBitArrayBufferLength(numberOfLemmingsInLevel) * sizeof(ulong);
        var result = 0;

        var gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(levelData);

        foreach (var gadgetInstanceData in levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(gadgetInstanceData.StyleIdentifier, gadgetInstanceData.PieceIdentifier);
            var gadgetArchetypeData = gadgetArchetypeDataLookup[stylePiecePair];

            if (gadgetArchetypeData.SpecificationData.GadgetType != gadgetInstanceData.SpecificationData.GadgetType)
                throw new Exception("GadgetType mismatch!");

            var numberOfBytesNeededForThisGadget = CalculateNumberOfBytesNeededForThisGadget(gadgetArchetypeData, gadgetInstanceData, lemmingTrackerByteRequirement);

            // Ensure 8-byte alignment for the next gadget
            numberOfBytesNeededForThisGadget--;
            numberOfBytesNeededForThisGadget |= sizeof(ulong) - 1;
            numberOfBytesNeededForThisGadget++;

            result += numberOfBytesNeededForThisGadget;
        }

        return result;
    }

    private static int CalculateNumberOfBytesNeededForThisGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        int lemmingTrackerByteRequirement)
    {
        var result =
            GadgetBounds.GadgetBoundsDataSize + // A gadget will always have bounds
            sizeof(int); // A gadget will always have a state index

        result += gadgetArchetypeData.SpecificationData.CalculateExtraNumberOfBytesNeededForSnapshotting();
        result += gadgetInstanceData.SpecificationData.CalculateExtraNumberOfBytesNeededForSnapshotting();

        if (gadgetArchetypeData.SpecificationData.GadgetType == GadgetType.HitBoxGadget)
            result += lemmingTrackerByteRequirement;

        return result;
    }

    public void BuildLevelGadgets(LemmingManager lemmingManager, TribeManager tribeManager)
    {
        nint dataHandle = GadgetDataBuffer.Handle;
        ref nint dataHandleRef = ref dataHandle;

        var gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(_levelData);

        foreach (var gadgetInstanceData in _levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(gadgetInstanceData.StyleIdentifier, gadgetInstanceData.PieceIdentifier);
            var gadgetArchetypeData = gadgetArchetypeDataLookup[stylePiecePair];

            if (gadgetArchetypeData.SpecificationData.GadgetType != gadgetInstanceData.SpecificationData.GadgetType)
                throw new Exception("GadgetType mismatch!");

            var newGadget = BuildGadget(gadgetArchetypeData, gadgetInstanceData, lemmingManager, tribeManager, ref dataHandleRef, lemmingManager.NumberOfLemmings);
            _gadgets.Add(newGadget);

            dataHandle--;
            dataHandle |= sizeof(ulong) - 1;
            dataHandle++;
        }
    }

    private GadgetBase BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        LemmingManager lemmingManager,
        TribeManager tribeManager,
        ref nint dataHandleRef,
        int numberOfLemmingsInLevel)
    {
        return gadgetArchetypeData.SpecificationData.GadgetType switch
        {
            GadgetType.HitBoxGadget => BuildHitBoxGadget(gadgetArchetypeData, gadgetInstanceData, lemmingManager, tribeManager, ref dataHandleRef, numberOfLemmingsInLevel),
            GadgetType.HatchGadget => BuildHatchGadget(gadgetArchetypeData, gadgetInstanceData, tribeManager, ref dataHandleRef),
            GadgetType.LogicGate => BuildLogicGateGadget(gadgetArchetypeData, gadgetInstanceData, ref dataHandleRef),
            //  GadgetType.Counter => 
            GadgetType.LevelTimerObserver => BuildLevelTimerObserverGadget(gadgetArchetypeData, gadgetInstanceData, ref dataHandleRef),

            _ => throw new NotImplementedException(),
        };
    }

    private HitBoxGadget BuildHitBoxGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        LemmingManager lemmingManager,
        TribeManager tribeManager,
        ref nint dataHandleRef,
        int numberOfLemmingsInLevel)
    {
        var hitBoxGadgetBuilder = new HitBoxGadgetBuilder(gadgetArchetypeData, gadgetInstanceData, _gadgetTriggers, _gadgetBehaviours);

        return hitBoxGadgetBuilder.BuildHitBoxGadget(lemmingManager, tribeManager, ref dataHandleRef, numberOfLemmingsInLevel);
    }

    private HatchGadget BuildHatchGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        TribeManager tribeManager,
        ref nint dataHandleRef)
    {
        var hatchGadgetBuilder = new HatchGadgetBuilder(gadgetArchetypeData, gadgetInstanceData, _gadgetTriggers, _gadgetBehaviours);

        return hatchGadgetBuilder.BuildHatchGadget(tribeManager, ref dataHandleRef);
    }

    private GadgetBase BuildLogicGateGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        ref nint dataHandleRef)
    {
        var logicGateBuilder = new LogicGateBuilder(gadgetArchetypeData, gadgetInstanceData, _gadgetTriggers, _gadgetBehaviours);

        return logicGateBuilder.BuildLogicGateGadget(ref dataHandleRef);
    }

    private GadgetBase BuildLevelTimerObserverGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        ref nint dataHandleRef)
    {
        throw new NotImplementedException();
    }

    public GadgetBase[] GetGadgets() => _gadgets.ToArray();
    public GadgetTrigger[] GetGadgetTriggers() => _gadgetTriggers.ToArray();
    public GadgetBehaviour[] GetGadgetBehaviours() => _gadgetBehaviours.ToArray();
}
