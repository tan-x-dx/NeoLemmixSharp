using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;

public sealed class HatchSpawnData
{
    private readonly PointerWrapper _lemmingsToRelease;
    private readonly DihedralTransformation _dihedralTransformation;
    private readonly int _tribeId;
    private readonly uint _rawStateData;

    public int HatchGroupId { get; }
    public int LemmingsToRelease => _lemmingsToRelease.IntValue;

    public HatchSpawnData(
        ref nint dataHandle,
        int lemmingsToRelease,
        DihedralTransformation dihedralTransformation,
        int tribeId,
        uint rawStateData,
        int hatchGroupId)
    {
        _lemmingsToRelease = PointerDataHelper.CreateItem<PointerWrapper>(ref dataHandle);
        _lemmingsToRelease.IntValue = lemmingsToRelease;
        _dihedralTransformation = dihedralTransformation;
        _tribeId = tribeId;
        _rawStateData = rawStateData;

        HatchGroupId = hatchGroupId;
    }

    public void SpawnLemming(Lemming lemming)
    {
        lemming.SetRawData(_dihedralTransformation, _tribeId, _rawStateData);

        FallerAction.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentActionType == LemmingActionType.FallerAction; // could be a walker if eg. spawned inside terrain
        _lemmingsToRelease.IntValue--;
    }
}
