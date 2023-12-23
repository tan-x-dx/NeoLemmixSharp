using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;
using NeoLemmixSharp.Engine.LevelBuilding.Data;
using System.Runtime.InteropServices;

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

	private readonly List<ControlPanelButton> _allButtons = new();
	private readonly List<SkillAssignButton> _skillAssignButtons = new();

	private readonly int _maxSkillPanelScroll;

	private int _controlPanelScale = 4;

	public int ReleaseRateButtonOffset { get; }
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

	public ReadOnlySpan<SkillAssignButton> SkillAssignButtons => CollectionsMarshal.AsSpan(_skillAssignButtons);
	public ReadOnlySpan<ControlPanelButton> AllButtons => CollectionsMarshal.AsSpan(_allButtons);

	public LevelControlPanel(
		LevelData levelData,
		ControlPanelButtonAvailability controlPanelButtonAvailability,
		LevelInputController controller,
		SkillSetManager skillSetManager,
		LevelTimer levelTimer)
	{
		_controller = controller;
		LevelTimer = levelTimer;

		ReleaseRateButtonOffset = GetReleaseRateButtonOffset(levelData, controlPanelButtonAvailability);

		SetUpSkillAssignButtons(controlPanelButtonAvailability, skillSetManager);
		SetUpControlButtons(controlPanelButtonAvailability);

		_maxSkillPanelScroll = _skillAssignButtons.Count - MaxNumberOfSkillButtons;

		SetSelectedSkillAssignmentButton(_skillAssignButtons.FirstOrDefault());
	}

	private static int GetReleaseRateButtonOffset(
		LevelData data,
		ControlPanelButtonAvailability controlPanelButtonAvailability)
	{
		var numberOfHatchGroups = data.AllHatchGroupData.Count;
		if (numberOfHatchGroups > 1)
			return 0;

		return controlPanelButtonAvailability.HasFlag(ControlPanelButtonAvailability.ShowReleaseRateButtonsIfPossible) ? 2 : 0;
	}

	private void SetUpSkillAssignButtons(
		ControlPanelButtonAvailability controlPanelButtonAvailability,
		SkillSetManager skillSetManager)
	{
		var allSkillTrackingData = skillSetManager.AllSkillTrackingData;

		var i = 0;
		var hatchGroupOffset = ReleaseRateButtonOffset;
		if (controlPanelButtonAvailability.HasFlag(ControlPanelButtonAvailability.EnableClassicModeSkillsIfPossible) &&
			skillSetManager.HasClassicSkillsOnly())
		{
			foreach (var classicSkill in LemmingSkill.AllClassicSkills)
			{
				var skillTrackingData = skillSetManager.GetSkillTrackingData(classicSkill.Id, 0);
				var skillTrackingDataId = skillTrackingData?.SkillTrackingDataId ?? -1;

				var skillAssignButton = new SkillAssignButton(i,
					(i + hatchGroupOffset) & 7,
					classicSkill.Id,
					skillTrackingDataId);
				_skillAssignButtons.Add(skillAssignButton);
				i++;
			}
		}
		else
		{
			foreach (var skillTrackingData in allSkillTrackingData)
			{
				var skillAssignButton = new SkillAssignButton(
					i,
					(i + hatchGroupOffset) & 7,
					skillTrackingData.Skill.Id,
					skillTrackingData.SkillTrackingDataId);
				_skillAssignButtons.Add(skillAssignButton);
				i++;
			}
		}
	}

	private void SetUpControlButtons(ControlPanelButtonAvailability controlPanelButtonAvailability)
	{
		var numberOfTechnicalButtons = ReleaseRateButtonOffset +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowPauseButton, 1) +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowNukeButton, 1) +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowFastForwardsButton, 1) +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowRestartButton, 1) +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowFrameNudgeButtons, 2) +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowDirectionSelectButtons, 2) +
									   ButtonAvailability(ControlPanelButtonAvailability.ShowClearPhysicsAndReplayButton, 2) +
									   GetExtraSkillAssignButtonCount();

		_allButtons.Capacity = _skillAssignButtons.Count + numberOfTechnicalButtons;
		_allButtons.AddRange(_skillAssignButtons);

		return;

		int ButtonAvailability(ControlPanelButtonAvailability test, int numberOfButtons)
		{
			return controlPanelButtonAvailability.HasFlag(test)
				? numberOfButtons
				: 0;
		}

		int GetExtraSkillAssignButtonCount()
		{
			var numberOfSkills = _skillAssignButtons.Count;

			if (numberOfSkills > MaxNumberOfSkillButtons)
				return 2;

			return controlPanelButtonAvailability.HasFlag(ControlPanelButtonAvailability.RemoveExcessSkillAssignButtons)
				? 0
				: MaxNumberOfSkillButtons - numberOfSkills;
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

		for (var i = 0; i < _skillAssignButtons.Count; i++)
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
		if (_skillAssignButtons.Count > MaxNumberOfSkillButtons)
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