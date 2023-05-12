﻿using NeoLemmixSharp.Engine.ControlPanel;
using NeoLemmixSharp.Util;
using System;

namespace NeoLemmixSharp.Engine;

public sealed class LevelCursor
{
    private readonly LevelControlPanel _controlPanel;
    private readonly LevelInputController _controller;
    private readonly Lemming[] _lemmings;

    public bool HighlightLemming { get; private set; }
    public bool LemmingsUnderCursor => _numberOfLemmingsUnderCursor > 0;

    public LevelPosition CursorPosition { get; set; }

    private Lemming? _lemmingUnderCursor;
    private int _numberOfLemmingsUnderCursor;

    public LevelCursor(LevelControlPanel controlPanel, LevelInputController controller, Lemming[] lemmings)
    {
        _controlPanel = controlPanel;
        _controller = controller;
        _lemmings = lemmings;
    }

    public void HandleMouseInput()
    {
        CheckLemmingsUnderCursor();

        if ((_controller.LeftMouseButtonStatus & MouseButtonStatusConsts.MouseButtonPressed) == MouseButtonStatusConsts.MouseButtonUnpressed)
            return;

        if (_lemmingUnderCursor != null && _controlPanel.SelectedSkill is not null)
        {
            _controlPanel.SelectedSkill.AssignToLemming(_lemmingUnderCursor);
        }
    }

    private void CheckLemmingsUnderCursor()
    {
        _lemmingUnderCursor = null;
        _numberOfLemmingsUnderCursor = 0;
        int curValue = 10;

        for (var i = 0; i < _lemmings.Length; i++)
        {
            var lemming = _lemmings[i];

            if (HighlightLemming && !false/*(L = GetHighlitLemming)*/)
            {
                continue;
            }

            /* if (lemming.IsRemoved || lemming.IsTeleporting)
             {
                 continue;
             }*/
            /*
            // Check if we only look for highlighted Lems
            if (IsHighlight and not(L = GetHighlitLemming))
            or(IsReplay and not(L = GetTargetLemming)) then Continue;
            // Does Lemming exist
            if L.LemRemoved or L.LemTeleporting then Continue;
            // Is the Lemming unable to receive skills, because zombie, neutral, or was-ohnoer? (remove unless we haven't yet had any lem under the cursor)
            if L.CannotReceiveSkills and Assigned(PriorityLem) then Continue;
            */

            // Is Lemming inside cursor (only check if we are not using Hightlightning!)
            if (!LemmingIsUnderCursor(lemming))// and(not(IsHighlight or IsReplay))
            {
                continue;
            }
            // Directional select

            /*
            if (fSelectDx <> 0) and(fSelectDx <> L.LemDx) and(not(IsHighlight or IsReplay)) then Continue;
            // Select only walkers
            if IsSelectWalkerHotkey and(L.LemAction <> baWalking) and(not(IsHighlight or IsReplay)) then Continue;
            */

            // Increase number of lemmings in cursor (if not a zombie or neutral)
            //if(lemming.CanReceiveSkills)
            //{
            _numberOfLemmingsUnderCursor++;
            //}

            // Determine priority class of current lemming
            var curPriorityBox = 0;
            if (false) //IsSelectUnassignedHotkey or IsSelectWalkerHotkey then
            {
                curPriorityBox = 1;
            }
            else
            {
                curPriorityBox = 0;
                var lemIsInBox = false;
                do
                {
                    lemIsInBox = false;
                    curPriorityBox++;
                } while (curPriorityBox > Math.Min(curValue, 4) || lemIsInBox);
            }

            // Can this lemming actually receive the skill?
            if (!_controlPanel.SelectedSkill?.CanAssignToLemming(lemming) ?? false)
            {
                curPriorityBox = 8;
            }

            // Deprioritize zombie even when just counting lemmings
            if (false)//(!lemming.CanReceiveSkills)
            {
                curPriorityBox = 9;
            }

            if (curPriorityBox < curValue || (curPriorityBox == curValue && IsCloserToCursorCentre(_lemmingUnderCursor, lemming)))
            {
                // New top priority lemming found
                _lemmingUnderCursor = lemming;
                curValue = curPriorityBox;
            }
        }

        //  Delete PriorityLem if too low-priority and we wish to assign a skill
        //  if (CurValue > 6) && !(NewSkillOrig = baNone) then PriorityLem := nil;

    }

    private bool LemmingIsUnderCursor(Lemming lemming)
    {
        var p0 = lemming.Orientation.Move(lemming.LevelPosition, -8, 10);
        var p1 = lemming.Orientation.Move(lemming.LevelPosition, 5, -3);

        return lemming.Orientation.FirstIsAboveSecond(p0, CursorPosition) &&
               lemming.Orientation.FirstIsToLeftOfSecond(p0, CursorPosition) &&
               lemming.Orientation.FirstIsBelowSecond(p1, CursorPosition) &&
               lemming.Orientation.FirstIsToRightOfSecond(p1, CursorPosition);
    }

    /*    
begin
  PriorityLem := nil;

  if fHitTestAutoFail then
  begin
    Result := 0;
    Exit;
  end;

  NumLemInCursor := 0;
  CurValue := 10;
  if NewSkillOrig = baNone then
  begin
    NewSkill := SkillPanelButtonToAction[fSelectedSkill];
    // Set NewSkill if level has no skill at all in the skillbar
    if NewSkill = baNone then NewSkill := baExploding;
  end
  else
    NewSkill := NewSkillOrig;

  for i := (LemmingList.Count - 1) downto 0 do
  begin
    L := LemmingList.List[i];

    // Check if we only look for highlighted Lems
    if (IsHighlight and not (L = GetHighlitLemming))
    or (IsReplay and not (L = GetTargetLemming)) then Continue;
    // Does Lemming exist
    if L.LemRemoved or L.LemTeleporting then Continue;
    // Is the Lemming unable to receive skills, because zombie, neutral, or was-ohnoer? (remove unless we haven't yet had any lem under the cursor)
    if L.CannotReceiveSkills and Assigned(PriorityLem) then Continue;
    // Is Lemming inside cursor (only check if we are not using Hightlightning!)
    if (not LemIsInCursor(L, MousePos)) and (not (IsHighlight or IsReplay)) then Continue;
    // Directional select
    if (fSelectDx <> 0) and (fSelectDx <> L.LemDx) and (not (IsHighlight or IsReplay)) then Continue;
    // Select only walkers
    if IsSelectWalkerHotkey and (L.LemAction <> baWalking) and (not (IsHighlight or IsReplay)) then Continue;

    // Increase number of lemmings in cursor (if not a zombie or neutral)
    if not L.CannotReceiveSkills then Inc(NumLemInCursor);

    // Determine priority class of current lemming
    if IsSelectUnassignedHotkey or IsSelectWalkerHotkey then
      CurPriorityBox := 1
    else
    begin
      CurPriorityBox := 0;
      repeat
        LemIsInBox := IsLemInPriorityBox(L, CurPriorityBox {PriorityBoxOrder[CurPriorityBox]});
        Inc(CurPriorityBox);
      until (CurPriorityBox > MinIntValue([CurValue, 4])) or LemIsInBox;
    end;

    // Can this lemmings actually receive the skill?
    if not NewSkillMethods[NewSkill](L) then CurPriorityBox := 8;

    // Deprioritize zombie even when just counting lemmings
    if L.CannotReceiveSkills then CurPriorityBox := 9;

    if     (CurPriorityBox < CurValue)
       or ((CurPriorityBox = CurValue) and IsCloserToCursorCenter(PriorityLem, L, MousePos)) then
    begin
      // New top priority lemming found
      PriorityLem := L;
      CurValue := CurPriorityBox;
    end;
  end;

end;


  function LemIsInCursor(L: TLemming; MousePos: TPoint): Boolean;
  var
    X, Y: Integer;
  begin
    X := L.LemX - 8;
    Y := L.LemY - 10;
    Result := PtInRect(Rect(X, Y, X + 13, Y + 13), MousePos);
  end;



    function TLemmingGame.GetPriorityLemming(out PriorityLem: TLemming;
                                          NewSkillOrig: TBasicLemmingAction;
                                          MousePos: TPoint;
                                          IsHighlight: Boolean = False;
                                          IsReplay: Boolean = False): Integer;
const
  NonPerm = 0;
  Perm = 1;
  NonWalk = 2;
  Walk = 3;
var
  i, CurPriorityBox: Integer;
  // CurValue = 1, 2, 3, 4: Lem is assignable and in one PriorityBox
  // CurValue = 8: Lem is unassignable, but no zombie
  // CurValue = 9: Lem is zombie
  CurValue: Integer;
  L: TLemming;
  LemIsInBox: Boolean;
  NumLemInCursor: Integer;
  NewSkill: TBasicLemmingAction;



  function IsLemInPriorityBox(L: TLemming; PriorityBox: Integer): Boolean;
  begin
    Result := True;
    case PriorityBox of
      Perm    : Result :=     L.HasPermanentSkills;
      NonPerm : Result :=     (L.LemAction in [baBashing, baFencing, baMining, baDigging, baBuilding,
                                               baPlatforming, baStacking, baBlocking, baShrugging,
                                               baReaching, baShimmying, baLasering]);
      Walk    : Result :=     (L.LemAction in [baWalking, baAscending]);
      NonWalk : Result := not (L.LemAction in [baWalking, baAscending]);
    end;
  end;



    
  function IsCloserToCursorCenter(LOld, LNew: TLemming; MousePos: TPoint): Boolean;
  begin
    Result := (GetLemDistance(LNew, MousePos) < GetLemDistance(LOld, MousePos));
  end;

  function GetLemDistance(L: TLemming; MousePos: TPoint): Integer;
  begin
    // We compute the distance to the center of the cursor after 2x-zooming, to have integer values
    Result :=   Sqr(2 * (L.LemX - 8) - 2 * MousePos.X + 13)
              + Sqr(2 * (L.LemY - 10) - 2 * MousePos.Y + 13)
  end;

    */

    private bool IsCloserToCursorCentre(Lemming? previousCandidate, Lemming newCandidate)
    {
        return false;
    }
}