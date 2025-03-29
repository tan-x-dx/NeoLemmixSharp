using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Rendering;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class LevelControlPanel : IInitialisable
{
    public const int MaxNumberOfSkillButtons = 10;
    public const int NumberOfTechnicalButtons = 9;
    public const int ControlPanelButtonPixelWidth = 16;
    public const int ControlPanelButtonPixelHeight = 24;
    public const int ControlPanelInfoPixelHeight = 16;
    public const int ControlPanelTotalPixelHeight = ControlPanelButtonPixelHeight + ControlPanelInfoPixelHeight;
    public const int MinControlPanelScaleMultiplier = 4;
    public const int MaxControlPanelScaleMultiplier = 6;
    public const int NumberOfReleaseRateButtons = 3;
    public const int MinimapWidth = 111;

    /// <summary>
    /// Is only not-null if there is precisely ONE hatch group per level.
    /// In that case, this references that singular hatch group.
    /// </summary>
    private readonly HatchGroup? _singularHatchGroup;
    private readonly SkillAssignButton[] _skillAssignButtons;
    private readonly ControlPanelButton[] _allButtons;
    private readonly ControlPanelTextualData _controlPanelTextualData;

    private readonly int _maxSkillPanelScroll;

    private LevelSize _windowSize;

    public LevelPosition ControlPanelPosition { get; private set; }
    public LevelSize ControlPanelSize { get; private set; }
    public int ControlPanelScreenWidth => ControlPanelSize.W * ControlPanelScaleMultiplier;
    public int ControlPanelScreenHeight => ControlPanelSize.H * ControlPanelScaleMultiplier;

    public int SkillPanelScroll { get; private set; }

    public int ControlPanelScaleMultiplier { get; private set; } = 4;

    public ControlPanelTextualData TextualData => _controlPanelTextualData;
    public SkillAssignButton? SelectedSkillAssignButton { get; private set; }
    public int SelectedSkillButtonId => SelectedSkillAssignButton?.SkillAssignButtonId ?? -1;

    public ReadOnlySpan<SkillAssignButton> SkillAssignButtons => new(_skillAssignButtons);
    public ReadOnlySpan<ControlPanelButton> AllButtons => new(_allButtons);

    public LevelControlPanel(
        ControlPanelParameterSet controlPanelParameters,
        LevelInputController controller,
        LemmingManager lemmingManager,
        SkillSetManager skillSetManager)
    {
        var allHatchGroups = lemmingManager.AllHatchGroups;
        _singularHatchGroup = allHatchGroups.Length == 1
            ? allHatchGroups[0]
            : null;

        ControlPanelHelperMethods.ResetButtonIds();
        _skillAssignButtons = ControlPanelHelperMethods.SetUpSkillAssignButtons(
            this,
            controlPanelParameters,
            skillSetManager);
        _allButtons = ControlPanelHelperMethods.SetUpControlButtons(
            controller,
            _skillAssignButtons,
            _singularHatchGroup,
            controlPanelParameters);

        _controlPanelTextualData = new ControlPanelTextualData(controlPanelParameters);

        _maxSkillPanelScroll = _skillAssignButtons.Length - MaxNumberOfSkillButtons;
    }

    public void Initialise()
    {
        var firstSkillAssignButton = _skillAssignButtons.Length > 0
            ? _skillAssignButtons[0]
            : null;
        SetSelectedSkillAssignmentButton(firstSkillAssignButton);
    }

    public void SetWindowDimensions(int screenWidth, int screenHeight)
    {
        var previousWindowSize = _windowSize;
        _windowSize = new LevelSize(screenWidth, screenHeight);

        if (_windowSize == previousWindowSize)
            return;

        RecalculateButtonDimensions();

        ControlPanelSize = new LevelSize(CalculateControlPanelWidth(), ControlPanelTotalPixelHeight);

        ControlPanelPosition = new LevelPosition(
            (_windowSize.W - ControlPanelScreenWidth) / 2,
            _windowSize.H - ControlPanelScreenHeight);
    }

    public void SetPanelScale(int scale)
    {
        ControlPanelScaleMultiplier = Math.Clamp(scale, MinControlPanelScaleMultiplier, MaxControlPanelScaleMultiplier);
    }

    private void RecalculateButtonDimensions()
    {
        const int halfH0 = ControlPanelButtonPixelHeight / 2;

        var x0 = 0;
        var index = 0;

        var allButtons = AllButtons;
        var skillAssignButtons = SkillAssignButtons;

        if (skillAssignButtons.Length > 0)
        {
            for (; index < allButtons.Length; index++)
            {
                var controlPanelButton = allButtons[index];
                // Quit when we come across the first skill assign button
                if (controlPanelButton.ButtonAction.ButtonType == ButtonType.SkillAssign)
                    break;

                x0 += ControlPanelButtonPixelWidth;
                UpdateButtonDimensions(controlPanelButton);
            }

            // Deal with skill assign buttons separately
            UpdateSkillAssignButtonDimensions();
        }

        var numberOfSkillAssignButtons = skillAssignButtons.Length;

        // Jump ahead in the list to the first non skill assign button
        var dx = Math.Min(numberOfSkillAssignButtons, MaxNumberOfSkillButtons) * ControlPanelButtonPixelWidth;
        x0 += dx;
        index += numberOfSkillAssignButtons;

        for (; index < allButtons.Length; index++)
        {
            var controlPanelButton = allButtons[index];
            if (controlPanelButton is null)
                continue;

            x0 += ControlPanelButtonPixelWidth;
            UpdateButtonDimensions(controlPanelButton);
        }

        return;

        void UpdateButtonDimensions(ControlPanelButton button)
        {
            button.X = x0;
            button.Width = ControlPanelButtonPixelWidth;

            switch (button.ButtonAction.ButtonType.GetButtonTypeSizePosition())
            {
                case ButtonTypeSizePosition.Normal:
                    button.Y = ControlPanelInfoPixelHeight;
                    button.Height = ControlPanelButtonPixelHeight;
                    return;

                case ButtonTypeSizePosition.TopHalf:
                    button.Y = ControlPanelInfoPixelHeight;
                    button.Height = halfH0;

                    x0 -= ControlPanelButtonPixelWidth;
                    return;

                case ButtonTypeSizePosition.BottomHalf:
                    button.Y = ControlPanelInfoPixelHeight + halfH0;
                    button.Height = halfH0;
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private int CalculateControlPanelWidth()
    {
        const int textTotalLength = ControlPanelTextualData.TotalControlPanelTextLength *
                                    PanelFont.GlyphWidth;
        const int iconsTotalLength = ControlPanelRenderer.TotalNumberOfIcons *
                                     ControlPanelRenderer.PanelIconWidth;

        const int infoLength = textTotalLength + iconsTotalLength;

        var buttonsWidth = GetTotalWidthOfAllButtons();

        var max = Math.Max(infoLength, buttonsWidth);

        return max + MinimapWidth;

        int GetTotalWidthOfAllButtons()
        {
            var numberOfButtonsOnScreenAtAnyGivenTime = 0;

            foreach (var button in AllButtons)
            {
                if (button is null)
                    continue;

                if (button.ButtonAction.ButtonType == ButtonType.SkillAssign)
                    continue;

                if (!button.ShouldRender)
                    continue;

                numberOfButtonsOnScreenAtAnyGivenTime++;
            }

            numberOfButtonsOnScreenAtAnyGivenTime += Math.Min(MaxNumberOfSkillButtons, _skillAssignButtons.Length);

            return numberOfButtonsOnScreenAtAnyGivenTime * ControlPanelButtonPixelWidth;
        }
    }

    private void UpdateSkillAssignButtonDimensions()
    {
        var indexOfLastSkillAssignButtonToRender = SkillPanelScroll + MaxNumberOfSkillButtons;
        var skillAssignOffset = GetFirstIndexOfSkillAssignButton();

        if (skillAssignOffset < 0)
            return;

        var x0 = ControlPanelButtonPixelWidth * (1 + skillAssignOffset - SkillPanelScroll);

        var skillAssignButtons = SkillAssignButtons;

        for (var i = 0; i < skillAssignButtons.Length; i++)
        {
            var button = skillAssignButtons[i];
            button.X = x0;
            button.Y = ControlPanelInfoPixelHeight;
            button.Width = ControlPanelButtonPixelWidth;
            button.Height = ControlPanelButtonPixelHeight;
            skillAssignButtons[i].ShouldRender = i >= SkillPanelScroll && i < indexOfLastSkillAssignButtonToRender;
            x0 += ControlPanelButtonPixelWidth;
        }

        return;

        int GetFirstIndexOfSkillAssignButton()
        {
            var allButtons = AllButtons;

            for (var index = 0; index < allButtons.Length; index++)
            {
                var button = allButtons[index];
                if (button.ButtonAction.ButtonType == ButtonType.SkillAssign)
                    return index;
            }

            return -1;
        }
    }

    public void HandleMouseInput()
    {
        if (SkillAssignButtons.Length > MaxNumberOfSkillButtons)
        {
            TrackScrollWheel();
        }

        var leftMouseButton = LevelScreen.LevelInputController.LeftMouseButtonAction;
        var rightMouseButton = LevelScreen.LevelInputController.RightMouseButtonAction;

        var localMousePosition = LevelScreen.LevelInputController.MousePosition - ControlPanelPosition;

        var localMouseX = localMousePosition.X / ControlPanelScaleMultiplier;
        var localMouseY = localMousePosition.Y / ControlPanelScaleMultiplier;

        foreach (var controlPanelButton in AllButtons)
        {
            if (controlPanelButton is null)
                continue;

            if (!controlPanelButton.MouseIsOverButton(localMouseX, localMouseY))
                continue;

            var buttonAction = controlPanelButton.ButtonAction;

            if (leftMouseButton.IsPressed)
            {
                buttonAction.OnPress(leftMouseButton.IsDoubleTap);
                return;
            }

            if (leftMouseButton.IsActionDown)
            {
                buttonAction.OnMouseDown();
            }

            if (rightMouseButton.IsPressed)
            {
                buttonAction.OnRightClick();
            }

            return;
        }
    }

    private void TrackScrollWheel()
    {
        ChangeSkillAssignButtonScroll(LevelScreen.LevelInputController.ScrollDelta);
    }

    public void ChangeSkillAssignButtonScroll(int delta)
    {
        var previousValue = SkillPanelScroll;
        SkillPanelScroll = Math.Clamp(SkillPanelScroll - delta, 0, _maxSkillPanelScroll);
        if (SkillPanelScroll == previousValue)
            return;

        UpdateSkillAssignButtonDimensions();
    }

    public void SetSelectedSkillAssignmentButton(SkillAssignButton? skillAssignButton)
    {
        if (SelectedSkillAssignButton == skillAssignButton)
            return;

        if (SelectedSkillAssignButton is not null)
        {
            SelectedSkillAssignButton.IsSelected = false;
        }

        SelectedSkillAssignButton = skillAssignButton;

        if (SelectedSkillAssignButton is not null)
        {
            SelectedSkillAssignButton.IsSelected = true;
        }

        UpdateCursorColors();
    }

    private void UpdateCursorColors()
    {
        var skillSetManager = LevelScreen.SkillSetManager;
        var skillTrackingDataId = SelectedSkillAssignButton?.SkillTrackingDataId ?? -1;

        var skillTrackingData = skillSetManager.GetSkillTrackingData(skillTrackingDataId);

        LevelScreen.LevelCursor.SetSelectedTeam(skillTrackingData?.Team);
    }

    public void UpdateSkillCount(SkillTrackingData skillTrackingData)
    {
        var selectedSkillAssignButton = GetControlPanelButtonFor(skillTrackingData);

        selectedSkillAssignButton?.UpdateSkillCount(skillTrackingData.SkillCount);
    }

    public void OnSpawnIntervalChanged()
    {
        if (_singularHatchGroup is null)
            return;

        var spawnIntervalDisplayButton = GetSpawnIntervalDisplayButton();

        spawnIntervalDisplayButton?.UpdateNumericalValue();

        return;

        SpawnIntervalButton? GetSpawnIntervalDisplayButton()
        {
            foreach (var button in AllButtons)
            {
                if (button is SpawnIntervalButton result &&
                    result.ButtonAction.ButtonType == ButtonType.SpawnIntervalIncrease)
                    return result;
            }

            return null;
        }
    }

    public ControlPanelButton? GetControlPanelButtonOfType(ButtonType buttonType)
    {
        foreach (var button in AllButtons)
        {
            if (button.ButtonAction.ButtonType == buttonType)
                return button;
        }

        return null;
    }

    private SkillAssignButton? GetControlPanelButtonFor(SkillTrackingData skillTrackingData)
    {
        foreach (var skillAssignButton in SkillAssignButtons)
        {
            if (skillAssignButton.SkillTrackingDataId == skillTrackingData.SkillTrackingDataId)
                return skillAssignButton;
        }

        return null;
    }
}