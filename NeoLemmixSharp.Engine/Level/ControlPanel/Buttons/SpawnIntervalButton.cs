using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public abstract class SpawnIntervalButton : ControlPanelButton
{
	private const int NumberOfChars = 3;

	private readonly int[] _chars = new int[NumberOfChars];
	protected readonly HatchGroup HatchGroup;
	public bool ShowSpawnInterval { get; }

	public ReadOnlySpan<int> ReleaseRateChars => new(_chars);

	protected SpawnIntervalButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
		: base(skillPanelFrame)
	{
		HatchGroup = hatchGroup;
		ShowSpawnInterval = controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval);

		UpdateNumericalValue();
	}

	public void UpdateNumericalValue()
	{
		var span = new Span<int>(_chars);
		var numericalValue = GetNumericalValue();
		TextRenderingHelpers.WriteDigits(span, numericalValue);
	}

	protected abstract int GetNumericalValue();
}

public sealed class SpawnIntervalMinusButton : SpawnIntervalButton
{
	public SpawnIntervalMinusButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
		: base(skillPanelFrame, controlPanelParameters, hatchGroup)
	{
	}

	public override void OnPress()
	{
		// Minus sign => RR decreases => SI increases
		// Hence delta = +1
		HatchGroup.ChangeSpawnInterval(1);

		LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ReleaseRateMinusButtonRenderer(spriteBank, this);
	}

	protected override int GetNumericalValue()
	{
		return ShowSpawnInterval
			? HatchGroup.MinSpawnInterval
			: HatchGroup.MinReleaseRate;
	}
}

public sealed class SpawnIntervalDisplayButton : SpawnIntervalButton
{
	public SpawnIntervalDisplayButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
		: base(skillPanelFrame, controlPanelParameters, hatchGroup)
	{
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ReleaseRateDisplayButtonRenderer(spriteBank, this);
	}

	protected override int GetNumericalValue()
	{
		return ShowSpawnInterval
			? HatchGroup.CurrentSpawnInterval
			: HatchGroup.CurrentReleaseRate;
	}
}

public sealed class SpawnIntervalPlusButton : SpawnIntervalButton
{
	public SpawnIntervalPlusButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
		: base(skillPanelFrame, controlPanelParameters, hatchGroup)
	{
	}

	public override void OnPress()
	{
		// Plus sign => RR increases => SI decreases
		// Hence delta = -1
		HatchGroup.ChangeSpawnInterval(-1);

		LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new ReleaseRatePlusButtonRenderer(spriteBank, this);
	}

	protected override int GetNumericalValue()
	{
		return ShowSpawnInterval
			? HatchGroup.MaxSpawnInterval
			: HatchGroup.MaxReleaseRate;
	}
}