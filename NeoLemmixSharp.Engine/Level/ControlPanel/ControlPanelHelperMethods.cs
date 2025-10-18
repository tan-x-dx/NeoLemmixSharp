using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Level.Objectives;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.Rendering.Ui;

namespace NeoLemmixSharp.Engine.Level.ControlPanel;

public static class ControlPanelHelperMethods
{
    private static int _buttonId;

    public static void ResetButtonIds()
    {
        _buttonId = 0;
    }

    public static SkillAssignButton[] SetUpSkillAssignButtons(
        ControlPanelParameterSet controlPanelParameters,
        SkillSetManager skillSetManager)
    {
        return controlPanelParameters.Contains(ControlPanelParameters.EnableClassicModeSkillsIfPossible) &&
               skillSetManager.HasClassicSkillsOnly()
            ? CreateClassicModeSkillAssignButtons()
            : CreateSkillAssignButtons();

        SkillAssignButton[] CreateClassicModeSkillAssignButtons()
        {
            var i = 0;

            var allClassicSkills = LemmingSkill.AllClassicSkills;
            var result = new SkillAssignButton[allClassicSkills.Count];

            foreach (var classicSkill in allClassicSkills)
            {
                var skillTrackingData = skillSetManager.GetSkillTrackingData(classicSkill.Id, EngineConstants.ClassicTribeId);

                int skillTrackingDataId;
                if (skillTrackingData is null)
                {
                    skillTrackingDataId = -1;
                }
                else
                {
                    skillTrackingDataId = skillTrackingData.SkillTrackingDataId;
                }

                var skillAssignButton = new SkillAssignButton(
                    _buttonId++,
                    i,
                    i,
                    classicSkill.Id,
                    skillTrackingDataId);

                skillAssignButton.UpdateSkillCount();
                result[i++] = skillAssignButton;
            }

            return result;
        }

        SkillAssignButton[] CreateSkillAssignButtons()
        {
            var i = 0;

            var allSkillTrackingData = skillSetManager.AllItems;
            var result = new SkillAssignButton[allSkillTrackingData.Length];

            foreach (var skillTrackingData in allSkillTrackingData)
            {
                var skillAssignButton = new SkillAssignButton(
                    _buttonId++,
                    i,
                    i,
                    skillTrackingData.Skill.Id,
                    skillTrackingData.SkillTrackingDataId);
                result[i++] = skillAssignButton;
                skillAssignButton.UpdateSkillCount();
            }

            return result;
        }
    }

    public static ControlPanelButton[] SetUpControlButtons(
        LevelInputController controller,
        SkillAssignButton[] skillAssignButtons,
        HatchGroup? hatchGroup,
        ControlPanelParameterSet controlPanelParameters)
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
            result[0] = SpawnIntervalButton.CreateSpawnIntervalIncreaseButton(_buttonId++, 2, controlPanelParameters, hatchGroup!);
            result[1] = SpawnIntervalButton.CreateSpawnIntervalDisplayButton(_buttonId++, 1, controlPanelParameters, hatchGroup!);
            result[2] = SpawnIntervalButton.CreateSpawnIntervalDecreaseButton(_buttonId++, 0, controlPanelParameters, hatchGroup!);
        }

        var newButtonIndex = releaseRateButtonOffset;

        if (showSkillAssignScrollButtons) // Left scroll button
        {
            var buttonAction = new SkillAssignScrollButtonAction(1);
            result[newButtonIndex] = new ControlPanelButton(
                _buttonId++,
                newButtonIndex & ControlPanelButton.SkillPanelFrameMask,
                buttonAction,
                PanelHelpers.SkillAssignScrollLeftX,
                PanelHelpers.ButtonIconsY);
            newButtonIndex++;
        }

        Array.Copy(skillAssignButtons, 0, result, newButtonIndex, numberOfSkillAssignButtons);
        newButtonIndex += numberOfSkillAssignButtons;

        if (showSkillAssignScrollButtons) // Right scroll button
        {
            var buttonAction = new SkillAssignScrollButtonAction(-1);
            result[newButtonIndex] = new ControlPanelButton(
                _buttonId++,
                newButtonIndex & ControlPanelButton.SkillPanelFrameMask,
                buttonAction,
                PanelHelpers.SkillAssignScrollRightX,
                PanelHelpers.ButtonIconsY);
            newButtonIndex++;
        }
        else // Padding buttons
        {
            for (var p = 0; p < paddingButtonCount; p++)
            {
                result[newButtonIndex] = new PaddingButton(_buttonId++);
                newButtonIndex++;
            }
        }

        if (showPause)
        {
            var pauseAction = controller.Pause;
            var buttonAction = new PauseButtonAction(pauseAction);
            result[newButtonIndex] = new ControlPanelButton(
                _buttonId++,
                newButtonIndex & ControlPanelButton.SkillPanelFrameMask,
                buttonAction,
                PanelHelpers.PauseButtonX,
                PanelHelpers.ButtonIconsY);
            newButtonIndex++;
        }

        if (showNuke)
        {
            var buttonAction = new NukeButtonAction();
            result[newButtonIndex] = new ControlPanelButton(
                _buttonId++,
                newButtonIndex & ControlPanelButton.SkillPanelFrameMask,
                buttonAction,
                PanelHelpers.NukeButtonX,
                PanelHelpers.ButtonIconsY);
            newButtonIndex++;
        }

        if (showFastForward)
        {
            var fastForwardAction = controller.ToggleFastForwards;
            var buttonAction = new FastForwardButtonAction(fastForwardAction);
            result[newButtonIndex] = new ControlPanelButton(
                _buttonId++,
                newButtonIndex & ControlPanelButton.SkillPanelFrameMask,
                buttonAction,
                PanelHelpers.FastForwardButtonX,
                PanelHelpers.ButtonIconsY);
            newButtonIndex++;
        }

        if (showRestart)
        {
            var buttonAction = new RestartButtonAction();
            result[newButtonIndex] = new ControlPanelButton(
                _buttonId++,
                newButtonIndex & ControlPanelButton.SkillPanelFrameMask,
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
            testFlag = controlPanelParameters.Contains(test);

            return testFlag
                ? numberOfButtons
                : 0;
        }

        int GetPaddingButtonCount()
        {
            return showSkillAssignScrollButtons ||
                   controlPanelParameters.Contains(ControlPanelParameters.RemoveSkillAssignPaddingButtons)
                ? 0
                : LevelControlPanel.MaxNumberOfSkillButtons - numberOfSkillAssignButtons;
        }

        bool IncludeReleaseRateButtons()
        {
            return hatchGroup is not null && controlPanelParameters.Contains(ControlPanelParameters.ShowSpawnIntervalButtonsIfPossible);
        }
    }
}