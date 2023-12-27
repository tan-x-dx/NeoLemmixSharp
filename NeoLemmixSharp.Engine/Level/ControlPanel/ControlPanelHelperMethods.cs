using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Skills;

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
			result[0] = new SpawnIntervalIncreaseButton(0, controlPanelParameters, hatchGroup!);
			result[1] = new SpawnIntervalDisplayButton(1, controlPanelParameters, hatchGroup!);
			result[2] = new SpawnIntervalDecreaseButton(2, controlPanelParameters, hatchGroup!);
		}

		var newButtonIndex = releaseRateButtonOffset;

		if (showSkillAssignScrollButtons) // Left scroll button
		{
			result[newButtonIndex] = new SkillAssignScrollButton(newButtonIndex, -1);
			newButtonIndex++;
		}

		Array.Copy(skillAssignButtons, 0, result, newButtonIndex, numberOfSkillAssignButtons);
		newButtonIndex += numberOfSkillAssignButtons;

		if (showSkillAssignScrollButtons) // Right scroll button
		{
			result[newButtonIndex] = new SkillAssignScrollButton(newButtonIndex, 1);
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
			result[newButtonIndex] = new PauseButton(newButtonIndex);
			newButtonIndex++;
		}

		if (showNuke)
		{
			result[newButtonIndex] = new NukeButton(newButtonIndex);
			newButtonIndex++;
		}

		if (showFastForward)
		{
			result[newButtonIndex] = new FastForwardButton(newButtonIndex);
			newButtonIndex++;
		}

		if (showRestart)
		{
			result[newButtonIndex] = new RestartButton(newButtonIndex);
			newButtonIndex++;
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