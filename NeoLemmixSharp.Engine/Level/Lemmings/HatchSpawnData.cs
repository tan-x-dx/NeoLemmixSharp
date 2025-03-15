using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchSpawnData
{
    private readonly Team _team;
    private readonly uint _rawStateData;
    private readonly FacingDirection _facingDirection;

    public int HatchGroupId { get; }
    public Orientation Orientation { get; }
    public int LemmingsToRelease { get; private set; }

    public HatchSpawnData(
        int hatchGroupId,
        Team team,
        uint rawStateData,
        Orientation orientation,
        FacingDirection facingDirection,
        int lemmingsToRelease)
    {
        HatchGroupId = hatchGroupId;
        _team = team;
        _rawStateData = rawStateData;
        Orientation = orientation;
        _facingDirection = facingDirection;
        LemmingsToRelease = lemmingsToRelease;
    }

    public void InitialiseLemming(Lemming lemming)
    {
        lemming.SetRawDataFromOther(_team, _rawStateData, Orientation, _facingDirection);

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentAction == FallerAction.Instance; // could be a walker if eg. spawned inside terrain
        LemmingsToRelease--;
    }
}