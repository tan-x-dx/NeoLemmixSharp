using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Timer;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class ControlPanelTextualData
{
    private const int CharLengthForLemmingActionAndCount = LevelConstants.LongestActionNameLength + // Enough space for the action part
                                                           1 + // Add a space
                                                           CharLengthForLemmingCount; // Lemmings under cursor

    private const int CharLengthForHatchCount = 3;
    private const int CharLengthForLemmingCount = 3;
    private const int CharLengthForGoalCount = 1 + CharLengthForLemmingCount; // Can have a minus sign so add 1

    public const int TotalControlPanelTextLength = CharLengthForLemmingActionAndCount +
                                                   CharLengthForHatchCount +
                                                   CharLengthForLemmingCount +
                                                   CharLengthForGoalCount;

    private LemmingActionAndCountCharBuffer _lemmingActionAndCountString;
    private HatchCountCharBuffer _hatchCountString;
    private LemmingCountCharBuffer _lemmingsOutString;
    private GoalCountCharBuffer _goalCountString;

    public LevelTimer LevelTimer { get; }

    public ReadOnlySpan<int> LemmingActionAndCountSpan => _lemmingActionAndCountString;
    public ReadOnlySpan<int> HatchCountSpan => _hatchCountString;
    public ReadOnlySpan<int> LemmingsOutSpan => _lemmingsOutString;
    public ReadOnlySpan<int> GoalCountSpan => _goalCountString;

    public ControlPanelTextualData(LevelTimer levelTimer)
    {
        LevelTimer = levelTimer;
    }

    public void SetCursorData(string actionName, int numberOfLemmingsUnderCursor)
    {
        var sourceSpan = actionName.AsSpan();
        Span<int> destSpan = _lemmingActionAndCountString;
        destSpan.Clear();

        var i = 0;
        while (i < sourceSpan.Length)
        {
            destSpan[i] = sourceSpan[i];
            i++;
        }

        i++; // Add a space. Can just increment index as value of zero will not be rendered
        var spanLength = TextRenderingHelpers.GetNumberStringLength(numberOfLemmingsUnderCursor);
        TextRenderingHelpers.WriteDigits(destSpan.Slice(i, spanLength), numberOfLemmingsUnderCursor);
    }

    public void ClearCursorData()
    {
        Span<int> span = _lemmingActionAndCountString;
        span.Clear();
    }

    public void SetHatchData(int hatchCount)
    {
        Span<int> span = _hatchCountString;
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        Span<int> span = _lemmingsOutString;
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, lemmingCount);
    }

    public void SetGoalData(int goalNumber)
    {
        Span<int> span = _goalCountString;
        if (goalNumber < 0)
        {
            goalNumber = -goalNumber;
            span[0] = '-';
            span = span.Slice(1, CharLengthForGoalCount - 1);
        }

        span.Clear();

        TextRenderingHelpers.WriteDigits(span, goalNumber);
    }

    [InlineArray(CharLengthForLemmingActionAndCount)]
    private struct LemmingActionAndCountCharBuffer
    {
        private int _firstElement;
    }

    [InlineArray(CharLengthForHatchCount)]
    private struct HatchCountCharBuffer
    {
        private int _firstElement;
    }

    [InlineArray(CharLengthForLemmingCount)]
    private struct LemmingCountCharBuffer
    {
        private int _firstElement;
    }

    [InlineArray(CharLengthForGoalCount)]
    private struct GoalCountCharBuffer
    {
        private int _firstElement;
    }
}