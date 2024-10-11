using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class ControlPanelTextualData
{
    private const int CharLengthForLemmingActionAndCount = LevelConstants.LongestActionNameLength + // Enough space for the action part
                                                           1 + // Add a space
                                                           CharLengthForLemmingCount; // Lemmings under cursor

    private const int CharLengthForHatchCount = 3;
    private const int CharLengthForLemmingCount = 4; // Max number of lemmings is a number with 4 digits
    private const int CharLengthForGoalCount = 1 + CharLengthForLemmingCount; // Can have a minus sign so add 1

    public const int TotalControlPanelTextLength = CharLengthForLemmingActionAndCount +
                                                   CharLengthForHatchCount +
                                                   CharLengthForLemmingCount +
                                                   CharLengthForGoalCount;

    private LemmingActionAndCountCharBuffer _lemmingActionAndCountString;
    private HatchCountCharBuffer _hatchCountString;
    private LemmingCountCharBuffer _lemmingsOutString;
    private GoalCountCharBuffer _goalCountString;

    private readonly ControlPanelParameterSet _controlPanelParameters;

    public ReadOnlySpan<char> LemmingActionAndCountSpan => _lemmingActionAndCountString;
    public ReadOnlySpan<char> HatchCountSpan => _hatchCountString;
    public ReadOnlySpan<char> LemmingsOutSpan => _lemmingsOutString;
    public ReadOnlySpan<char> GoalCountSpan => _goalCountString;

    public ControlPanelTextualData(
        ControlPanelParameterSet controlPanelParameters)
    {
        _controlPanelParameters = controlPanelParameters;
    }

    public void SetCursorData(Lemming? lemmingUnderCursor, int numberOfLemmingsUnderCursor)
    {
        if (lemmingUnderCursor is null)
        {
            ClearCursorData();
        }
        else
        {
            WriteLemmingCursorData(lemmingUnderCursor, numberOfLemmingsUnderCursor);
        }
    }

    private void WriteLemmingCursorData(Lemming lemming, int numberOfLemmingsUnderCursor)
    {
        var textLength = WriteLemmingInfo(lemming);

        textLength++; // Add a space. Can just increment index as value of zero will not be rendered
        var spanLength = TextRenderingHelpers.GetNumberStringLength(numberOfLemmingsUnderCursor);
        Span<char> destSpan = _lemmingActionAndCountString;
        TextRenderingHelpers.WriteDigits(destSpan.Slice(textLength, spanLength), numberOfLemmingsUnderCursor);
    }

    [Pure]
    private bool ShowAthleteInformation() => _controlPanelParameters.Contains(ControlPanelParameters.ShowAthleteInformation);

    private int WriteLemmingInfo(Lemming lemming)
    {
        Span<char> destSpan = _lemmingActionAndCountString;
        destSpan.Clear();

        var state = lemming.State;

        if (state.HasPermanentSkill && ShowAthleteInformation())
            return WriteAthleteInformation(destSpan, state);

        var action = lemming.CurrentAction;

        return action.CursorSelectionPriorityValue == LevelConstants.NonPermanentSkillPriority
            ? WriteActionName(destSpan, action)
            : WriteAthleteTypeInformation(destSpan, action, state);
    }

    private static int WriteAthleteInformation(Span<char> destSpan, LemmingState state)
    {
        destSpan = destSpan[..7];
        destSpan.Fill('-');

        if (state.IsSlider) destSpan[0] = 'L';
        if (state.IsClimber) destSpan[1] = 'C';
        if (state.IsSwimmer) destSpan[2] = 'S';
        else if (state.IsAcidLemming) destSpan[2] = 'A';
        else if (state.IsWaterLemming) destSpan[2] = 'W';
        if (state.IsFloater) destSpan[3] = 'F';
        else if (state.IsGlider) destSpan[3] = 'G';
        if (state.IsDisarmer) destSpan[4] = 'D';
        if (state.IsZombie) destSpan[5] = 'Z';
        if (state.IsNeutral) destSpan[6] = 'N';

        return 7;
    }

    private static int WriteAthleteTypeInformation(
        Span<char> destSpan,
        LemmingAction action,
        LemmingState state)
    {
        ReadOnlySpan<char> sourceSpan;
        var isZombie = state.IsZombie;
        var isNeutral = state.IsNeutral;

        if (isZombie && isNeutral)
        {
            sourceSpan = LevelConstants.NeutralControlPanelZombie;
        }
        else if (isZombie)
        {
            sourceSpan = LevelConstants.ZombieControlPanelString;
        }
        else if (isNeutral)
        {
            sourceSpan = LevelConstants.NeutralControlPanelString;
        }
        else
        {
            var numberOfPermanentSkills = state.NumberOfPermanentSkills;

            sourceSpan = numberOfPermanentSkills switch
            {
                2 => LevelConstants.AthleteControlPanelString2Skills,
                3 => LevelConstants.AthleteControlPanelString3Skills,
                4 => LevelConstants.AthleteControlPanelString4Skills,
                5 => LevelConstants.AthleteControlPanelString5Skills,
                _ => action.LemmingActionName
            };
        }

        sourceSpan.CopyTo(destSpan);

        return sourceSpan.Length;
    }

    private static int WriteActionName(Span<char> destSpan, LemmingAction action)
    {
        var sourceSpan = action.LemmingActionName.AsSpan();
        sourceSpan.CopyTo(destSpan);
        return sourceSpan.Length;
    }

    public void ClearCursorData()
    {
        Span<char> span = _lemmingActionAndCountString;
        span.Clear();
    }

    public void SetHatchData(int hatchCount)
    {
        Span<char> span = _hatchCountString;
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        Span<char> span = _lemmingsOutString;
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, lemmingCount);
    }

    public void SetGoalData(int goalNumber)
    {
        Span<char> span = _goalCountString;
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
        private char _firstElement;
    }

    [InlineArray(CharLengthForHatchCount)]
    private struct HatchCountCharBuffer
    {
        private char _firstElement;
    }

    [InlineArray(CharLengthForLemmingCount)]
    private struct LemmingCountCharBuffer
    {
        private char _firstElement;
    }

    [InlineArray(CharLengthForGoalCount)]
    private struct GoalCountCharBuffer
    {
        private char _firstElement;
    }
}