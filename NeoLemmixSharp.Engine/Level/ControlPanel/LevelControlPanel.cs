using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class LevelControlPanel : ILevelControlPanel
{
    public const int MaxNumberOfSkillButtons = 10;
    private const int NumberOfTechnicalButtons = 9;
    private const int ControlPanelButtonPixelWidth = 16;
    private const int ControlPanelButtonPixelHeight = 23;
    private const int ControlPanelInfoPixelHeight = 16;
    private const int ControlPanelTotalPixelHeight = ControlPanelButtonPixelHeight + ControlPanelInfoPixelHeight;
    private const int MinControlPanelScaleMultiplier = 4;
    private const int MaxControlPanelScaleMultiplier = 6;

    private readonly LevelInputController _controller;

    private readonly ControlPanelButton[] _allButtons;
    private readonly SkillAssignButton[] _skillAssignButtons;

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

    public LevelTimer LevelTimer { get; }
    public SkillAssignButton? SelectedSkillAssignButton { get; private set; }
    public int SelectedSkillButtonId => SelectedSkillAssignButton?.SkillAssignButtonId ?? -1;

    public ReadOnlySpan<SkillAssignButton> SkillAssignButtons => new(_skillAssignButtons);
    public ReadOnlySpan<ControlPanelButton> AllButtons => new(_allButtons);
    public bool HasReleaseRateButtons { get; }
    private int ReleaseRateButtonOffset => HasReleaseRateButtons ? 2 : 0;

    public LevelControlPanel(
        LevelData levelData,
        SkillSetManager skillSetManager,
        LevelInputController controller,
        LevelTimer levelTimer)
    {
        _controller = controller;
        LevelTimer = levelTimer;

        HasReleaseRateButtons = levelData.AllHatchGroupData.Count == 1;

        _skillAssignButtons = CreateSkillAssignButtons(skillSetManager);
        _allButtons = new ControlPanelButton[NumberOfTechnicalButtons + _skillAssignButtons.Length];

        SetUpButtons();

        _maxSkillPanelScroll = _skillAssignButtons.Length - MaxNumberOfSkillButtons;

        SetSelectedSkillAssignmentButton(_skillAssignButtons.FirstOrDefault());
    }

    private SkillAssignButton[] CreateSkillAssignButtons(SkillSetManager skillSetManager)
    {
        var allSkillTrackingData = skillSetManager.AllSkillTrackingData;
        var result = new SkillAssignButton[allSkillTrackingData.Length];

        var i = 0;
        var hatchGroupOffset = ReleaseRateButtonOffset;
        foreach (var skillTrackingData in allSkillTrackingData)
        {
            result[i] = new SkillAssignButton(i, (i + hatchGroupOffset) & 7, skillTrackingData);
            i++;
        }

        return result;
    }

    private void SetUpButtons()
    {
        var i = 0;
        for (; i < _skillAssignButtons.Length; i++)
        {
            _allButtons[i] = _skillAssignButtons[i];
        }
    }

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
        HorizontalButtonScreenSpace = (MaxNumberOfSkillButtons + NumberOfTechnicalButtons) * ControlPanelButtonPixelWidth * _controlPanelScale;

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
        /*
        UpdateButtonDimensions(_releaseRateMinusButton);
        x0 += ControlPanelButtonScreenWidth;
        UpdateButtonDimensions(_releaseRatePlusButton);

       */
        UpdateSkillAssignButtonDimensions();/*

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
        */
        return;

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

        var x0 = ControlPanelX + ControlPanelButtonScreenWidth * (ReleaseRateButtonOffset - SkillPanelScroll);

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

        if (!_controller.LeftMouseButtonAction.IsPressed)
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