using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering.Ui;

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
		var showSpawnInterval = controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval);

		var spawnIntervalMinValueGetter = new SpawnIntervalMinValueGetter(
			hatchGroup,
			showSpawnInterval);

		var buttonAction = new SpawnIntervalChangeButtonAction(spawnIntervalMinValueGetter, hatchGroup);

		var iconX = showSpawnInterval
			? PanelHelpers.PlusButtonX
			: PanelHelpers.MinusButtonX;

		return new SpawnIntervalButton(
			skillPanelFrame,
			spawnIntervalMinValueGetter,
			buttonAction,
			iconX,
			PanelHelpers.ButtonIconsY);
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
		var showSpawnInterval = controlPanelParameters.TestFlag(ControlPanelParameters.ShowSpawnInterval);

		var spawnIntervalMaxValueGetter = new SpawnIntervalMaxValueGetter(
			hatchGroup,
			showSpawnInterval);

		var buttonAction = new SpawnIntervalChangeButtonAction(spawnIntervalMaxValueGetter, hatchGroup);

		var iconX = showSpawnInterval
			? PanelHelpers.MinusButtonX
			: PanelHelpers.PlusButtonX;

		return new SpawnIntervalButton(
			skillPanelFrame,
			spawnIntervalMaxValueGetter,
			buttonAction,
			iconX,
			PanelHelpers.ButtonIconsY);
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

	private interface ISpawnIntervalValueGetter
	{
		int GetSpawnIntervalDelta();
		int GetNumericalValue();
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