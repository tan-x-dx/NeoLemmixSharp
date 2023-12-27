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

	public abstract SpawnIntervalButtonIcon GetIcon();

	public enum SpawnIntervalButtonIcon
	{
		Nothing,
		MinusSign,
		PlusSign
	}

	public sealed override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		return new SpawnIntervalButtonRenderer(spriteBank, this);
	}
}

public sealed class SpawnIntervalDecreaseButton : SpawnIntervalButton
{
	public SpawnIntervalDecreaseButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
		: base(skillPanelFrame, controlPanelParameters, hatchGroup)
	{
	}

	public override void OnPress()
	{
		// SI decreases => RR increases
		// Hence delta = -1
		HatchGroup.ChangeSpawnInterval(-1);

		LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
	}

	protected override int GetNumericalValue()
	{
		return ShowSpawnInterval
			? HatchGroup.MaxSpawnInterval
			: HatchGroup.MaxReleaseRate;
	}

	public override SpawnIntervalButtonIcon GetIcon()
	{
		return ShowSpawnInterval
			? SpawnIntervalButtonIcon.PlusSign
			: SpawnIntervalButtonIcon.MinusSign;
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

	protected override int GetNumericalValue()
	{
		return ShowSpawnInterval
			? HatchGroup.CurrentSpawnInterval
			: HatchGroup.CurrentReleaseRate;
	}

	public override SpawnIntervalButtonIcon GetIcon() => SpawnIntervalButtonIcon.Nothing;
}

public sealed class SpawnIntervalIncreaseButton : SpawnIntervalButton
{
	public SpawnIntervalIncreaseButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
		: base(skillPanelFrame, controlPanelParameters, hatchGroup)
	{
	}

	public override void OnPress()
	{
		// SI increases => RR decreases
		// Hence delta = +1
		HatchGroup.ChangeSpawnInterval(1);

		LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
	}

	protected override int GetNumericalValue()
	{
		return ShowSpawnInterval
			? HatchGroup.MinSpawnInterval
			: HatchGroup.MinReleaseRate;
	}

	public override SpawnIntervalButtonIcon GetIcon()
	{
		return ShowSpawnInterval
			? SpawnIntervalButtonIcon.MinusSign
			: SpawnIntervalButtonIcon.PlusSign;
	}
}
