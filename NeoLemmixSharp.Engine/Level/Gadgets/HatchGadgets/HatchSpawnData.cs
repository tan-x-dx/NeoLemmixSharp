using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchSpawnData
{
    private readonly PointerWrapper<int> _lemmingsToRelease;
    private readonly int _tribeId;
    private readonly uint _rawStateData;
    private readonly FacingDirection _facingDirection;

    public int HatchGroupId { get; }
    public Orientation Orientation { get; }
    public int LemmingsToRelease => _lemmingsToRelease.Value;

    public HatchSpawnData(
        nint dataHandle,
        int hatchGroupId,
        int tribeId,
        uint rawStateData,
        Orientation orientation,
        FacingDirection facingDirection,
        int lemmingsToRelease)
    {
        _lemmingsToRelease = new PointerWrapper<int>(dataHandle);
        HatchGroupId = hatchGroupId;
        _tribeId = tribeId;
        _rawStateData = rawStateData;
        Orientation = orientation;
        _facingDirection = facingDirection;
        _lemmingsToRelease.Value = lemmingsToRelease;
    }

    public void SpawnLemming(Lemming lemming)
    {
        lemming.SetRawData(_tribeId, _rawStateData, Orientation, _facingDirection);

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentAction == FallerAction.Instance; // could be a walker if eg. spawned inside terrain
        _lemmingsToRelease.Value--;
    }
}
