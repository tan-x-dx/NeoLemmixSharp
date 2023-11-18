namespace NeoLemmixSharp.Engine.Level.Lemmings.ZombieHelpers;

public sealed class EmptyZombieHelper : IZombieHelper
{
    private static InvalidOperationException ExpectedNoZombiesException => new("Expected no zombies for this level!");

    public void RegisterZombie(Lemming lemming) => throw ExpectedNoZombiesException;
    public void UpdateZombiePosition(Lemming lemming) { } // Do nothing
    public void DeregisterZombie(Lemming lemming) => throw ExpectedNoZombiesException;
    public bool AnyZombies() => false; // No zombies!
    public void CheckZombies(Lemming lemming) { } // Do nothing
}