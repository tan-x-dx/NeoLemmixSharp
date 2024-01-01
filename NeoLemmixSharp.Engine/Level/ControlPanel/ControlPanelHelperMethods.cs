﻿using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Rendering.Ui;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public static class ControlPanelHelperMethods
{
	public static SkillAssignButton[] SetUpSkillAssignButtons(
		ILevelControlPanel controlPanel,
		ControlPanelParameters controlPanelParameters,
		SkillSetManager skillSetManager)
	{
		return controlPanelParameters.TestFlag(ControlPanelParameters.EnableClassicModeSkillsIfPossible) &&
			   skillSetManager.HasClassicSkillsOnly()
			? CreateClassicModeSkillAssignButtons()
			: CreateSkillAssignButtons();

		SkillAssignButton[] CreateClassicModeSkillAssignButtons()
		{
			var i = 0;

			var result = new SkillAssignButton[LevelConstants.NumberOfClassicSkills];

			foreach (var classicSkill in LemmingSkill.AllClassicSkills)
			{
				var skillTrackingData = skillSetManager.GetSkillTrackingData(classicSkill.Id, 0);

				int skillTrackingDataId;
				int skillCount;
				if (skillTrackingData is null)
				{
					skillTrackingDataId = -1;
					skillCount = 0;
				}
				else
				{
					skillTrackingDataId = skillTrackingData.SkillTrackingDataId;
					skillCount = skillTrackingData.SkillCount;
				}

				var skillAssignButton = new SkillAssignButton(
					i,
					i,
					classicSkill.Id,
					skillTrackingDataId);

				controlPanel.UpdateSkillCount(skillAssignButton, skillCount);
				result[i++] = skillAssignButton;
			}

			return result;
		}

		SkillAssignButton[] CreateSkillAssignButtons()
		{
			var i = 0;

			var allSkillTrackingData = skillSetManager.AllSkillTrackingData;
			var result = new SkillAssignButton[allSkillTrackingData.Length];

			foreach (var skillTrackingData in allSkillTrackingData)
			{
				var skillAssignButton = new SkillAssignButton(
					i,
					i,
					skillTrackingData.Skill.Id,
					skillTrackingData.SkillTrackingDataId);
				result[i++] = skillAssignButton;
				controlPanel.UpdateSkillCount(skillAssignButton, skillTrackingData.SkillCount);
			}

			return result;
		}
	}

	public static ControlPanelButton[] SetUpControlButtons(
		SkillAssignButton[] skillAssignButtons,
		HatchGroup? hatchGroup,
		ControlPanelParameters controlPanelParameters)
	{
		var numberOfSkillAssignButtons = skillAssignButtons.Length;
		var includeReleaseRateButtons = IncludeReleaseRateButtons();
		var releaseRateButtonOffset = includeReleaseRateButtons
			? LevelControlPanel.NumberOfReleaseRateButtons
			: 0;
		var showSkillAssignScrollButtons = numberOfSkillAssignButtons > LevelControlPanel.MaxNumberOfSkillButtons;
		var paddingButtonCount = GetPaddingButtonCount();

		var numberOfTechnicalButtons = releaseRateButtonOffset +
									   ButtonAvailability(ControlPanelParameters.ShowPauseButton, 1, out var showPause) +
									   ButtonAvailability(ControlPanelParameters.ShowNukeButton, 1, out var showNuke) +
									   ButtonAvailability(ControlPanelParameters.ShowFastForwardsButton, 1, out var showFastForward) +
									   ButtonAvailability(ControlPanelParameters.ShowRestartButton, 1, out var showRestart) +
									   ButtonAvailability(ControlPanelParameters.ShowFrameNudgeButtons, 2, out var showFrameNudge) +
									   ButtonAvailability(ControlPanelParameters.ShowDirectionSelectButtons, 2, out var showDirectionSelect) +
									   ButtonAvailability(ControlPanelParameters.ShowClearPhysicsAndReplayButton, 2, out var showClearPhysicsAndReplay) +
									   paddingButtonCount +
									   (showSkillAssignScrollButtons ? 2 : 0);

		var totalNumberOfButtons = numberOfSkillAssignButtons + numberOfTechnicalButtons;

		var result = new ControlPanelButton[totalNumberOfButtons];

		if (includeReleaseRateButtons)
		{
			// Always put these buttons at the start if they exist
			result[0] = SpawnIntervalButton.CreateSpawnIntervalIncreaseButton(2, controlPanelParameters, hatchGroup!);
			result[1] = SpawnIntervalButton.CreateSpawnIntervalDisplayButton(1, controlPanelParameters, hatchGroup!);
			result[2] = SpawnIntervalButton.CreateSpawnIntervalDecreaseButton(0, controlPanelParameters, hatchGroup!);
		}

		var newButtonIndex = releaseRateButtonOffset;

		if (showSkillAssignScrollButtons) // Left scroll button
		{
			var buttonAction = new SkillAssignScrollButtonAction(1);
			result[newButtonIndex] = new ControlPanelButton(
				newButtonIndex,
				buttonAction,
				PanelHelpers.SkillAssignScrollLeftX,
				PanelHelpers.SkillAssignScrollY);
			newButtonIndex++;
		}

		Array.Copy(skillAssignButtons, 0, result, newButtonIndex, numberOfSkillAssignButtons);
		newButtonIndex += numberOfSkillAssignButtons;

		if (showSkillAssignScrollButtons) // Right scroll button
		{
			var buttonAction = new SkillAssignScrollButtonAction(-1);
			result[newButtonIndex] = new ControlPanelButton(
				newButtonIndex,
				buttonAction,
				PanelHelpers.SkillAssignScrollRightX,
				PanelHelpers.SkillAssignScrollY);
			newButtonIndex++;
		}
		else // Padding buttons
		{
			for (var p = 0; p < paddingButtonCount; p++)
			{
				result[newButtonIndex] = new PaddingButton();
				newButtonIndex++;
			}
		}

		if (showPause)
		{
			var buttonAction = new PauseButtonAction();
			result[newButtonIndex] = new ControlPanelButton(
				newButtonIndex,
				buttonAction,
				PanelHelpers.PauseButtonX,
				PanelHelpers.ButtonIconsY);
			newButtonIndex++;
		}

		if (showNuke)
		{
			var buttonAction = new NukeButtonAction();
			result[newButtonIndex] = new ControlPanelButton(
				newButtonIndex,
				buttonAction,
				PanelHelpers.NukeButtonX,
				PanelHelpers.ButtonIconsY);
			newButtonIndex++;
		}

		if (showFastForward)
		{
			var buttonAction = new FastForwardButtonAction();
			result[newButtonIndex] = new ControlPanelButton(
				newButtonIndex,
				buttonAction,
				PanelHelpers.FastForwardButtonX,
				PanelHelpers.ButtonIconsY);
			newButtonIndex++;
		}

		if (showRestart)
		{
			var buttonAction = new RestartButtonAction();
			result[newButtonIndex] = new ControlPanelButton(
				newButtonIndex,
				buttonAction,
				PanelHelpers.RestartButtonX,
				PanelHelpers.ButtonIconsY);
			newButtonIndex++;
		}

		if (showFrameNudge)
		{
			var nudgeBack = new FrameNudgeButtonAction(-1);
			var nudgeForward = new FrameNudgeButtonAction(1);

		}

		if (showDirectionSelect)
		{

		}

		if (showClearPhysicsAndReplay)
		{

		}

		return result;

		int ButtonAvailability(ControlPanelParameters test, int numberOfButtons, out bool testFlag)
		{
			testFlag = controlPanelParameters.TestFlag(test);

			return testFlag
				? numberOfButtons
				: 0;
		}

		int GetPaddingButtonCount()
		{
			return showSkillAssignScrollButtons ||
				   controlPanelParameters.TestFlag(ControlPanelParameters.RemoveSkillAssignPaddingButtons)
				? 0
				: LevelControlPanel.MaxNumberOfSkillButtons - numberOfSkillAssignButtons;
		}

		bool IncludeReleaseRateButtons()
		{
			return hatchGroup is not null && controlPanelParameters.TestFlag(ControlPanelParameters.ShowReleaseRateButtonsIfPossible);
		}
	}
}