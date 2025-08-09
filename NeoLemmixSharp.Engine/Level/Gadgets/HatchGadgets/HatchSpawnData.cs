using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Tribes;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchSpawnData
{
    private readonly Tribe _tribe;
    private readonly uint _rawStateData;
    private readonly FacingDirection _facingDirection;

    public int HatchGroupId { get; }
    public Orientation Orientation { get; }
    public int LemmingsToRelease { get; private set; }

    public HatchSpawnData(
        int hatchGroupId,
        Tribe tribe,
        uint rawStateData,
        Orientation orientation,
        FacingDirection facingDirection,
        int lemmingsToRelease)
    {
        HatchGroupId = hatchGroupId;
        _tribe = tribe;
        _rawStateData = rawStateData;
        Orientation = orientation;
        _facingDirection = facingDirection;
        LemmingsToRelease = lemmingsToRelease;
    }

    public void InitialiseLemming(Lemming lemming)
    {
        lemming.SetRawDataFromOther(_tribe, _rawStateData, Orientation, _facingDirection);

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentAction == FallerAction.Instance; // could be a walker if eg. spawned inside terrain
        LemmingsToRelease--;
    }
}
