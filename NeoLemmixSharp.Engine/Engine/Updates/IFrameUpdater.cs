﻿namespace NeoLemmixSharp.Engine.Engine.Updates;

public interface IFrameUpdater
{
    void UpdateLemming(Lemming lemming);

    void Update();
}