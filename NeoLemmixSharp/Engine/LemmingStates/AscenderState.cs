using static NeoLemmixSharp.Engine.LemmingStates.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingStates;

public sealed class AscenderState : ILemmingState
{
    public static AscenderState Instance { get; } = new();

    private AscenderState()
    {
    }

    public int LemmingStateId => 3;
    public void UpdateLemming(Lemming lemming)
    {
        int levitation = CommonMethods.AboveGround(lemming);
        if (levitation > JumperStep)
            lemming.Y -= JumperStep;
        else
        {
            // conversion to walker
            lemming.Y -= levitation;
            lemming.CurrentState = WalkerState.Instance;
        }
    }
}