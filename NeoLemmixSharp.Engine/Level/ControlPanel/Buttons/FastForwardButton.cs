﻿using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class FastForwardButton : ControlPanelButton
{
	public FastForwardButton(int skillPanelFrame) : base(skillPanelFrame)
	{
	}

	public override void OnPress()
	{
		LevelScreen.UpdateScheduler.FastForwardButtonPress();
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ControlPanelButtonRenderer(spriteBank, this, PanelHelpers.FastForwardButtonX, PanelHelpers.ButtonIconsY);
	}
}