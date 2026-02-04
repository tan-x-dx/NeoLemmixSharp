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

    private readonly int _numberOfHitBoxGadgets;

    public GadgetBuilder(
        LevelData levelData,
        SafeBufferAllocator safeBufferAllocator,
        int numberOfLemmings)
    {
        _levelData = levelData;
        _numberOfHitBoxGadgets = CalculateNumberOfHitBoxGadgets(levelData);

        var numberOfGadgetInstances = _levelData.AllGadgetInstanceData.Count;

        // This list's required capacity is known - perfect fit
        _gadgets = new List<GadgetBase>(numberOfGadgetInstances);

        // Preallocate these lists with large(ish) initial capacities to reduce realloactions further down the line
        _gadgetTriggers = new List<GadgetTrigger>(numberOfGadgetInstances * GadgetStateCapacityMultiplier * GadgetTriggerCapacityMultiplier);
        _gadgetBehaviours = new List<GadgetBehaviour>(numberOfGadgetInstances * GadgetStateCapacityMultiplier * GadgetBehaviourCapacityMultiplier);

        var gadgetDataBufferLength = CalculateGadgetBufferSizes(levelData, numberOfLemmings);

        GadgetDataBuffer = safeBufferAllocator.AllocateRawArray(gadgetDataBufferLength);
    }

    private int CalculateGadgetBufferSizes(
        LevelData levelData,
        int numberOfLemmings)
    {
        var numberOfBytesPerLemmingTracker = CalculateNumberOfBytesPerLemmingTracker(numberOfLemmings);

        var result = _numberOfHitBoxGadgets * numberOfBytesPerLemmingTracker; // LemmingTracker data goes at the start

        var gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(levelData);

        foreach (var gadgetInstanceData in levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(gadgetInstanceData.StyleIdentifier, gadgetInstanceData.PieceIdentifier);
            var gadgetArchetypeData = gadgetArchetypeDataLookup[stylePiecePair];

            if (gadgetArchetypeData.SpecificationData.GadgetType != gadgetInstanceData.SpecificationData.GadgetType)
                throw new Exception("GadgetType mismatch!");

            var numberOfBytesNeededForThisGadget = CalculateNumberOfBytesNeededForThisGadget(gadgetArchetypeData, gadgetInstanceData);

            result += numberOfBytesNeededForThisGadget;
        }

        return result;
    }

    private static int CalculateNumberOfHitBoxGadgets(LevelData levelData)
    {
        var result = 0;
        foreach (var gadgetInstanceData in levelData.AllGadgetInstanceData)
        {
            if (gadgetInstanceData.SpecificationData.GadgetType == GadgetType.HitBoxGadget)
                result++;
        }

        return result;
    }

    private static int CalculateNumberOfBytesNeededForThisGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData)
    {
        var result =
            GadgetBounds.SizeInBytes + // A gadget will always have bounds
            sizeof(int); // A gadget will always have a state index

        result += gadgetArchetypeData.SpecificationData.CalculateExtraNumberOfBytesNeededForSnapshotting();
        result += gadgetInstanceData.SpecificationData.CalculateExtraNumberOfBytesNeededForSnapshotting();

        return result;
    }

    public void BuildLevelGadgets(TribeManager tribeManager, int numberOfLemmings)
    {
        nint dataHandle = GadgetDataBuffer.Handle;
        ref nint dataHandleRef = ref dataHandle;
        var hitBoxGadgetIndex = 0;

        var numberOfBytesPerLemmingTracker = CalculateNumberOfBytesPerLemmingTracker(numberOfLemmings);
        dataHandleRef += _numberOfHitBoxGadgets * numberOfBytesPerLemmingTracker;

        var gadgetArchetypeDataLookup = StyleCache.GetAllGadgetArchetypeData(_levelData);

        foreach (var gadgetInstanceData in _levelData.AllGadgetInstanceData)
        {
            var stylePiecePair = new StylePiecePair(gadgetInstanceData.StyleIdentifier, gadgetInstanceData.PieceIdentifier);
            var gadgetArchetypeData = gadgetArchetypeDataLookup[stylePiecePair];

            if (gadgetArchetypeData.SpecificationData.GadgetType != gadgetInstanceData.SpecificationData.GadgetType)
                throw new Exception("GadgetType mismatch!");

            var newGadget = BuildGadget(gadgetArchetypeData, gadgetInstanceData, tribeManager, numberOfBytesPerLemmingTracker, hitBoxGadgetIndex, ref dataHandleRef);
            if (gadgetInstanceData.SpecificationData.GadgetType == GadgetType.HitBoxGadget)
                hitBoxGadgetIndex++;

            _gadgets.Add(newGadget);
        }
    }

    private GadgetBase BuildGadget(
        GadgetArchetypeData gadgetArchetypeData,
        GadgetInstanceData gadgetInstanceData,
        TribeManager tribeManager,
        int numberOfBytesPerLemmingTracker,
        int hitBoxGadgetIndex,
        ref nint dataHandleRef)
    {
        return gadgetArchetypeData.SpecificationData.GadgetType switch
        {
            GadgetType.HitBoxGadget => BuildHitBoxGadget(gadgetArchetypeData, gadgetInstanceData, tribeManager, numberOfBytesPerLemmingTracker, hitBoxGadgetIndex, ref dataHandleRef),
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
        TribeManager tribeManager,
        int numberOfBytesPerLemmingTracker,
        int hitBoxGadgetIndex,
        ref nint dataHandleRef)
    {
        var hitBoxGadgetBuilder = new HitBoxGadgetBuilder(
            gadgetArchetypeData,
            gadgetInstanceData,
            _gadgetTriggers,
            _gadgetBehaviours);

        nint lemmingTrackerDataHandle = GadgetDataBuffer.Handle + (numberOfBytesPerLemmingTracker * hitBoxGadgetIndex);

        if (hitBoxGadgetIndex >= _numberOfHitBoxGadgets)
            throw new InvalidOperationException("More hit box gadgets than previously expected!");

        var lemmingTracker = new LemmingTracker(lemmingTrackerDataHandle);

        var result = hitBoxGadgetBuilder.BuildHitBoxGadget(
            tribeManager,
            ref dataHandleRef,
            lemmingTracker);

        return result;
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

    public LemmingTrackerManager BuildLevelTrackerManager(int numberOfLemmings)
    {
        var numberOfBytesPerLemmingTracker = CalculateNumberOfBytesPerLemmingTracker(numberOfLemmings);
        var numberOfBytesTotalForLemmingTrackerData = _numberOfHitBoxGadgets * numberOfBytesPerLemmingTracker;

        return new LemmingTrackerManager(GadgetDataBuffer.Handle, numberOfBytesTotalForLemmingTrackerData);
    }

    private static int CalculateNumberOfBytesPerLemmingTracker(int numberOfLemmings) => BitArrayHelpers.CalculateBitArrayBufferLength(numberOfLemmings) * sizeof(ulong);
}
