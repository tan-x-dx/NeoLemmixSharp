using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Timer;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class ControlPanelTextualData
{
    private const int CharLengthForHatchCount = 3;
    private const int CharLengthForLemmingCount = 3;
    private const int CharLengthForGoalCount = 1 + CharLengthForLemmingCount; // Can have a minus sign so add 1

    private readonly int[] _lemmingActionAndCountString;
    private readonly int[] _hatchCountString;
    private readonly int[] _lemmingsOutString;
    private readonly int[] _goalCountString;

    public LevelTimer LevelTimer { get; }

    public ReadOnlySpan<int> LemmingActionAndCountSpan => new(_lemmingActionAndCountString);
    public ReadOnlySpan<int> HatchCountSpan => new(_hatchCountString);
    public ReadOnlySpan<int> LemmingsOutSpan => new(_lemmingsOutString);
    public ReadOnlySpan<int> GoalCountSpan => new(_goalCountString);

    public ControlPanelTextualData(LevelTimer levelTimer)
    {
        var lemmingActionAndCountString = CalculateLemmingActionAndCountStringLength();
        _lemmingActionAndCountString = new int[lemmingActionAndCountString];
        _hatchCountString = new int[CharLengthForHatchCount];
        _lemmingsOutString = new int[CharLengthForLemmingCount];
        _goalCountString = new int[CharLengthForGoalCount];

        LevelTimer = levelTimer;
    }

    private static int CalculateLemmingActionAndCountStringLength()
    {
        var longestAction = 0;
        foreach (var action in LemmingAction.AllItems)
        {
            longestAction = Math.Max(longestAction, action.LemmingActionName.Length);
        }

        return longestAction // Enough space for the string part
               + 1 // Add a space
               + CharLengthForLemmingCount; // Lemmings under cursor
    }

    public int GetTotalCharacterLength()
    {
        return _lemmingActionAndCountString.Length +
               _hatchCountString.Length +
               _lemmingsOutString.Length +
               _goalCountString.Length;
    }

    public void SetCursorData(string actionName, int count)
    {
        var sourceSpan = actionName.AsSpan();
        var destSpan = new Span<int>(_lemmingActionAndCountString);
        destSpan.Clear();

        var i = 0;
        while (i < sourceSpan.Length)
        {
            destSpan[i] = sourceSpan[i];
            i++;
        }

        i++; // Add a space
        var spanLength = TextRenderingHelpers.QuickLog10(count);
        TextRenderingHelpers.WriteDigits(destSpan.Slice(i, spanLength), count);
    }

    public void ClearCursorData()
    {
        var span = new Span<int>(_lemmingActionAndCountString);
        span.Clear();
    }

    public void SetHatchData(int hatchCount)
    {
        var span = new Span<int>(_hatchCountString);
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        var span = new Span<int>(_lemmingsOutString);
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, lemmingCount);
    }

    public void SetGoalData(int goalNumber)
    {
        Span<int> span;
        if (goalNumber < 0)
        {
            goalNumber = -goalNumber;
            span = new Span<int>(_goalCountString, 1, CharLengthForGoalCount - 1);
            _goalCountString[0] = '-';
        }
        else
        {
            span = new Span<int>(_goalCountString);
        }

        span.Clear();

        TextRenderingHelpers.WriteDigits(span, goalNumber);
    }
}