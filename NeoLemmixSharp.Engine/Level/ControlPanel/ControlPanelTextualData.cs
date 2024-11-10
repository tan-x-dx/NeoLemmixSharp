using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class ControlPanelTextualData
{
    private const int CharLengthForLemmingActionAndCount = EngineConstants.LongestActionNameLength + // Enough space for the action part
                                                           1 + // Add a space
                                                           CharLengthForLemmingCount; // Lemmings under cursor

    private const int CharLengthForLemmingCount = 4; // Max number of lemmings is a number with 4 digits
    private const int CharLengthForGoalCount = 1 + CharLengthForLemmingCount; // Can have a minus sign so add 1

    public const int TotalControlPanelTextLength = CharLengthForLemmingActionAndCount +
                                                   CharLengthForLemmingCount +
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

        _lemmingActionAndCountString[textLength++] = ' '; // Add a space.
        var numericPartSpanLength = TextRenderingHelpers.GetNumberStringLength(numberOfLemmingsUnderCursor);
        Span<char> numericPartSpan = _lemmingActionAndCountString;
        TextRenderingHelpers.WriteDigits(numericPartSpan.Slice(textLength, numericPartSpanLength), numberOfLemmingsUnderCursor);
    }

    [Pure]
    private bool ShowExpandedAthleteInformation() => _controlPanelParameters.Contains(ControlPanelParameters.ShowExpandedAthleteInformation);

    private int WriteLemmingInfo(Lemming lemming)
    {
        var state = lemming.State;

        if (state.HasPermanentSkill && ShowExpandedAthleteInformation())
            return WriteExpandedAthleteInformation(state);

        var action = lemming.CurrentAction;

        return WriteMinimalAthleteInformation(action, state);
    }

    private int WriteExpandedAthleteInformation(LemmingState state)
    {
        Span<char> destSpan = _lemmingActionAndCountString;
        destSpan.Fill('-');

        if (state.IsSlider) _lemmingActionAndCountString[0] = 'L';
        if (state.IsClimber) _lemmingActionAndCountString[1] = 'C';
        if (state.IsSwimmer) _lemmingActionAndCountString[2] = 'S';
        else if (state.IsAcidLemming) _lemmingActionAndCountString[2] = 'A';
        else if (state.IsWaterLemming) _lemmingActionAndCountString[2] = 'W';
        if (state.IsFloater) _lemmingActionAndCountString[3] = 'F';
        else if (state.IsGlider) _lemmingActionAndCountString[3] = 'G';
        if (state.IsDisarmer) _lemmingActionAndCountString[4] = 'D';
        if (state.IsZombie) _lemmingActionAndCountString[5] = 'Z';
        if (state.IsNeutral) _lemmingActionAndCountString[6] = 'N';

        return 7;
    }

    private int WriteMinimalAthleteInformation(
        LemmingAction action,
        LemmingState state)
    {
        ReadOnlySpan<char> sourceSpan;
        Span<char> destSpan = _lemmingActionAndCountString;
        var isZombie = state.IsZombie;
        var isNeutral = state.IsNeutral;

        if (action.CursorSelectionPriorityValue == EngineConstants.NonPermanentSkillPriority)
        {
            sourceSpan = action.LemmingActionName;
        }
        else if (isZombie && isNeutral)
        {
            sourceSpan = EngineConstants.NeutralZombieControlPanelString;
        }
        else if (isZombie)
        {
            sourceSpan = EngineConstants.ZombieControlPanelString;
        }
        else if (isNeutral)
        {
            sourceSpan = EngineConstants.NeutralControlPanelString;
        }
        else
        {
            var numberOfPermanentSkills = state.NumberOfPermanentSkills;

            sourceSpan = numberOfPermanentSkills switch
            {
                2 => EngineConstants.AthleteControlPanelString2Skills,
                3 => EngineConstants.AthleteControlPanelString3Skills,
                4 => EngineConstants.AthleteControlPanelString4Skills,
                5 => EngineConstants.AthleteControlPanelString5Skills,
                _ => action.LemmingActionName
            };
        }

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
            span = span[1..CharLengthForGoalCount];
        }

        span.Clear();

        TextRenderingHelpers.WriteDigits(span, goalNumber);
    }

    [InlineArray(CharLengthForLemmingActionAndCount)]
    private struct LemmingActionAndCountCharBuffer
    {
        private char _firstElement;
    }

    [InlineArray(CharLengthForLemmingCount)]
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