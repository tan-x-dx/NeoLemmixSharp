using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchSpawnData
{
    private readonly PointerWrapper _lemmingsToRelease;
    private readonly Orientation _orientation;
    private readonly FacingDirection _facingDirection;
    private readonly int _tribeId;
    private readonly uint _rawStateData;

    public int HatchGroupId { get; }
    public int LemmingsToRelease => _lemmingsToRelease.IntValue;

    public HatchSpawnData(
        ref nint dataHandle,
        int lemmingsToRelease,
        Orientation orientation,
        FacingDirection facingDirection,
        int tribeId,
        uint rawStateData,
        int hatchGroupId)
    {
        _lemmingsToRelease = PointerDataHelper.CreateItem<PointerWrapper>(ref dataHandle);
        _lemmingsToRelease.IntValue = lemmingsToRelease;
        _orientation = orientation;
        _facingDirection = facingDirection;
        _tribeId = tribeId;
        _rawStateData = rawStateData;

        HatchGroupId = hatchGroupId;
    }

    public void SpawnLemming(Lemming lemming)
    {
        lemming.SetRawData(_orientation, _facingDirection, _tribeId, _rawStateData);

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentActionId == LemmingActionConstants.FallerActionId; // could be a walker if eg. spawned inside terrain
        _lemmingsToRelease.IntValue--;
    }
}
