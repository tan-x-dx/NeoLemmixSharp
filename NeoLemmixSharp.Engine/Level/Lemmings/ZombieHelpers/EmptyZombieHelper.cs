using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Lemmings.ZombieHelpers;

public sealed class EmptyZombieHelper : IZombieHelper
{
    [DoesNotReturn]
    private static void ThrowNoZombiesExpectedException() => throw new InvalidOperationException("Expected no zombies for this level!");

    public void Clear() { } // Do nothing
    public void RegisterZombie(Lemming lemming) => ThrowNoZombiesExpectedException();
    public void UpdateZombiePosition(Lemming lemming) { } // Do nothing
    public void DeregisterZombie(Lemming lemming) => ThrowNoZombiesExpectedException();
    public bool AnyZombies() => false; // No zombies!
    public void CheckZombies(Lemming lemming) { } // Do nothing
}