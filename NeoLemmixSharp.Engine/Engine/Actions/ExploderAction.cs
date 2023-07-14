namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class ExploderAction : LemmingAction
{
    public const int NumberOfExploderAnimationFrames = 1;

    public static ExploderAction Instance { get; } = new();

    private ExploderAction()
    {
    }

    public override int Id => 26;
    public override string LemmingActionName => "bomber";
    public override int NumberOfAnimationFrames => NumberOfExploderAnimationFrames;
    public override bool IsOneTimeAction => true;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }
}