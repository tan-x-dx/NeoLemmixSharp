using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Ui;
using NeoLemmixSharp.Engine.Rendering.Ui.Buttons;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SpawnIntervalButton : ControlPanelButton
{
	private const int NumberOfChars = 3;

	private readonly int[] _chars = new int[NumberOfChars];
	private readonly ISpawnIntervalValueGetter _spawnIntervalValueGetter;
	private int _numberOfDigitsToRender;

	public static SpawnIntervalButton CreateSpawnIntervalDecreaseButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
	{
		var spawnIntervalMinValueGetter = new SpawnIntervalMinValueGetter(
			hatchGroup,
			controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval));

		return new SpawnIntervalButton(skillPanelFrame, spawnIntervalMinValueGetter);
	}

	public static SpawnIntervalButton CreateSpawnIntervalDisplayButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
	{
		var spawnIntervalMinValueGetter = new SpawnIntervalCurrentValueGetter(
			hatchGroup,
			controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval));

		return new SpawnIntervalButton(skillPanelFrame, spawnIntervalMinValueGetter);
	}

	public static SpawnIntervalButton CreateSpawnIntervalIncreaseButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
	{
		var spawnIntervalMinValueGetter = new SpawnIntervalMaxValueGetter(
			hatchGroup,
			controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval));

		return new SpawnIntervalButton(skillPanelFrame, spawnIntervalMinValueGetter);
	}

	private SpawnIntervalButton(
		int skillPanelFrame,
		ISpawnIntervalValueGetter spawnIntervalValueGetter)
		: base(skillPanelFrame)
	{
		_spawnIntervalValueGetter = spawnIntervalValueGetter;
		UpdateNumericalValue();
	}

	public void UpdateNumericalValue()
	{
		var span = new Span<int>(_chars);
		var numericalValue = _spawnIntervalValueGetter.GetNumericalValue();
		_numberOfDigitsToRender = numericalValue >= 100 ? 3 : 2;
		TextRenderingHelpers.WriteDigits(span, numericalValue);
	}

	public override ReadOnlySpan<int> GetDigitsToRender() => new(_chars);
	public override int GetNumberOfDigitsToRender() => _numberOfDigitsToRender;
	public override void OnPress()
	{
		_spawnIntervalValueGetter.ChangeSpawnInterval();

		LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
	}

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		var iconX = _spawnIntervalValueGetter.GetIconX();
		var iconY = _spawnIntervalValueGetter.GetIconY();

		return new ControlPanelButtonRenderer(spriteBank, this, iconX, iconY);
	}

	private interface ISpawnIntervalValueGetter
	{
		void ChangeSpawnInterval();
		int GetNumericalValue();

		int GetIconX();
		int GetIconY();
	}

	private sealed class SpawnIntervalMinValueGetter : ISpawnIntervalValueGetter
	{
		private readonly HatchGroup _hatchGroup;
		private readonly bool _showSpawnInterval;

		public SpawnIntervalMinValueGetter(HatchGroup hatchGroup, bool showSpawnInterval)
		{
			_hatchGroup = hatchGroup;
			_showSpawnInterval = showSpawnInterval;
		}

		public void ChangeSpawnInterval() => _hatchGroup.ChangeSpawnInterval(-1);
		public int GetNumericalValue() => _showSpawnInterval
			? _hatchGroup.MinSpawnInterval
			: _hatchGroup.MinReleaseRate;
		public int GetIconX() => _showSpawnInterval
			? PanelHelpers.PlusButtonX
			: PanelHelpers.MinusButtonX;
		public int GetIconY() => PanelHelpers.ButtonIconsY;
	}

	private sealed class SpawnIntervalCurrentValueGetter : ISpawnIntervalValueGetter
	{
		private readonly HatchGroup _hatchGroup;
		private readonly bool _showSpawnInterval;

		public SpawnIntervalCurrentValueGetter(HatchGroup hatchGroup, bool showSpawnInterval)
		{
			_hatchGroup = hatchGroup;
			_showSpawnInterval = showSpawnInterval;
		}

		public void ChangeSpawnInterval() { } // Do nothing
		public int GetNumericalValue() => _showSpawnInterval
			? _hatchGroup.CurrentSpawnInterval
			: _hatchGroup.CurrentReleaseRate;
		public int GetIconX() => -1;
		public int GetIconY() => -1;
	}

	private sealed class SpawnIntervalMaxValueGetter : ISpawnIntervalValueGetter
	{
		private readonly HatchGroup _hatchGroup;
		private readonly bool _showSpawnInterval;

		public SpawnIntervalMaxValueGetter(HatchGroup hatchGroup, bool showSpawnInterval)
		{
			_hatchGroup = hatchGroup;
			_showSpawnInterval = showSpawnInterval;
		}

		public void ChangeSpawnInterval() => _hatchGroup.ChangeSpawnInterval(1);
		public int GetNumericalValue() => _showSpawnInterval
			? _hatchGroup.MaxSpawnInterval
			: _hatchGroup.MaxReleaseRate;
		public int GetIconX() => _showSpawnInterval
			? PanelHelpers.MinusButtonX
			: PanelHelpers.PlusButtonX;
		public int GetIconY() => PanelHelpers.ButtonIconsY;
	}
}