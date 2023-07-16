namespace NeoLemmixSharp.Engine.Engine.Actions;

public sealed class FencerAction : LemmingAction
{
    public const int NumberOfFencerAnimationFrames = 16;

    public static FencerAction Instance { get; } = new();

    private FencerAction()
    {
    }

    public override int Id => 10;
    public override string LemmingActionName => "fencer";
    public override int NumberOfAnimationFrames => NumberOfFencerAnimationFrames;
    public override bool IsOneTimeAction => false;

    public override bool UpdateLemming(Lemming lemming)
    {
        return false;
    }
}