using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.Lemmings.BlockerHelpers;

public interface IBlockerHelper
{
    void RegisterBlocker(Lemming lemming);
    void UpdateBlockerPosition(Lemming lemming);
    bool LemmingIsBlocking(Lemming lemming);
    void DeregisterBlocker(Lemming lemming);

    [Pure]
    bool CanAssignBlocker(Lemming lemming);

    /// <summary>
    /// Check for blockers, but not for miners removing terrain,
    /// see http://www.lemmingsforums.net/index.php?topic=2710.0.
    /// Also not for Jumpers, as this is handled by the JumperAction
    /// </summary>
    void CheckBlockers(Lemming lemming);
}