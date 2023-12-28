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
		var allButtons = AllButtons;

		var index = 0;
		for (; index < allButtons.Length; index++)
		{
			var controlPanelButton = allButtons[index];
			if (controlPanelButton is SkillAssignButton)
				break;

			x0 += ControlPanelButtonScreenWidth;
			UpdateButtonDimensions(controlPanelButton);
		}

		UpdateSkillAssignButtonDimensions();

		var dx = Math.Min(_skillAssignButtons.Length, MaxNumberOfSkillButtons) * ControlPanelButtonScreenWidth;
		x0 += dx;
		index += _skillAssignButtons.Length;

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
			button.ScreenY = y0;
			button.ScreenWidth = ControlPanelButtonScreenWidth;
			button.ScreenHeight = h0;
		}
	}

	private void UpdateSkillAssignButtonDimensions()
	{
		var indexOfLastSkillAssignButtonToRender = SkillPanelScroll + MaxNumberOfSkillButtons;
		var skillAssignOffset = GetFirstIndexOfSkillAssignButton();

		if (skillAssignOffset < 0)
			return;

		var x0 = ControlPanelX + ControlPanelButtonScreenWidth * (1 + skillAssignOffset - SkillPanelScroll);

		for (var i = 0; i < _skillAssignButtons.Length; i++)
		{
			var button = _skillAssignButtons[i];
			button.ScreenX = x0;
			button.ScreenY = ControlPanelButtonY;
			button.ScreenWidth = ControlPanelButtonScreenWidth;
			button.ScreenHeight = ControlPanelButtonScreenHeight;
			_skillAssignButtons[i].ShouldRender = i >= SkillPanelScroll && i < indexOfLastSkillAssignButtonToRender;
			x0 += ControlPanelButtonScreenWidth;
		}

		return;

		int GetFirstIndexOfSkillAssignButton()
		{
			for (var index = 0; index < AllButtons.Length; index++)
			{
				var button = AllButtons[index];
				if (button is SkillAssignButton)
					return index;
			}

			return -1;
		}
	}

	public void HandleMouseInput()
	{
		if (_skillAssignButtons.Length > MaxNumberOfSkillButtons)
		{
			TrackScrollWheel();
		}

		var leftMouseButton = _controller.LeftMouseButtonAction;
		var rightMouseButton = _controller.RightMouseButtonAction;

		var mouseX = _controller.MouseX;
		var mouseY = _controller.MouseY;
		foreach (var controlPanelButton in _allButtons)
		{
			if (controlPanelButton is null)
				continue;

			if (!controlPanelButton.TryPress(mouseX, mouseY))
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
			if (secondButton is SpawnIntervalButton result)
				return result;

			throw new InvalidOperationException($"Could not locate {nameof(SpawnIntervalButton)}");
		}
	}
}