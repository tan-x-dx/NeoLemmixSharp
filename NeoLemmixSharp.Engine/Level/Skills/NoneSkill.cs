using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Skills;

public static class NoneSkill
{
    [DoesNotReturn]
    public static bool CanAssignToLemming(Lemming lemming)
    {
        throw new InvalidOperationException("Cannot assign NONE skill!");
    }

    [DoesNotReturn]
    public static void AssignToLemming(Lemming lemming)
    {
        throw new InvalidOperationException("Cannot assign NONE skill!");
    }
}
