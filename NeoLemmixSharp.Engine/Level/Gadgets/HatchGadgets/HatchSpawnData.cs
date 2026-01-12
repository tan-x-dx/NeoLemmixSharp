using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchSpawnData
{
    private readonly PointerWrapper _lemmingsToRelease;
    private readonly int _tribeId;
    private readonly uint _rawStateData;
    private readonly Orientation _orientation;
    private readonly FacingDirection _facingDirection;

    public int HatchGroupId { get; }
    public int LemmingsToRelease => _lemmingsToRelease.IntValue;

    public HatchSpawnData(
        nint dataHandle,
        int lemmingsToRelease,
        int tribeId,
        uint rawStateData,
        Orientation orientation,
        FacingDirection facingDirection,
        int hatchGroupId)
    {
        _lemmingsToRelease = new PointerWrapper(dataHandle);
        _lemmingsToRelease.IntValue = lemmingsToRelease;
        _tribeId = tribeId;
        _rawStateData = rawStateData;
        _orientation = orientation;
        _facingDirection = facingDirection;

        HatchGroupId = hatchGroupId;
    }

    public void SpawnLemming(Lemming lemming)
    {
        lemming.SetRawData(_tribeId, _rawStateData, _orientation, _facingDirection);

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentAction == FallerAction.Instance; // could be a walker if eg. spawned inside terrain
        _lemmingsToRelease.IntValue--;
    }
}
