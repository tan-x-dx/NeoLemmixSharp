﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;

namespace NeoLemmixSharp.Engine.Engine.Updates;

public sealed class PauseUpdater : IFrameUpdater
{
    private readonly ItemWrapper<int> _elapsedTicks;

    public PauseUpdater(ItemWrapper<int> elapsedTicks)
    {
        _elapsedTicks = elapsedTicks;
    }

    public UpdateState UpdateState => UpdateState.Paused;

    public void UpdateLemming(Lemming lemming)
    {
    }

    public bool Tick()
    {
        return false;
    }
}