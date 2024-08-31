using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Timer;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class ControlPanelTextualData
{
    private const int CharLengthForLemmingActionAndCount = LevelConstants.LongestActionNameLength + // Enough space for the action part
                                                           1 + // Add a space
                                                           CharLengthForLemmingCount; // Lemmings under cursor

    private const int CharLengthForHatchCount = 3;
    private const int CharLengthForLemmingCount = 4;
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
        Span<int> destSpan = _lemmingActionAndCountString;
        TextRenderingHelpers.WriteDigits(destSpan.Slice(textLength, spanLength), numberOfLemmingsUnderCursor);
    }

    private static bool ShowAthleteInformation() => false;

    private int WriteLemmingInfo(Lemming lemming)
    {
        Span<int> destSpan = _lemmingActionAndCountString;
        destSpan.Clear();

        var state = lemming.State;

        if (state.HasPermanentSkill && ShowAthleteInformation())
        {
            destSpan = destSpan[..7];
            destSpan.Fill('-');

            if (state.IsSlider) destSpan[0] = 'L';
            if (state.IsClimber) destSpan[1] = 'C';
            if (state.IsSwimmer) destSpan[2] = 'S';
            if (state.IsFloater) destSpan[3] = 'F';
            else if (state.IsGlider) destSpan[3] = 'G';
            if (state.IsDisarmer) destSpan[4] = 'D';
            if (state.IsZombie) destSpan[5] = 'Z';
            if (state.IsNeutral) destSpan[6] = 'N';

            return 7;
        }

        var action = lemming.CurrentAction;

        var isZombie = state.IsZombie;
        var isNeutral = state.IsNeutral;
        if (action.CursorSelectionPriorityValue == LevelConstants.NonPermanentSkillPriority)
        {
            goto UseActionName;
        }

        ReadOnlySpan<int> sourceSpan;

        if (isZombie && isNeutral)
        {
            sourceSpan = LevelConstants.NeutralZombieStringNumericalSpan;
        }
        else if (isZombie)
        {
            sourceSpan = LevelConstants.ZombieStringNumericalSpan;
        }
        else if (isNeutral)
        {
            sourceSpan = LevelConstants.NeutralStringNumericalSpan;
        }
        else
        {
            var numberOfPermanentSkills = state.NumberOfPermanentSkills;

            switch (numberOfPermanentSkills)
            {
                case 2: sourceSpan = LevelConstants.AthleteString2Skills; break;
                case 3: sourceSpan = LevelConstants.AthleteString3Skills; break;
                case 4: sourceSpan = LevelConstants.AthleteString4Skills; break;
                case 5: sourceSpan = LevelConstants.AthleteString5Skills; break;
                default: goto UseActionName;
            };
        }

        sourceSpan.CopyTo(destSpan);

        return sourceSpan.Length;

        UseActionName:

        for (var i = 0; i < action.LemmingActionName.Length; i++)
        {
            destSpan[i] = action.LemmingActionName[i];
        }

        return action.LemmingActionName.Length;
    }

    /*
    function TBaseSkillPanel.GetSkillString(L: TLemming): String;
    var
    i: Integer;

    procedure DoInc(aText: String);
    begin
    Inc(i);
    case i of
      1: Result := aText;
      2: Result := SAthlete;
      3: Result := STriathlete;
      4: Result := SQuadathlete;
      5: Result := SQuintathlete
    end;
    end;
    begin
    Result := '';
    if L = nil then Exit;

    Result := LemmingActionStrings[L.LemAction];

    if L.HasPermanentSkills and GameParams.Hotkeys.CheckForKey(lka_ShowAthleteInfo) then
    begin
    Result := '-------';
    if L.LemIsSlider then Result[1] := 'L';    
    if L.LemIsClimber then Result[2] := 'C';
    if L.LemIsSwimmer then Result[3] := 'S';
    if L.LemIsFloater then Result[4] := 'F';
    if L.LemIsGlider then Result[4] := 'G';
    if L.LemIsDisarmer then Result[5] := 'D';
    if L.LemIsZombie then Result[6] := 'Z';
    if L.LemIsNeutral then Result[7] := 'N';
    end
    else if not (L.LemAction in [baBuilding, baPlatforming, baStacking, baLasering, baBashing, baMining, baDigging, baBlocking]) then
    begin
    i := 0;
    if L.LemIsSlider then DoInc(SSlider);
    if L.LemIsClimber then DoInc(SClimber);
    if L.LemIsSwimmer then DoInc(SSwimmer);
    if L.LemIsFloater then DoInc(SFloater);
    if L.LemIsGlider then DoInc(SGlider);
    if L.LemIsDisarmer then DoInc(SDisarmer);
    if L.LemIsZombie then Result := SZombie;
    if L.LemIsNeutral then Result := SNeutral;
    if L.LemIsZombie and L.LemIsNeutral then Result := SNeutralZombie;
    end;
    end;
    */

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