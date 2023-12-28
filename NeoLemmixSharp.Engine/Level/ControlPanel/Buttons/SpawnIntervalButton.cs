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

		var buttonAction = new SpawnIntervalChangeButtonAction(spawnIntervalMinValueGetter, hatchGroup);

		var iconX = spawnIntervalMinValueGetter.GetIconX();
		var iconY = spawnIntervalMinValueGetter.GetIconY();

		return new SpawnIntervalButton(
			skillPanelFrame,
			spawnIntervalMinValueGetter,
			buttonAction,
			iconX,
			iconY);
	}

	public static SpawnIntervalButton CreateSpawnIntervalDisplayButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
	{
		var spawnIntervalMinValueGetter = new SpawnIntervalCurrentValueGetter(
			hatchGroup,
			controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval));

		return new SpawnIntervalButton(
			skillPanelFrame,
			spawnIntervalMinValueGetter,
			EmptyButtonAction.Instance,
			-1,
			-1);
	}

	public static SpawnIntervalButton CreateSpawnIntervalIncreaseButton(
		int skillPanelFrame,
		ControlPanelParameters controlPanelParameters,
		HatchGroup hatchGroup)
	{
		var spawnIntervalMaxValueGetter = new SpawnIntervalMaxValueGetter(
			hatchGroup,
			controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval));

		var buttonAction = new SpawnIntervalChangeButtonAction(spawnIntervalMaxValueGetter, hatchGroup);

		var iconX = spawnIntervalMaxValueGetter.GetIconX();
		var iconY = spawnIntervalMaxValueGetter.GetIconY();

		return new SpawnIntervalButton(
			skillPanelFrame,
			spawnIntervalMaxValueGetter,
			buttonAction,
			iconX,
			iconY);
	}

	private SpawnIntervalButton(
		int skillPanelFrame,
		ISpawnIntervalValueGetter spawnIntervalValueGetter,
		IButtonAction buttonAction,
		int iconX,
		int iconY)
		: base(skillPanelFrame, buttonAction, iconX, iconY)
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

	public override ControlPanelButtonRenderer CreateButtonRenderer(ControlPanelSpriteBank spriteBank)
	{
		var iconX = _spawnIntervalValueGetter.GetIconX();
		var iconY = _spawnIntervalValueGetter.GetIconY();

		return new ControlPanelButtonRenderer(spriteBank, this, iconX, iconY);
	}

	private interface ISpawnIntervalValueGetter
	{
		int GetSpawnIntervalDelta();
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

		public int GetSpawnIntervalDelta() => -1;
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

		public int GetSpawnIntervalDelta() => 0;
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

		public int GetSpawnIntervalDelta() => 1;
		public int GetNumericalValue() => _showSpawnInterval
			? _hatchGroup.MaxSpawnInterval
			: _hatchGroup.MaxReleaseRate;
		public int GetIconX() => _showSpawnInterval
			? PanelHelpers.MinusButtonX
			: PanelHelpers.PlusButtonX;
		public int GetIconY() => PanelHelpers.ButtonIconsY;
	}

	private sealed class SpawnIntervalChangeButtonAction : IButtonAction
	{
		private readonly ISpawnIntervalValueGetter _spawnIntervalValueGetter;
		private readonly HatchGroup _hatchGroup;

		public SpawnIntervalChangeButtonAction(
			ISpawnIntervalValueGetter spawnIntervalValueGetter,
			HatchGroup hatchGroup)
		{
			_spawnIntervalValueGetter = spawnIntervalValueGetter;
			_hatchGroup = hatchGroup;
		}

		public void OnMouseDown()
		{
		}

		public void OnPress()
		{
			var delta = _spawnIntervalValueGetter.GetSpawnIntervalDelta();
			_hatchGroup.ChangeSpawnInterval(delta);

			LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
		}

		public void OnDoubleTap()
		{
		}

		public void OnRightClick()
		{
			var delta = _spawnIntervalValueGetter.GetSpawnIntervalDelta();
			_hatchGroup.ChangeSpawnInterval(delta * 1000); // Set to extremal value - will be clamped appropriately

			LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
		}
	}
}