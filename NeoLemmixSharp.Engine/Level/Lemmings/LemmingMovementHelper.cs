using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public ref struct LemmingMovementHelper
{
    public const int MaxIntermediateCheckPositions = 11;

    private readonly Lemming _lemming;
    private readonly Span<LevelPosition> _checkPositions;
    public int Length;

    public LemmingMovementHelper(Lemming lemming, Span<LevelPosition> checkPositions)
    {
        _lemming = lemming;
        _checkPositions = checkPositions;
    }

    public void EvaluateCheckPositions()
    {
        if (_lemming.Id == 6)
        {
            ;
        }

        var workPosition = _lemming.PreviousLevelPosition;
        var previousLemmingPosition = _lemming.PreviousLevelPosition;
        var currentLemmingPosition = _lemming.LevelPosition;

        var orientation = _lemming.Orientation;
        var previousAction = _lemming.PreviousAction;
        if (_lemming.PreviousAction == JumperAction.Instance)
        {
            HandleJumping(); // But continue with the rest as normal
        }

        // No movement
        if (previousLemmingPosition == currentLemmingPosition)
        {
            if (previousAction != JumperAction.Instance || Length == 0)
            {
                AddPosition(workPosition);
            }

            return;
        }

        // Special treatment of miners!
        if (previousAction == MinerAction.Instance)
        {
            // First move one pixel down, if Y-coordinate changed
            if (orientation.FirstIsBelowSecond(currentLemmingPosition, workPosition))
            {
                workPosition = orientation.MoveDown(workPosition, 1);
                AddPosition(workPosition);
            }

            MoveHorizontally(orientation, ref workPosition, currentLemmingPosition);
            MoveVertically(orientation, ref workPosition, currentLemmingPosition);

            return;
        }

        // Lemming moves up or is faller; exception is made for builders!
        if ((orientation.FirstIsAboveSecond(currentLemmingPosition, previousLemmingPosition) || _lemming.CurrentAction == FallerAction.Instance) &&
            previousAction != BuilderAction.Instance)
        {
            MoveHorizontally(orientation, ref workPosition, currentLemmingPosition);
            MoveVertically(orientation, ref workPosition, currentLemmingPosition);

            return;
        }

        // Lemming moves down (or straight) and is not a faller; alternatively lemming is a builder!
        MoveVertically(orientation, ref workPosition, currentLemmingPosition);
        MoveHorizontally(orientation, ref workPosition, currentLemmingPosition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AddPosition(LevelPosition levelPosition)
    {
        _checkPositions[Length++] = levelPosition;
    }

    private void MoveHorizontally(Orientation orientation, ref LevelPosition workPosition, LevelPosition referencePosition)
    {
        var dx = Math.Sign(orientation.GetHorizontalDelta(workPosition, referencePosition));
        /*orientation.FirstIsToRightOfSecond(referencePosition, workPosition)
            ? 1
            : -1;*/

        while (!orientation.MatchesHorizontally(workPosition, referencePosition))
        {
            workPosition = orientation.MoveRight(workPosition, dx);
            AddPosition(workPosition);
        }
    }

    private void MoveVertically(Orientation orientation, ref LevelPosition workPosition, LevelPosition referencePosition)
    {
        var dy = Math.Sign(orientation.GetVerticalDelta(workPosition, referencePosition));
        /*orientation.FirstIsBelowSecond(referencePosition, workPosition)
            ? 1
            : -1;*/

        while (!orientation.MatchesVertically(workPosition, referencePosition))
        {
            workPosition = orientation.MoveDown(workPosition, dy);
            AddPosition(workPosition);
        }
    }

    private void HandleJumping()
    {

    }
}