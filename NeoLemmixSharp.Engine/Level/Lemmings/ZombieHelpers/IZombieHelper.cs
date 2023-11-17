using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Lemmings.ZombieHelpers;

public interface IZombieHelper
{
    void RegisterZombie(Lemming lemming);
    void UpdateZombiePosition(Lemming lemming);
    void DeregisterZombie(Lemming lemming);

    [Pure]
    bool AnyZombies();
    void CheckZombies(Lemming lemming);
}