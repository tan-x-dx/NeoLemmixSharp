using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public readonly ref struct LemmingMovementHelper
{
    public const int MaxIntermediateCheckPositions = 11;

    private readonly Lemming _lemming;
    private readonly Span<LevelPosition> _checkPositions;

    public LemmingMovementHelper(Lemming lemming, Span<LevelPosition> checkPositions)
    {
        _lemming = lemming;
        _checkPositions = checkPositions;
    }

    /// <summary>
    /// The intermediate checks are made according to:
    /// http://www.lemmingsforums.net/index.php?topic=2604.7
    /// </summary>
    public int EvaluateCheckPositions()
    {
        var previousLemmingPosition = _lemming.PreviousLevelPosition;
        var currentLemmingPosition = _lemming.LevelPosition;

        var orientation = _lemming.Orientation;
        var previousAction = _lemming.PreviousAction;

        var workPosition = previousLemmingPosition;

        var length = 0;
        if (previousAction == JumperAction.Instance)
        {
            HandleJumping(ref workPosition, ref length); // But continue with the rest as normal
        }

        // No movement
        if (previousLemmingPosition == currentLemmingPosition)
        {
            if (previousAction == JumperAction.Instance && length != 0)
                return length;

            AddPosition(workPosition, ref length);

            return length;
        }

        // Special treatment of miners!
        if (previousAction == MinerAction.Instance)
        {
            // First move one pixel down, if Y-coordinate changed
            if (orientation.FirstIsBelowSecond(currentLemmingPosition, workPosition))
            {
                workPosition = orientation.MoveDown(workPosition, 1);
                AddPosition(workPosition, ref length);
            }

            MoveHorizontally(orientation, ref workPosition, currentLemmingPosition, ref length);
            MoveVertically(orientation, ref workPosition, currentLemmingPosition, ref length);

            return length;
        }

        // Lemming moves up or is faller; exception is made for builders!
        if (previousAction != BuilderAction.Instance &&
            (orientation.FirstIsAboveSecond(currentLemmingPosition, previousLemmingPosition) ||
             _lemming.CurrentAction == FallerAction.Instance))
        {
            MoveHorizontally(orientation, ref workPosition, currentLemmingPosition, ref length);
            MoveVertically(orientation, ref workPosition, currentLemmingPosition, ref length);

            return length;
        }

        // Lemming moves down (or straight) and is not a faller; alternatively lemming is a builder!
        MoveVertically(orientation, ref workPosition, currentLemmingPosition, ref length);
        MoveHorizontally(orientation, ref workPosition, currentLemmingPosition, ref length);

        return length;
    }

    private void AddPosition(LevelPosition levelPosition, ref int length)
    {
        _checkPositions[length++] = levelPosition;
    }

    private void MoveHorizontally(
        Orientation orientation,
        ref LevelPosition workPosition,
        LevelPosition referencePosition,
        ref int length)
    {
        var dx = Math.Sign(orientation.GetHorizontalDelta(workPosition, referencePosition));

        while (!orientation.MatchesHorizontally(workPosition, referencePosition))
        {
            workPosition = orientation.MoveRight(workPosition, dx);
            AddPosition(workPosition, ref length);
        }
    }

    private void MoveVertically(
        Orientation orientation,
        ref LevelPosition workPosition,
        LevelPosition referencePosition,
        ref int length)
    {
        var dy = Math.Sign(orientation.GetVerticalDelta(workPosition, referencePosition));

        while (!orientation.MatchesVertically(workPosition, referencePosition))
        {
            workPosition = orientation.MoveDown(workPosition, dy);
            AddPosition(workPosition, ref length);
        }
    }

    private void HandleJumping(
        ref LevelPosition workPosition,
        ref int length)
    {
        var jumpPositions = _lemming.GetJumperPositions();

        foreach (var levelPosition in jumpPositions)
        {
            if (LevelScreen.PositionOutOfBounds(levelPosition))
                break;

            workPosition = levelPosition;
            AddPosition(workPosition, ref length);
        }
    }
}