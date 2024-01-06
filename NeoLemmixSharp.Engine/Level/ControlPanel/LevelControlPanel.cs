using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Level.Timer;

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
	public int ControlPanelScale { get; private set; } = 4;

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
		_singularHatchGroup = allHatchGroups.Length == 1
			? allHatchGroups[0]
			: null;

		_skillAssignButtons = ControlPanelHelperMethods.SetUpSkillAssignButtons(this, controlPanelParameters, skillSetManager);
		_allButtons = ControlPanelHelperMethods.SetUpControlButtons(_skillAssignButtons, _singularHatchGroup, controlPanelParameters);

		_maxSkillPanelScroll = _skillAssignButtons.Length - MaxNumberOfSkillButtons;

		var firstSkillAssignButton = _skillAssignButtons.Length > 0
			? _skillAssignButtons[0]
			: null;
	//	SetSelectedSkillAssignmentButton(firstSkillAssignButton);
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
		var previousScale = ControlPanelScale;

		ControlPanelScale = Math.Clamp(scale, MinControlPanelScaleMultiplier, MaxControlPanelScaleMultiplier);

		if (ControlPanelScale == previousScale)
			return;

		RecalculateButtonDimensions();
	}

	private void RecalculateButtonDimensions()
	{
		HorizontalButtonScreenSpace = (MaxNumberOfSkillButtons + NumberOfTechnicalButtons) * ControlPanelButtonPixelWidth * ControlPanelScale;

		ControlPanelX = (ScreenWidth - HorizontalButtonScreenSpace) / 2;
		ControlPanelY = ScreenHeight - (ControlPanelTotalPixelHeight * ControlPanelScale);

		ControlPanelButtonScreenWidth = ControlPanelButtonPixelWidth * ControlPanelScale;
		ControlPanelButtonScreenHeight = ControlPanelButtonPixelHeight * ControlPanelScale;
		ControlPanelInfoScreenHeight = ControlPanelInfoPixelHeight * ControlPanelScale;
		ControlPanelScreenHeight = ControlPanelTotalPixelHeight * ControlPanelScale;

		ControlPanelButtonY = ControlPanelY + ControlPanelInfoScreenHeight;

		var x0 = ControlPanelX;
		var y0 = ControlPanelButtonY;
		var h0 = ControlPanelButtonScreenHeight;
		var halfH0 = h0 / 2;
		var allButtons = AllButtons;

		var index = 0;
		for (; index < allButtons.Length; index++)
		{
			var controlPanelButton = allButtons[index];
			// Quit when we come across the first skill assign button
			if (controlPanelButton.ButtonAction.ButtonType == ButtonType.SkillAssign)
				break;

			x0 += ControlPanelButtonScreenWidth;
			UpdateButtonDimensions(controlPanelButton);
		}

		// Deal with skill assign buttons separately
		UpdateSkillAssignButtonDimensions();

		var skillAssignButtons = SkillAssignButtons;
		var numberOfSkillAssignButtons = skillAssignButtons.Length;

		// Jump ahead in the list to the first non skill assign button
		var dx = Math.Min(numberOfSkillAssignButtons, MaxNumberOfSkillButtons) * ControlPanelButtonScreenWidth;
		x0 += dx;
		index += numberOfSkillAssignButtons;

		for (; index < allButtons.Length; index++)
		{
			var controlPanelButton = allButtons[index];
			if (controlPanelButton is null)
				continue;

			x0 += ControlPanelButtonScreenWidth;
			UpdateButtonDimensions(controlPanelButton);
		}

		return;

		void UpdateButtonDimensions(ControlPanelButton button)
		{
			button.ScreenX = x0;
			button.ScreenWidth = ControlPanelButtonScreenWidth;

			switch (button.ButtonAction.ButtonType.GetButtonTypeSizePosition())
			{
				case ButtonTypeSizePosition.Normal:
					button.ScreenY = y0;
					button.ScreenHeight = h0;
					return;

				case ButtonTypeSizePosition.TopHalf:
					button.ScreenY = y0;
					button.ScreenHeight = halfH0;

					x0 -= ControlPanelButtonScreenWidth;
					return;

				case ButtonTypeSizePosition.BottomHalf:
					button.ScreenY = y0 + halfH0;
					button.ScreenHeight = halfH0;
					return;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void UpdateSkillAssignButtonDimensions()
	{
		var indexOfLastSkillAssignButtonToRender = SkillPanelScroll + MaxNumberOfSkillButtons;
		var skillAssignOffset = GetFirstIndexOfSkillAssignButton();

		if (skillAssignOffset < 0)
			return;

		var x0 = ControlPanelX + ControlPanelButtonScreenWidth * (1 + skillAssignOffset - SkillPanelScroll);

		var skillAssignButtons = SkillAssignButtons;

		for (var i = 0; i < skillAssignButtons.Length; i++)
		{
			var button = skillAssignButtons[i];
			button.ScreenX = x0;
			button.ScreenY = ControlPanelButtonY;
			button.ScreenWidth = ControlPanelButtonScreenWidth;
			button.ScreenHeight = ControlPanelButtonScreenHeight;
			skillAssignButtons[i].ShouldRender = i >= SkillPanelScroll && i < indexOfLastSkillAssignButtonToRender;
			x0 += ControlPanelButtonScreenWidth;
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

		var leftMouseButton = _controller.LeftMouseButtonAction;
		var rightMouseButton = _controller.RightMouseButtonAction;

		var mouseX = _controller.MouseX;
		var mouseY = _controller.MouseY;
		foreach (var controlPanelButton in AllButtons)
		{
			if (controlPanelButton is null)
				continue;

			if (!controlPanelButton.MouseIsOverButton(mouseX, mouseY))
				continue;

			var buttonAction = controlPanelButton.ButtonAction;

			if (leftMouseButton.IsDoubleTap)
			{
				buttonAction.OnDoubleTap();
				return;
			}

			if (leftMouseButton.IsPressed)
			{
				buttonAction.OnPress();
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
		ChangeSkillAssignButtonScroll(_controller.ScrollDelta);
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

	public void UpdateSkillCount(SkillAssignButton? selectedSkillAssignButton, int skillCount)
	{
		selectedSkillAssignButton?.UpdateSkillCount(skillCount);
	}

	public void OnSpawnIntervalChanged()
	{
		if (_singularHatchGroup is null)
			return;

		var spawnIntervalDisplayButton = GetSpawnIntervalDisplayButton();

		spawnIntervalDisplayButton.UpdateNumericalValue();

		return;

		SpawnIntervalButton GetSpawnIntervalDisplayButton()
		{
			var buttons = AllButtons;

			var secondButton = buttons[1];
			if (secondButton is SpawnIntervalButton result &&
				result.ButtonAction.ButtonType == ButtonType.SpawnIntervalDisplay)
				return result;

			throw new InvalidOperationException($"Could not locate {nameof(SpawnIntervalButton)}");
		}
	}
}