using static NeoLemmixSharp.Engine.LemmingStates.LemmingConstants;

namespace NeoLemmixSharp.Engine.LemmingStates;

public sealed class FallerState : ILemmingState
{
    public static FallerState Instance { get; } = new();

    private FallerState()
    {
    }

    public int LemmingStateId => 2;
    public void UpdateLemming(Lemming lemming)
    {
      /*  var free = CommonMethods.NumberOfNonSolidPixelsBelow(lemming, FallerStep);
        if (free == FallDistanceForceFall)
        {
            lemming.Y += FallerStep;
        }
        else
        {
            lemming.Y += free; // max: FALLER_STEP
        }

        if (lemming.Y >= LevelScreen.CurrentLevel!.Height)
        {
            lemming.Y = 0;
        }

        if (free == 0)
        { // check ground hit
            lemming.CurrentState = WalkerState.Instance;
            // counter = 0;
        }*/
    }
}