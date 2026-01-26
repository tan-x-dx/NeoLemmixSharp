using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public unsafe sealed class ControlPanelTextualData : IDisposable
{
    private const int CharLengthForLemmingActionAndCount = LemmingActionConstants.LongestActionNameLength + // Enough space for the action part
                                                           1 + // Add a space
                                                           CharLengthForLemmingCount; // Lemmings under cursor

    private const int CharLengthForLemmingCount = NumberFormattingHelpers.Uint32NumberBufferLength;
    private const int CharLengthForGoalCount = NumberFormattingHelpers.Int32NumberBufferLength;

    public const int TotalControlPanelTextLength = CharLengthForLemmingActionAndCount +
                                                   CharLengthForLemmingCount +
                                                   CharLengthForLemmingCount +
                                                   CharLengthForGoalCount;

    private readonly char* _lemmingActionAndCountPointer;
    private readonly char* _hatchCountPointer;
    private readonly char* _lemmingsOutPointer;
    private readonly char* _goalCountPointer;

    private int _numberOfCharsForlemmingActionAndCount = 0;
    private int _numberOfCharsForhatchCount = 0;
    private int _numberOfCharsForlemmingsOut = 0;
    private int _numberOfCharsForGoalCount = 0;

    private readonly ControlPanelParameterSet _controlPanelParameters;
    private readonly RawArray _byteBuffer;

    private bool _isDisposed;

    public ReadOnlySpan<char> LemmingActionAndCountSpan => Helpers.CreateReadOnlySpan<char>(_lemmingActionAndCountPointer, _numberOfCharsForlemmingActionAndCount);
    public ReadOnlySpan<char> HatchCountSpan => Helpers.CreateReadOnlySpan<char>(_hatchCountPointer, _numberOfCharsForhatchCount);
    public ReadOnlySpan<char> LemmingsOutSpan => Helpers.CreateReadOnlySpan<char>(_lemmingsOutPointer, _numberOfCharsForlemmingsOut);
    public ReadOnlySpan<char> GoalCountSpan => Helpers.CreateReadOnlySpan<char>(_goalCountPointer, _numberOfCharsForGoalCount);

    public ControlPanelTextualData(ControlPanelParameterSet controlPanelParameters)
    {
        _controlPanelParameters = controlPanelParameters;

        _byteBuffer = Helpers.AllocateBuffer<char>(TotalControlPanelTextLength);

        _lemmingActionAndCountPointer = (char*)_byteBuffer.Handle;
        _hatchCountPointer = (char*)_byteBuffer.Handle + CharLengthForLemmingActionAndCount;
        _lemmingsOutPointer = (char*)_byteBuffer.Handle + (CharLengthForLemmingActionAndCount + CharLengthForLemmingCount);
        _goalCountPointer = (char*)_byteBuffer.Handle + (CharLengthForLemmingActionAndCount + CharLengthForLemmingCount + CharLengthForLemmingCount);
    }

    public void ClearTextualData()
    {
        _numberOfCharsForlemmingActionAndCount = 0;
        _numberOfCharsForhatchCount = 0;
        _numberOfCharsForlemmingsOut = 0;
        _numberOfCharsForGoalCount = 0;
    }

    public void SetCursorData(Lemming lemmingUnderCursor, uint numberOfLemmingsUnderCursor)
    {
        var textLength = WriteLemmingInfo(lemmingUnderCursor);

        char* p = _lemmingActionAndCountPointer + textLength;
        *p = ' '; // Add a space.
        p++;
        textLength++;
        textLength += NumberFormattingHelpers.WriteDigits(p, numberOfLemmingsUnderCursor);

        _numberOfCharsForlemmingActionAndCount = textLength;
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

        char* p = _lemmingActionAndCountPointer;

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
        Span<char> destSpan = new(_lemmingActionAndCountPointer, LemmingActionConstants.LongestActionNameLength);

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

        _numberOfCharsForhatchCount = NumberFormattingHelpers.WriteDigits(_hatchCountPointer, (uint)hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        Debug.Assert(lemmingCount >= 0);

        _numberOfCharsForlemmingsOut = NumberFormattingHelpers.WriteDigits(_lemmingsOutPointer, (uint)lemmingCount);
    }

    public void SetGoalData(int goalNumber)
    {
        var textLength = 0;
        char* p = _goalCountPointer;
        if (goalNumber < 0)
        {
            goalNumber = -goalNumber;
            *p = '-';
            p++;
            textLength++;
        }

        textLength += NumberFormattingHelpers.WriteDigits(p, (uint)goalNumber);
        _numberOfCharsForGoalCount = textLength;
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
