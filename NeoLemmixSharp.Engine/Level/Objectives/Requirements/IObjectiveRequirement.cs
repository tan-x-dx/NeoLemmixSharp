﻿using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.Objectives.Requirements;

public interface IObjectiveRequirement
{
    bool IsSatisfied { get; }
    bool IsFailed { get; }

    void RecheckCriteria();
}

public interface ISkillSetRequirement : IObjectiveRequirement
{
    ReadOnlySpan<SkillSetData> SkillSetData { get; }
}