﻿using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public sealed class HatchSpawnData
{
    private readonly Team _team;
    private readonly uint _rawStateData;
    private readonly Orientation _orientation;
    private readonly FacingDirection _facingDirection;

    public int LemmingsToRelease { get; private set; }

    public HatchSpawnData(Team team, uint rawStateData, Orientation orientation, FacingDirection facingDirection, int lemmingsToRelease)
    {
        _team = team;
        _rawStateData = rawStateData;
        _orientation = orientation;
        _facingDirection = facingDirection;
        LemmingsToRelease = lemmingsToRelease;
    }

    public void InitialiseLemming(Lemming lemming)
    {
        lemming.SetRawData(_team, _rawStateData, _orientation, _facingDirection);

        FallerAction.Instance.TransitionLemmingToAction(lemming, false);
        lemming.InitialFall = lemming.CurrentAction == FallerAction.Instance; // could be a walker if eg. spawned inside terrain
        LemmingsToRelease--;
    }
}