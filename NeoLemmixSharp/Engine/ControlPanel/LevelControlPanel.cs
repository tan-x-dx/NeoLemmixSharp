using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.LevelBuilding.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class LevelControlPanel
{
    private const int MaxNumberOfSkillButtons = 10;
    private const int ControlPanelButtonPixelWidth = 16;
    private const int ControlPanelButtonPixelHeight = 23;
    private const int ControlPanelInfoPixelHeight = 16;
    private const int ControlPanelTotalPixelHeight = ControlPanelButtonPixelHeight + ControlPanelInfoPixelHeight;

    private readonly ControlPanelButton _releaseRateMinusButton;
    private readonly ControlPanelButton _releaseRatePlusButton;

    private readonly SkillAssignButton[] _skillAssignButtons;

    private readonly ControlPanelButton _pauseButton;
    private readonly ControlPanelButton _nukeButton;
    private readonly ControlPanelButton _fastForwardButton;
    private readonly ControlPanelButton _restartButton;
    private readonly ControlPanelButton _frameSkipBackButton;
    private readonly ControlPanelButton _frameSkipForwardButton;
    private readonly ControlPanelButton _directionSelectLeftButton;
    private readonly ControlPanelButton _directionSelectRightButton;
    private readonly ControlPanelButton _clearPhysicsButton;
    private readonly ControlPanelButton _replayButton;

    private readonly int _maxSkillPanelScroll;

    private int _controlPanelScale = 4;
    private SkillAssignButton? _selectedSkillAssignButton;

    private int _horizontalButtonScreenSpace;
    private int _controlPanelX;
    private int _controlPanelY;
    private int _controlPanelButtonScreenWidth;
    private int _controlPanelButtonScreenHeight;
    private int _controlPanelInfoScreenHeight;
    private int _controlPanelButtonY;

    private int _skillPanelScroll;
    private int _previousScrollWheelValue;

    public int ControlPanelScreenHeight { get; private set; }

    public LevelControlPanel(SkillSet skillSet)
    {
        _releaseRateMinusButton = new ControlPanelButton(0);
        _releaseRatePlusButton = new ControlPanelButton(1);

        _skillAssignButtons = CreateSkillAssignButtons(skillSet);

        _maxSkillPanelScroll = _skillAssignButtons.Length - MaxNumberOfSkillButtons;

        _pauseButton = new ControlPanelButton(3);
        _nukeButton = new ControlPanelButton(4);
        _fastForwardButton = new ControlPanelButton(5);
        _restartButton = new ControlPanelButton(6);
        _frameSkipBackButton = new ControlPanelButton(7);
        _frameSkipForwardButton = new ControlPanelButton(0);
        _directionSelectLeftButton = new ControlPanelButton(1);
        _directionSelectRightButton = new ControlPanelButton(2);
        _clearPhysicsButton = new ControlPanelButton(3);
        _replayButton = new ControlPanelButton(4);

        SetSelectedSkillAssignmentButton(_skillAssignButtons.FirstOrDefault());
    }

    private static SkillAssignButton[] CreateSkillAssignButtons(SkillSet skillSet)
    {
        var tempList = new List<SkillAssignButton>();

        var i = 2;

        if (skillSet.NumberOfBashers.HasValue) { AddSkillAssignmentButton(BasherSkill.Instance, skillSet.NumberOfBashers.Value); }
        if (skillSet.NumberOfBlockers.HasValue) { AddSkillAssignmentButton(BlockerSkill.Instance, skillSet.NumberOfBlockers.Value); }
        if (skillSet.NumberOfBombers.HasValue) { AddSkillAssignmentButton(BomberSkill.Instance, skillSet.NumberOfBombers.Value); }
        if (skillSet.NumberOfBuilders.HasValue) { AddSkillAssignmentButton(BuilderSkill.Instance, skillSet.NumberOfBuilders.Value); }
        if (skillSet.NumberOfClimbers.HasValue) { AddSkillAssignmentButton(ClimberSkill.Instance, skillSet.NumberOfClimbers.Value); }
        if (skillSet.NumberOfCloners.HasValue) { AddSkillAssignmentButton(ClonerSkill.Instance, skillSet.NumberOfCloners.Value); }
        if (skillSet.NumberOfDiggers.HasValue) { AddSkillAssignmentButton(DiggerSkill.Instance, skillSet.NumberOfDiggers.Value); }
        if (skillSet.NumberOfDisarmers.HasValue) { AddSkillAssignmentButton(DisarmerSkill.Instance, skillSet.NumberOfDisarmers.Value); }
        if (skillSet.NumberOfFencers.HasValue) { AddSkillAssignmentButton(FencerSkill.Instance, skillSet.NumberOfFencers.Value); }
        if (skillSet.NumberOfFloaters.HasValue) { AddSkillAssignmentButton(FloaterSkill.Instance, skillSet.NumberOfFloaters.Value); }
        if (skillSet.NumberOfGliders.HasValue) { AddSkillAssignmentButton(GliderSkill.Instance, skillSet.NumberOfGliders.Value); }
        if (skillSet.NumberOfJumpers.HasValue) { AddSkillAssignmentButton(JumperSkill.Instance, skillSet.NumberOfJumpers.Value); }
        if (skillSet.NumberOfLaserers.HasValue) { AddSkillAssignmentButton(LasererSkill.Instance, skillSet.NumberOfLaserers.Value); }
        if (skillSet.NumberOfMiners.HasValue) { AddSkillAssignmentButton(MinerSkill.Instance, skillSet.NumberOfMiners.Value); }
        if (skillSet.NumberOfPlatformers.HasValue) { AddSkillAssignmentButton(PlatformerSkill.Instance, skillSet.NumberOfPlatformers.Value); }
        if (skillSet.NumberOfShimmiers.HasValue) { AddSkillAssignmentButton(ShimmierSkill.Instance, skillSet.NumberOfShimmiers.Value); }
        if (skillSet.NumberOfSliders.HasValue) { AddSkillAssignmentButton(SliderSkill.Instance, skillSet.NumberOfSliders.Value); }
        if (skillSet.NumberOfStackers.HasValue) { AddSkillAssignmentButton(StackerSkill.Instance, skillSet.NumberOfStackers.Value); }
        if (skillSet.NumberOfStoners.HasValue) { AddSkillAssignmentButton(StonerSkill.Instance, skillSet.NumberOfStoners.Value); }
        if (skillSet.NumberOfSwimmers.HasValue) { AddSkillAssignmentButton(SwimmerSkill.Instance, skillSet.NumberOfSwimmers.Value); }
        if (skillSet.NumberOfWalkers.HasValue) { AddSkillAssignmentButton(WalkerSkill.Instance, skillSet.NumberOfWalkers.Value); }

        return tempList.ToArray();

        void AddSkillAssignmentButton(LemmingSkill lemmingSkill, int amount)
        {
            tempList.Add(new SkillAssignButton(lemmingSkill, amount, i = (i + 1) & 7));
        }
    }

    public IEnumerable<SkillAssignButton> SkillAssignButtons => _skillAssignButtons;

    public void SetWindowDimensions(int screenWidth, int screenHeight)
    {
        // 19 = 10 skill buttons + 9 other buttons
        _horizontalButtonScreenSpace = 19 * ControlPanelButtonPixelWidth * _controlPanelScale;

        _controlPanelX = (screenWidth - _horizontalButtonScreenSpace) / 2;
        _controlPanelY = screenHeight - (ControlPanelTotalPixelHeight * _controlPanelScale);

        _controlPanelButtonScreenWidth = ControlPanelButtonPixelWidth * _controlPanelScale;
        _controlPanelButtonScreenHeight = ControlPanelButtonPixelHeight * _controlPanelScale;
        _controlPanelInfoScreenHeight = ControlPanelInfoPixelHeight * _controlPanelScale;
        ControlPanelScreenHeight = ControlPanelTotalPixelHeight * _controlPanelScale;

        _controlPanelButtonY = _controlPanelY + _controlPanelInfoScreenHeight;

        var x0 = _controlPanelX;
        var y0 = _controlPanelButtonY;
        var h0 = _controlPanelButtonScreenHeight;

        UpdateButtonDimensions(_releaseRateMinusButton);
        x0 += _controlPanelButtonScreenWidth;
        UpdateButtonDimensions(_releaseRatePlusButton);

        UpdateSkillAssignButtonDimensions();

        x0 = _controlPanelButtonScreenWidth * 12;

        UpdateButtonDimensions(_pauseButton);
        x0 += _controlPanelButtonScreenWidth;
        UpdateButtonDimensions(_nukeButton);
        x0 += _controlPanelButtonScreenWidth;
        UpdateButtonDimensions(_fastForwardButton);
        x0 += _controlPanelButtonScreenWidth;
        UpdateButtonDimensions(_restartButton);
        x0 += _controlPanelButtonScreenWidth;
        h0 /= 2;

        UpdateButtonDimensions(_frameSkipBackButton);
        y0 += h0;
        UpdateButtonDimensions(_frameSkipForwardButton);

        y0 -= h0;
        x0 += _controlPanelButtonScreenWidth;

        UpdateButtonDimensions(_directionSelectLeftButton);
        y0 += h0;
        UpdateButtonDimensions(_directionSelectRightButton);

        y0 -= h0;
        x0 += _controlPanelButtonScreenWidth;

        UpdateButtonDimensions(_clearPhysicsButton);
        y0 += h0;
        UpdateButtonDimensions(_replayButton);

        void UpdateButtonDimensions(ControlPanelButton button)
        {
            button.ScreenX = x0;
            button.ScreenY = y0;
            button.ScreenWidth = _controlPanelButtonScreenWidth;
            button.ScreenHeight = h0;
            button.ScaleMultiplier = _controlPanelScale;
        }
    }

    private void UpdateSkillAssignButtonDimensions()
    {
        var indexOfLastSkillAssignButtonToRender = _skillPanelScroll + MaxNumberOfSkillButtons;

        var x0 = _controlPanelButtonScreenWidth * (2 - _skillPanelScroll);

        for (var i = 0; i < _skillAssignButtons.Length; i++)
        {
            var button = _skillAssignButtons[i];
            button.ScreenX = x0;
            button.ScreenY = _controlPanelButtonY;
            button.ScreenWidth = _controlPanelButtonScreenWidth;
            button.ScreenHeight = _controlPanelButtonScreenHeight;
            button.ScaleMultiplier = _controlPanelScale;
            _skillAssignButtons[i].ShouldRender = i >= _skillPanelScroll && i < indexOfLastSkillAssignButtonToRender;
            x0 += _controlPanelButtonScreenWidth;
        }
    }

    public void HandleMouseInput(MouseState mouseState)
    {
        TrackScrollWheel(mouseState.ScrollWheelValue);

        if (mouseState.LeftButton != ButtonState.Pressed)
            return;

        var mouseX = mouseState.X;
        var mouseY = mouseState.Y;
        foreach (var skillAssignButton in _skillAssignButtons)
        {
            if (skillAssignButton.TryPress(mouseX, mouseY))
            {
                SetSelectedSkillAssignmentButton(skillAssignButton);
            }
        }
    }

    private void TrackScrollWheel(int scrollWheelValue)
    {
        var delta = scrollWheelValue - _previousScrollWheelValue;
        _previousScrollWheelValue = scrollWheelValue;

        if (delta > 0)
        {
            ScrollSkillPanel(-1);
        }
        else if (delta < 0)
        {
            ScrollSkillPanel(1);
        }
    }

    private void ScrollSkillPanel(int delta)
    {
        var previousValue = _skillPanelScroll;
        _skillPanelScroll = Math.Clamp(_skillPanelScroll + delta, 0, _maxSkillPanelScroll);
        if (_skillPanelScroll == previousValue)
            return;

        UpdateSkillAssignButtonDimensions();
    }

    private void SetSelectedSkillAssignmentButton(SkillAssignButton? skillAssignButton)
    {
        if (_selectedSkillAssignButton != null)
        {
            _selectedSkillAssignButton.IsSelected = false;
        }

        _selectedSkillAssignButton = skillAssignButton;

        if (_selectedSkillAssignButton != null)
        {
            _selectedSkillAssignButton.IsSelected = true;
        }
    }
}