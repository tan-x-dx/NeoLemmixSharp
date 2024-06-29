﻿namespace NeoLemmixSharp.Engine.Level.Objectives;

public sealed class TimeRequirement : IObjectiveRequirement
{
    public int TimeLimitInSeconds { get; }

    public TimeRequirement(int timeLimitInSeconds)
    {
        TimeLimitInSeconds = timeLimitInSeconds;
    }

    public bool IsSatisfied { get; }
    public bool IsFailed { get; }
}