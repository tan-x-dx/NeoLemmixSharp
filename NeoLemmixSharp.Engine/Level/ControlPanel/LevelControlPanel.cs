using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;
using System.Diagnostics;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public sealed class LevelControlPanel : ILevelControlPanel
{
	public const int MaxNumberOfSkillButtons = 10;
	public const int NumberOfTechnicalButtons = 9;
	public const int ControlPanelButtonPixelWidth = 16;
	public const int ControlPanelButtonPixelHeight = 23;
	public const int ControlPanelInfoPixelHeight = 16;
	public const int ControlPanelTotalPixelHeight = ControlPanelButtonPixelHeight + ControlPanelInfoPixelHeight;
	public const int MinControlPanelScaleMultiplier = 4;
	public const int MaxControlPanelScaleMultiplier = 6;
	public const int NumberOfReleaseRateButtons = 3;

	private readonly LevelInputController _controller;

	/// <summary>
	/// Is only not-null if there is precisely ONE hatch group per level.
	/// In that case, this references that singular hatch group.
	/// </summary>
	private readonly HatchGroup? _singularHatchGroup;
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

	public LevelControlPanel(
		ControlPanelParameters controlPanelParameters,
		LevelInputController controller,
		SkillSetManager skillSetManager,
		LemmingManager lemmingManager,
		LevelTimer levelTimer)
	{
		_controller = controller;
		LevelTimer = levelTimer;

		var allHatchGroups = lemmingManager.AllHatchGroups;
		_singularHatchGroup = allHatchGroups.Length == 1 ? allHatchGroups[0] : null;

		_skillAssignButtons = ControlPanelHelperMethods.SetUpSkillAssignButtons(this, controlPanelParameters, skillSetManager);
		_allButtons = ControlPanelHelperMethods.SetUpControlButtons(_skillAssignButtons, _singularHatchGroup, controlPanelParameters);

		_maxSkillPanelScroll = _skillAssignButtons.Length - MaxNumberOfSkillButtons;

		SetSelectedSkillAssignmentButton(_skillAssignButtons.FirstOrDefault());
	}

	public void SetWindowDimensions(int screenWidth, int screenHeight)
	{
		var previousWidth = ScreenWidth;
		var previousHeight = ScreenHeight;
		ScreenWidth = screenWidth;
		ScreenHeight = screenHeight;

		if (ScreenWidth == previousWidth && ScreenHeight == previousHeight)
			return;

		RecalculateButtonDimensions();
	}

	public void SetPanelScale(int scale)
	{
		var previousScale = _controlPanelScale;

		_controlPanelScale = Math.Clamp(scale, MinControlPanelScaleMultiplier, MaxControlPanelScaleMultiplier);

		if (_controlPanelScale == previousScale)
			return;

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
		var releaseRateButtonOffset = _singularHatchGroup is null
			? 0
			: NumberOfReleaseRateButtons;

		var x0 = ControlPanelX + ControlPanelButtonScreenWidth * (releaseRateButtonOffset - SkillPanelScroll);

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

		var leftMouseButton = _controller.LeftMouseButtonAction;
		if (!leftMouseButton.IsActionDown)
			return;

		var mouseX = _controller.MouseX;
		var mouseY = _controller.MouseY;
		foreach (var controlPanelButton in _allButtons)
		{
			if (!controlPanelButton.TryPress(mouseX, mouseY))
				continue;

			if (leftMouseButton.IsDoubleTap)
			{
				controlPanelButton.OnDoubleTap();
				return;
			}

			if (leftMouseButton.IsPressed)
			{
				controlPanelButton.OnPress();
			}

			return;
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
	}

	public void UpdateSkillCount(SkillAssignButton? selectedSkillAssignButton, int skillCount)
	{
		selectedSkillAssignButton?.UpdateSkillCount(skillCount);
	}

	public void OnSpawnIntervalChanged()
	{
		if (_singularHatchGroup is null)
			return;

		var spawnIntervalDisplayButton = AllButtons[1] as SpawnIntervalDisplayButton;

		Debug.Assert(spawnIntervalDisplayButton is not null);

		spawnIntervalDisplayButton.UpdateNumericalValue();
	}
}