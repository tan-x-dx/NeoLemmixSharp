using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
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

    private readonly char* _cursorDataPointer;
    private readonly char* _hatchCountPointer;
    private readonly char* _lemmingsOutPointer;
    private readonly char* _goalCountPointer;

    private readonly ControlPanelParameterSet _controlPanelParameters;
    private readonly RawArray _byteBuffer;

    private bool _isDisposed;

    public ReadOnlySpan<char> LemmingActionAndCountSpan => new(_cursorDataPointer, CharLengthForCursorData);
    public ReadOnlySpan<char> HatchCountSpan => new(_hatchCountPointer, CharLengthForLemmingCount);
    public ReadOnlySpan<char> LemmingsOutSpan => new(_lemmingsOutPointer, CharLengthForLemmingCount);
    public ReadOnlySpan<char> GoalCountSpan => new(_goalCountPointer, CharLengthForGoalCount);

    public ControlPanelTextualData(ControlPanelParameterSet controlPanelParameters)
    {
        _controlPanelParameters = controlPanelParameters;

        _byteBuffer = Helpers.AllocateBuffer<char>(TotalControlPanelTextLength);

        _cursorDataPointer = (char*)_byteBuffer.Handle;
        _hatchCountPointer = (char*)_byteBuffer.Handle + CharLengthForCursorData;
        _lemmingsOutPointer = (char*)_byteBuffer.Handle + (CharLengthForCursorData + CharLengthForLemmingCount);
        _goalCountPointer = (char*)_byteBuffer.Handle + (CharLengthForCursorData + CharLengthForLemmingCount + CharLengthForLemmingCount);
    }

    public void ClearTextualData()
    {
        new Span<byte>((void*)_byteBuffer.Handle, TotalControlPanelTextLength * sizeof(char)).Clear();
    }

    public void SetCursorData(Lemming lemmingUnderCursor, uint numberOfLemmingsUnderCursor)
    {
        var textLength = WriteLemmingInfo(lemmingUnderCursor);

        char* p = _cursorDataPointer + textLength;
        *p = ' '; // Add a space.
        p++;
        NumberFormattingHelpers.WriteDigits(p, numberOfLemmingsUnderCursor);
    }

    private int WriteLemmingInfo(Lemming lemming)
    {
        var state = lemming.State;

        if (state.HasPermanentSkill && ShowExpandedAthleteInformation())
            return WriteExpandedAthleteInformation(state);

        var action = lemming.CurrentAction;
        return WriteMinimalAthleteInformation(action, state);
    }

    [Pure]
    private bool ShowExpandedAthleteInformation() => _controlPanelParameters.Contains(ControlPanelParameters.ShowExpandedAthleteInformation);

    private int WriteExpandedAthleteInformation(LemmingState state)
    {
        const int TextLengthForExpandedAthleteInformation = 7;

        char* p = _cursorDataPointer;

        *p = state.IsSlider ? 'L' : '-';
        p++;
        *p = state.IsClimber ? 'C' : '-';
        p++;
        *p = state.IsSwimmer ? 'S' : state.IsAcidLemming ? 'A' : state.IsWaterLemming ? 'W' : '-';
        p++;
        *p = state.IsFloater ? 'F' : state.IsGlider ? 'G' : '-';
        p++;
        *p = state.IsDisarmer ? 'D' : '-';
        p++;
        *p = state.IsZombie ? 'Z' : '-';
        p++;
        *p = state.IsNeutral ? 'N' : '-';

        return TextLengthForExpandedAthleteInformation;
    }

    private int WriteMinimalAthleteInformation(
        LemmingAction action,
        LemmingState state)
    {
        ReadOnlySpan<char> sourceSpan = GetSourceString(action, state).AsSpan();
        Span<char> destSpan = new(_cursorDataPointer, CharLengthForCursorData);

        sourceSpan.CopyTo(destSpan);

        return sourceSpan.Length;

        static string GetSourceString(
            LemmingAction action,
            LemmingState state)
        {
            if (action.CursorSelectionPriorityValue == LemmingActionConstants.NonPermanentSkillPriority)
                return action.LemmingActionName;

            if (state.IsZombie)
            {
                return state.IsNeutral
                    ? EngineConstants.NeutralZombieControlPanelString
                    : EngineConstants.ZombieControlPanelString;
            }

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

    public void SetHatchData(int hatchCount)
    {
        Debug.Assert(hatchCount >= 0);

        NumberFormattingHelpers.WriteDigits(_hatchCountPointer, (uint)hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        Debug.Assert(lemmingCount >= 0);

        NumberFormattingHelpers.WriteDigits(_lemmingsOutPointer, (uint)lemmingCount);
    }

    public void SetGoalData(int goalNumber)
    {
        char* p = _goalCountPointer;
        if (goalNumber < 0)
        {
            goalNumber = -goalNumber;
            *p = '-';
            p++;
        }

        NumberFormattingHelpers.WriteDigits(p, (uint)goalNumber);
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            _byteBuffer.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
