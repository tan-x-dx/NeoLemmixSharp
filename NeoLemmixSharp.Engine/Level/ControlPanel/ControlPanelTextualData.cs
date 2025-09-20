using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public unsafe sealed class ControlPanelTextualData : IDisposable
{
    private const int CharLengthForCursorData = LemmingActionConstants.LongestActionNameLength + // Enough space for the action part
                                                1 + // Add a space
                                                CharLengthForLemmingCount; // Lemmings under cursor

    private const int CharLengthForLemmingCount = 4; // Max number of lemmings is a number with 4 digits
    private const int CharLengthForGoalCount = 1 + CharLengthForLemmingCount; // Can have a minus sign so add 1

    public const int TotalControlPanelTextLength = CharLengthForCursorData +
                                                   CharLengthForLemmingCount +
                                                   CharLengthForLemmingCount +
                                                   CharLengthForGoalCount;

    private readonly ControlPanelParameterSet _controlPanelParameters;
    private readonly RawArray _byteBuffer;

    private readonly char* _cursorDataPointer;
    private readonly char* _hatchCountPointer;
    private readonly char* _lemmingsOutPointer;
    private readonly char* _goalCountPointer;

    public ReadOnlySpan<char> LemmingActionAndCountSpan => new(_cursorDataPointer, CharLengthForCursorData);
    public ReadOnlySpan<char> HatchCountSpan => new(_hatchCountPointer, CharLengthForLemmingCount);
    public ReadOnlySpan<char> LemmingsOutSpan => new(_lemmingsOutPointer, CharLengthForLemmingCount);
    public ReadOnlySpan<char> GoalCountSpan => new(_goalCountPointer, CharLengthForGoalCount);

    public ControlPanelTextualData(
        ControlPanelParameterSet controlPanelParameters)
    {
        _controlPanelParameters = controlPanelParameters;

        _byteBuffer = Helpers.AllocateBuffer<char>(TotalControlPanelTextLength);

        _cursorDataPointer = (char*)_byteBuffer.Handle;
        _hatchCountPointer = (char*)_byteBuffer.Handle + CharLengthForCursorData;
        _lemmingsOutPointer = (char*)_byteBuffer.Handle + (CharLengthForCursorData + CharLengthForLemmingCount);
        _goalCountPointer = (char*)_byteBuffer.Handle + (CharLengthForCursorData + CharLengthForLemmingCount + CharLengthForLemmingCount);
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

        char* p = _cursorDataPointer + textLength;
        *p = ' '; // Add a space.
        p++;
        var numericPartSpanLength = TextRenderingHelpers.GetNumberStringLength(numberOfLemmingsUnderCursor);

        TextRenderingHelpers.WriteDigits(new Span<char>(p, numericPartSpanLength), numberOfLemmingsUnderCursor);
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
        const int TextLengthForExpandedAthleteInformation = 7;

        char* p = _cursorDataPointer;
        for (var i = 0; i < TextLengthForExpandedAthleteInformation; i++)
            p[i] = '-';

        if (state.IsSlider) p[0] = 'L';
        if (state.IsClimber) p[1] = 'C';
        if (state.IsSwimmer) p[2] = 'S';
        else if (state.IsAcidLemming) p[2] = 'A';
        else if (state.IsWaterLemming) p[2] = 'W';
        if (state.IsFloater) p[3] = 'F';
        else if (state.IsGlider) p[3] = 'G';
        if (state.IsDisarmer) p[4] = 'D';
        if (state.IsZombie) p[5] = 'Z';
        if (state.IsNeutral) p[6] = 'N';

        return TextLengthForExpandedAthleteInformation;
    }

    private int WriteMinimalAthleteInformation(
        LemmingAction action,
        LemmingState state)
    {
        ReadOnlySpan<char> sourceSpan = GetSourceSpan();
        Span<char> destSpan = new(_cursorDataPointer, CharLengthForCursorData);

        sourceSpan.CopyTo(destSpan);

        return sourceSpan.Length;

        ReadOnlySpan<char> GetSourceSpan()
        {
            if (action.CursorSelectionPriorityValue == LemmingActionConstants.NonPermanentSkillPriority)
                return action.LemmingActionName;

            if (state.IsZombie && state.IsNeutral)
                return EngineConstants.NeutralZombieControlPanelString;

            if (state.IsZombie)
                return EngineConstants.ZombieControlPanelString;

            if (state.IsNeutral)
                return EngineConstants.NeutralControlPanelString;

            var numberOfPermanentSkills = state.NumberOfPermanentSkills;

            return numberOfPermanentSkills switch
            {
                2 => EngineConstants.AthleteControlPanelString2Skills,
                3 => EngineConstants.AthleteControlPanelString3Skills,
                4 => EngineConstants.AthleteControlPanelString4Skills,
                5 => EngineConstants.AthleteControlPanelString5Skills,
                _ => action.LemmingActionName
            };
        }
    }

    public void ClearCursorData()
    {
        Span<char> span = new(_cursorDataPointer, CharLengthForCursorData);
        span.Clear();
    }

    public void SetHatchData(int hatchCount)
    {
        Span<char> span = new(_hatchCountPointer, CharLengthForLemmingCount);
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        Span<char> span = new(_lemmingsOutPointer, CharLengthForLemmingCount);
        span.Clear();
        TextRenderingHelpers.WriteDigits(span, lemmingCount);
    }

    public void SetGoalData(int goalNumber)
    {
        char* p = _goalCountPointer;
        var spanLength = CharLengthForGoalCount;
        if (goalNumber < 0)
        {
            goalNumber = -goalNumber;
            *p = '-';
            p++;
            spanLength--;
        }

        var span = new Span<char>(p, spanLength);
        TextRenderingHelpers.WriteDigits(span, goalNumber);
    }

    public void Dispose()
    {
        _byteBuffer.Dispose();
    }
}
