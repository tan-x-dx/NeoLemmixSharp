using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Lemmings;
using static NeoLemmixSharp.Engine.Level.Lemmings.LemmingActionHelpers;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class FloaterAction : LemmingAction
{
    public static readonly FloaterAction Instance = new();

    private static ReadOnlySpan<int> FloaterFallTable => [3, 3, 3, 3, -1, 0, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2];

    private FloaterAction()
        : base(
            EngineConstants.FloaterActionId,
            EngineConstants.FloaterActionName,
            EngineConstants.FloaterActionSpriteFileName,
            EngineConstants.FloaterAnimationFrames,
            EngineConstants.MaxFloaterPhysicsFrames,
            EngineConstants.PermanentSkillPriority)
    {
    }

    public override bool UpdateLemming(Lemming lemming)
    {
        var maxFallDistance = FloaterFallTable[lemming.PhysicsFrame - 1];

        var orientation = lemming.Orientation;
        ref var lemmingPosition = ref lemming.LevelPosition;

        var updraftFallDelta = GetUpdraftFallDelta(lemming);

        maxFallDistance += updraftFallDelta.Y;

        lemmingPosition = orientation.MoveRight(lemmingPosition, updraftFallDelta.X);

        var groundPixelDistance = Math.Min(FindGroundPixel(lemming, lemmingPosition), 0);
        if (maxFallDistance > -groundPixelDistance)
        {
            lemmingPosition = orientation.MoveUp(lemmingPosition, groundPixelDistance);
            lemming.SetNextAction(WalkerAction.Instance);
        }
        else
        {
            lemmingPosition = orientation.MoveDown(lemmingPosition, maxFallDistance);
        }

        return true;
    }

    protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
    protected override int TopLeftBoundsDeltaY(int animationFrame) => 12;

    protected override int BottomRightBoundsDeltaX(int animationFrame) => 4;
    protected override int BottomRightBoundsDeltaY(int animationFrame) => 1;
}