﻿using NeoLemmixSharp.Common.Util;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;

public sealed class EmptyHitBox : IHitBox
{
    public static EmptyHitBox Instance { get; } = new();

    private EmptyHitBox()
    {
    }

    public bool MatchesLemming(Lemming lemming, LevelPosition levelPosition) => false;
}