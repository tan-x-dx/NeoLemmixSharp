using NeoLemmixSharp.Engine.LemmingSkills;
using NeoLemmixSharp.LevelBuilding.Data;
using NeoLemmixSharp.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoLemmixSharp.Engine.ControlPanel;

public sealed class LevelControlPanel : ILevelControlPanel
{
    public const int MaxNumberOfSkillButtons = 10;
    private const int ControlPanelButtonPixelWidth = 16;
    private const int ControlPanelButtonPixelHeight = 23;
    private const int ControlPanelInfoPixelHeight = 16;
    private const int ControlPanelTotalPixelHeight = ControlPanelButtonPixelHeight + ControlPanelInfoPixelHeight;
    private const int MinControlPanelScaleMultiplier = 4;
    private const int MaxControlPanelScaleMultiplier = 6;

    private readonly LevelInputController _controller;

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

    public int ScreenWidth { get; private set; }
    public int ScreenHeight { get; private set; }
    public int HorizontalButtonScreenSpace { get; private set; }
    public int ControlPanelX { get; private set; }
    public int ControlPanelY { get; private set; }
    public int ControlPanelButtonScreenWidth { get; private set; }
    public int ControlPanelButtonScreenHeight { get; private set; }
    public int ControlPanelInfoScreenHeight { get; private set; }
    public int ControlPanelButtonY { get; private set; }

    public int SkillPanelScroll { get; private set; }

    public int ControlPanelScreenHeight { get; private set; }

    public SkillAssignButton? SelectedSkillAssignButton { get; private set; }
    public LemmingSkill SelectedSkill => SelectedSkillAssignButton?.LemmingSkill ?? NoneSkill.Instance;

    public LevelControlPanel(SkillSet skillSet, LevelInputController controller)
    {
        _controller = controller;
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

        if (skillSet.NumberOfBashers.HasValue) { AddSkillAssignmentButton(new BasherSkill(skillSet.NumberOfBashers.Value)); }
        if (skillSet.NumberOfBlockers.HasValue) { AddSkillAssignmentButton(new BlockerSkill(skillSet.NumberOfBlockers.Value)); }
        if (skillSet.NumberOfBombers.HasValue) { AddSkillAssignmentButton(new BomberSkill(skillSet.NumberOfBombers.Value)); }
        if (skillSet.NumberOfBuilders.HasValue) { AddSkillAssignmentButton(new BuilderSkill(skillSet.NumberOfBuilders.Value)); }
        if (skillSet.NumberOfClimbers.HasValue) { AddSkillAssignmentButton(new ClimberSkill(skillSet.NumberOfClimbers.Value)); }
        if (skillSet.NumberOfCloners.HasValue) { AddSkillAssignmentButton(new ClonerSkill(skillSet.NumberOfCloners.Value)); }
        if (skillSet.NumberOfDiggers.HasValue) { AddSkillAssignmentButton(new DiggerSkill(skillSet.NumberOfDiggers.Value)); }
        if (skillSet.NumberOfDisarmers.HasValue) { AddSkillAssignmentButton(new DisarmerSkill(skillSet.NumberOfDisarmers.Value)); }
        if (skillSet.NumberOfFencers.HasValue) { AddSkillAssignmentButton(new FencerSkill(skillSet.NumberOfFencers.Value)); }
        if (skillSet.NumberOfFloaters.HasValue) { AddSkillAssignmentButton(new FloaterSkill(skillSet.NumberOfFloaters.Value)); }
        if (skillSet.NumberOfGliders.HasValue) { AddSkillAssignmentButton(new GliderSkill(skillSet.NumberOfGliders.Value)); }
        if (skillSet.NumberOfJumpers.HasValue) { AddSkillAssignmentButton(new JumperSkill(skillSet.NumberOfJumpers.Value)); }
        if (skillSet.NumberOfLaserers.HasValue) { AddSkillAssignmentButton(new LasererSkill(skillSet.NumberOfLaserers.Value)); }
        if (skillSet.NumberOfMiners.HasValue) { AddSkillAssignmentButton(new MinerSkill(skillSet.NumberOfMiners.Value)); }
        if (skillSet.NumberOfPlatformers.HasValue) { AddSkillAssignmentButton(new PlatformerSkill(skillSet.NumberOfPlatformers.Value)); }
        if (skillSet.NumberOfShimmiers.HasValue) { AddSkillAssignmentButton(new ShimmierSkill(skillSet.NumberOfShimmiers.Value)); }
        if (skillSet.NumberOfSliders.HasValue) { AddSkillAssignmentButton(new SliderSkill(skillSet.NumberOfSliders.Value)); }
        if (skillSet.NumberOfStackers.HasValue) { AddSkillAssignmentButton(new StackerSkill(skillSet.NumberOfStackers.Value)); }
        if (skillSet.NumberOfStoners.HasValue) { AddSkillAssignmentButton(new StonerSkill(skillSet.NumberOfStoners.Value)); }
        if (skillSet.NumberOfSwimmers.HasValue) { AddSkillAssignmentButton(new SwimmerSkill(skillSet.NumberOfSwimmers.Value)); }
        if (skillSet.NumberOfWalkers.HasValue) { AddSkillAssignmentButton(new WalkerSkill(skillSet.NumberOfWalkers.Value)); }

        return tempList.ToArray();

        void AddSkillAssignmentButton(LemmingSkill lemmingSkill)
        {
            tempList.Add(new SkillAssignButton(lemmingSkill, i));
            i = (i + 1) & 7;
        }
    }

    public IEnumerable<SkillAssignButton> SkillAssignButtons => _skillAssignButtons;

    public void SetWindowDimensions(int screenWidth, int screenHeight)
    {
        ScreenWidth = screenWidth;
        ScreenHeight = screenHeight;

        RecalculateButtonDimensions();
    }

    public void SetPanelScale(int scale)
    {
        _controlPanelScale = Math.Clamp(scale, MinControlPanelScaleMultiplier, MaxControlPanelScaleMultiplier);

        RecalculateButtonDimensions();
    }

    private void RecalculateButtonDimensions()
    {
        // 19 = 10 skill buttons + 9 other buttons
        HorizontalButtonScreenSpace = 19 * ControlPanelButtonPixelWidth * _controlPanelScale;

        ControlPanelX = (ScreenWidth - HorizontalButtonScreenSpace) / 2;
        ControlPanelY = ScreenHeight - (ControlPanelTotalPixelHeight * _controlPanelScale);

        ControlPanelButtonScreenWidth = ControlPanelButtonPixelWidth * _controlPanelScale;
        ControlPanelButtonScreenHeight = ControlPanelButtonPixelHeight * _controlPanelScale;
        ControlPanelInfoScreenHeight = ControlPanelInfoPixelHeight * _controlPanelScale;
        ControlPanelScreenHeight = ControlPanelTotalPixelHeight * _controlPanelScale;

        ControlPanelButtonY = ControlPanelY + ControlPanelInfoScreenHeight;

        var x0 = ControlPanelX;
        var y0 = ControlPanelButtonY;
        var h0 = ControlPanelButtonScreenHeight;

        UpdateButtonDimensions(_releaseRateMinusButton);
        x0 += ControlPanelButtonScreenWidth;
        UpdateButtonDimensions(_releaseRatePlusButton);

        UpdateSkillAssignButtonDimensions();

        x0 = ControlPanelButtonScreenWidth * 12;

        UpdateButtonDimensions(_pauseButton);
        x0 += ControlPanelButtonScreenWidth;
        UpdateButtonDimensions(_nukeButton);
        x0 += ControlPanelButtonScreenWidth;
        UpdateButtonDimensions(_fastForwardButton);
        x0 += ControlPanelButtonScreenWidth;
        UpdateButtonDimensions(_restartButton);
        x0 += ControlPanelButtonScreenWidth;
        h0 /= 2;

        UpdateButtonDimensions(_frameSkipBackButton);
        y0 += h0;
        UpdateButtonDimensions(_frameSkipForwardButton);

        y0 -= h0;
        x0 += ControlPanelButtonScreenWidth;

        UpdateButtonDimensions(_directionSelectLeftButton);
        y0 += h0;
        UpdateButtonDimensions(_directionSelectRightButton);

        y0 -= h0;
        x0 += ControlPanelButtonScreenWidth;

        UpdateButtonDimensions(_clearPhysicsButton);
        y0 += h0;
        UpdateButtonDimensions(_replayButton);

        void UpdateButtonDimensions(ControlPanelButton button)
        {
            button.ScreenX = x0;
            button.ScreenY = y0;
            button.ScreenWidth = ControlPanelButtonScreenWidth;
            button.ScreenHeight = h0;
            button.ScaleMultiplier = _controlPanelScale;
        }
    }

    private void UpdateSkillAssignButtonDimensions()
    {
        var indexOfLastSkillAssignButtonToRender = SkillPanelScroll + MaxNumberOfSkillButtons;

        var x0 = ControlPanelButtonScreenWidth * (2 - SkillPanelScroll);

        for (var i = 0; i < _skillAssignButtons.Length; i++)
        {
            var button = _skillAssignButtons[i];
            button.ScreenX = x0;
            button.ScreenY = ControlPanelButtonY;
            button.ScreenWidth = ControlPanelButtonScreenWidth;
            button.ScreenHeight = ControlPanelButtonScreenHeight;
            button.ScaleMultiplier = _controlPanelScale;
            _skillAssignButtons[i].ShouldRender = i >= SkillPanelScroll && i < indexOfLastSkillAssignButtonToRender;
            x0 += ControlPanelButtonScreenWidth;
        }
    }

    public void HandleMouseInput()
    {
        if (_skillAssignButtons.Length > MaxNumberOfSkillButtons)
        {
            TrackScrollWheel();
        }

        if (_controller.LeftMouseButtonStatus != MouseButtonStatusConsts.MouseButtonPressed)
            return;

        var mouseX = _controller.MouseX;
        var mouseY = _controller.MouseY;
        foreach (var skillAssignButton in _skillAssignButtons)
        {
            if (skillAssignButton.TryPress(mouseX, mouseY))
            {
                SetSelectedSkillAssignmentButton(skillAssignButton);
                return;
            }
        }
    }

    private void TrackScrollWheel()
    {
        var previousValue = SkillPanelScroll;
        SkillPanelScroll = Math.Clamp(SkillPanelScroll - _controller.ScrollDelta, 0, _maxSkillPanelScroll);
        if (SkillPanelScroll == previousValue)
            return;

        UpdateSkillAssignButtonDimensions();
    }

    private void SetSelectedSkillAssignmentButton(SkillAssignButton? skillAssignButton)
    {
        if (SelectedSkillAssignButton != null)
        {
            SelectedSkillAssignButton.IsSelected = false;
        }

        SelectedSkillAssignButton = skillAssignButton;

        if (SelectedSkillAssignButton != null)
        {
            SelectedSkillAssignButton.IsSelected = true;
        }
    }
}