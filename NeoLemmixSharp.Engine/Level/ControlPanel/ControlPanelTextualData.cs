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

    private int _numberOfCharsForLemmingActionAndCount = 0;
    private int _numberOfCharsForHatchCount = 0;
    private int _numberOfCharsForLemmingsOut = 0;
    private int _numberOfCharsForGoalCount = 0;

    private readonly ControlPanelParameterSet _controlPanelParameters;
    private readonly RawArray _byteBuffer;

    private bool _isDisposed;

    public ReadOnlySpan<char> LemmingActionAndCountSpan => Helpers.CreateReadOnlySpan<char>(_lemmingActionAndCountPointer, _numberOfCharsForLemmingActionAndCount);
    public ReadOnlySpan<char> HatchCountSpan => Helpers.CreateReadOnlySpan<char>(_hatchCountPointer, _numberOfCharsForHatchCount);
    public ReadOnlySpan<char> LemmingsOutSpan => Helpers.CreateReadOnlySpan<char>(_lemmingsOutPointer, _numberOfCharsForLemmingsOut);
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
        _numberOfCharsForLemmingActionAndCount = 0;
        _numberOfCharsForHatchCount = 0;
        _numberOfCharsForLemmingsOut = 0;
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

        _numberOfCharsForLemmingActionAndCount = textLength;
    }

    private int WriteLemmingInfo(Lemming lemming)
    {
        if (lemming.HasPermanentSkill && ShowExpandedAthleteInformation())
            return WriteExpandedAthleteInformation(lemming);

        var actionType = lemming.CurrentActionType;
        return WriteMinimalAthleteInformation(lemming, actionType);
    }

    [Pure]
    private bool ShowExpandedAthleteInformation() => _controlPanelParameters.Contains(ControlPanelParameters.ShowExpandedAthleteInformation);

    private int WriteExpandedAthleteInformation(Lemming lemming)
    {
        const int TextLengthForExpandedAthleteInformation = 7;

        char* p = _lemmingActionAndCountPointer;

        *p = lemming.IsSlider ? 'L' : '-';
        p++;
        *p = lemming.IsClimber ? 'C' : '-';
        p++;
        *p = lemming.IsSwimmer ? 'S' : lemming.IsAcidLemming ? 'A' : lemming.IsWaterLemming ? 'W' : '-';
        p++;
        *p = lemming.IsFloater ? 'F' : lemming.IsGlider ? 'G' : '-';
        p++;
        *p = lemming.IsDisarmer ? 'D' : '-';
        p++;
        *p = lemming.IsZombie ? 'Z' : '-';
        p++;
        *p = lemming.IsNeutral ? 'N' : '-';

        return TextLengthForExpandedAthleteInformation;
    }

    private int WriteMinimalAthleteInformation(
        Lemming lemming,
        LemmingActionType actionType)
    {
        var sourceSpan = GetSourceString(lemming, actionType);
        Span<char> destSpan = new(_lemmingActionAndCountPointer, LemmingActionConstants.LongestActionNameLength);

        sourceSpan.CopyTo(destSpan);

        return sourceSpan.Length;

        static string GetSourceString(
            Lemming lemming,
            LemmingActionType actionType)
        {
            var cursorSelectionPriority = LemmingAction.GetCursorSelectionPriorityForActionType(actionType);

            if (cursorSelectionPriority == CursorSelectionPriority.NonPermanentSkillPriority)
                return LemmingActionConstants.GetLemmingActionNameFromId(actionType);

            if (lemming.IsZombie)
            {
                return lemming.IsNeutral
                    ? EngineConstants.NeutralZombieControlPanelString
                    : EngineConstants.ZombieControlPanelString;
            }

            if (lemming.IsNeutral)
                return EngineConstants.NeutralControlPanelString;

            var numberOfPermanentSkills = lemming.NumberOfPermanentSkills;

            return numberOfPermanentSkills switch
            {
                2 => EngineConstants.AthleteControlPanelString2Skills,
                3 => EngineConstants.AthleteControlPanelString3Skills,
                4 => EngineConstants.AthleteControlPanelString4Skills,
                5 => EngineConstants.AthleteControlPanelString5Skills,
                _ => LemmingActionConstants.GetLemmingActionNameFromId(actionType)
            };
        }
    }

    public void SetHatchData(int hatchCount)
    {
        Debug.Assert(hatchCount >= 0);

        _numberOfCharsForHatchCount = NumberFormattingHelpers.WriteDigits(_hatchCountPointer, (uint)hatchCount);
    }

    public void SetLemmingData(int lemmingCount)
    {
        Debug.Assert(lemmingCount >= 0);

        _numberOfCharsForLemmingsOut = NumberFormattingHelpers.WriteDigits(_lemmingsOutPointer, (uint)lemmingCount);
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
