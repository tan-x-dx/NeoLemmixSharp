using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Level.Objectives;

public readonly record struct SkillSetData(LemmingSkillType SkillType, int TribeId, int InitialQuantity);
