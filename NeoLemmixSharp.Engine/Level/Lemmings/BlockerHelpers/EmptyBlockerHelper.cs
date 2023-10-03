namespace NeoLemmixSharp.Engine.Level.Lemmings.BlockerHelpers;

public sealed class EmptyBlockerHelper : IBlockerHelper
{
    private static InvalidOperationException ExpectedNoBlockersException => new("Expected no blockers for this level!");

    public void RegisterBlocker(Lemming lemming) => throw ExpectedNoBlockersException;

    public void UpdateBlockerPosition(Lemming lemming) { } // Do nothing

    public bool LemmingIsBlocking(Lemming lemming) => false;
    public void DeregisterBlocker(Lemming lemming) => throw ExpectedNoBlockersException;

    public bool CanAssignBlocker(Lemming lemming) => throw ExpectedNoBlockersException;

    public void CheckBlockers(Lemming lemming) { } // Do nothing - no blockers!
}