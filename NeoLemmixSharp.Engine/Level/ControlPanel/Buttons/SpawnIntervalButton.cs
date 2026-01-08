using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HatchGadgets;
using NeoLemmixSharp.Engine.Rendering.Ui;
using System.Runtime.CompilerServices;

namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public sealed class SpawnIntervalButton : ControlPanelButton
{
    private const int NumberOfSpawnIntervalChars = 3;

    private readonly ISpawnIntervalValueGetter _spawnIntervalValueGetter;
    private SpawnIntervalCharBuffer _spawnIntervalCharBuffer;
    private int _numberOfDigitsToRender;

    public static SpawnIntervalButton CreateSpawnIntervalDecreaseButton(
        int buttonId,
        int skillPanelFrame,
        ControlPanelParameterSet controlPanelParameters,
        HatchGroup hatchGroup)
    {
        var showSpawnInterval = controlPanelParameters.Contains(ControlPanelParameters.ShowSpawnIntervalInsteadOfReleaseRate);

        var spawnIntervalMinValueGetter = new SpawnIntervalMinValueGetter(
            hatchGroup,
            showSpawnInterval);

        var iconX = showSpawnInterval
            ? PanelHelpers.PlusButtonX
            : PanelHelpers.MinusButtonX;

        return new SpawnIntervalButton(
            buttonId,
            skillPanelFrame,
            spawnIntervalMinValueGetter,
            iconX,
            PanelHelpers.ButtonIconsY);
    }

    public static SpawnIntervalButton CreateSpawnIntervalDisplayButton(
        int buttonId,
        int skillPanelFrame,
        ControlPanelParameterSet controlPanelParameters,
        HatchGroup hatchGroup)
    {
        var spawnIntervalCurrentValueGetter = new SpawnIntervalCurrentValueGetter(
            hatchGroup,
            controlPanelParameters.Contains(ControlPanelParameters.ShowSpawnIntervalInsteadOfReleaseRate));

        return new SpawnIntervalButton(
            buttonId,
            skillPanelFrame,
            spawnIntervalCurrentValueGetter,
            -1,
            -1);
    }

    public static SpawnIntervalButton CreateSpawnIntervalIncreaseButton(
        int buttonId,
        int skillPanelFrame,
        ControlPanelParameterSet controlPanelParameters,
        HatchGroup hatchGroup)
    {
        var showSpawnInterval = controlPanelParameters.Contains(ControlPanelParameters.ShowSpawnIntervalInsteadOfReleaseRate);

        var spawnIntervalMaxValueGetter = new SpawnIntervalMaxValueGetter(
            hatchGroup,
            showSpawnInterval);

        var iconX = showSpawnInterval
            ? PanelHelpers.MinusButtonX
            : PanelHelpers.PlusButtonX;

        return new SpawnIntervalButton(
            buttonId,
            skillPanelFrame,
            spawnIntervalMaxValueGetter,
            iconX,
            PanelHelpers.ButtonIconsY);
    }

    private SpawnIntervalButton(
        int buttonId,
        int skillPanelFrame,
        ISpawnIntervalValueGetter spawnIntervalValueGetter,
        int iconX,
        int iconY)
        : base(buttonId, skillPanelFrame, spawnIntervalValueGetter, iconX, iconY)
    {
        _spawnIntervalValueGetter = spawnIntervalValueGetter;
        UpdateNumericalValue();
    }

    public void UpdateNumericalValue()
    {
        var numericalValue = _spawnIntervalValueGetter.GetNumericalValue();
        _numberOfDigitsToRender = NumberFormattingHelpers.GetNumberStringLengthSmall(numericalValue);
        NumberFormattingHelpers.WriteDigits(_spawnIntervalCharBuffer, numericalValue);
    }

    public override ReadOnlySpan<char> GetDigitsToRender() => _spawnIntervalCharBuffer;
    public override int GetNumberOfDigitsToRender() => _numberOfDigitsToRender;

    private interface ISpawnIntervalValueGetter : IButtonAction
    {
        uint GetNumericalValue();
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

        public ButtonType ButtonType => ButtonType.SpawnIntervalDecrease;
        public uint GetNumericalValue() => _showSpawnInterval
            ? _hatchGroup.MinSpawnInterval
            : _hatchGroup.MinReleaseRate;

        public void OnMouseDown()
        {
            if (_hatchGroup.TryAddSpawnIntervalDelta(-1))
            {
                LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
            }
        }

        public void OnPress(bool isDoubleTap)
        {
        }

        public void OnRightClick()
        {
            if (_hatchGroup.TryAddSpawnIntervalDelta(-1000)) // Add extreme value - will be clamped appropriately
            {
                LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
            }
        }
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

        public ButtonType ButtonType => ButtonType.SpawnIntervalDisplay;
        public uint GetNumericalValue() => _showSpawnInterval
            ? _hatchGroup.CurrentSpawnInterval
            : _hatchGroup.CurrentReleaseRate;

        public void OnMouseDown()
        {
        }

        public void OnPress(bool isDoubleTap)
        {
        }

        public void OnRightClick()
        {
        }
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

        public ButtonType ButtonType => ButtonType.SpawnIntervalIncrease;
        public uint GetNumericalValue() => _showSpawnInterval
            ? _hatchGroup.CurrentSpawnInterval
            : _hatchGroup.CurrentReleaseRate;

        public void OnMouseDown()
        {
            if (_hatchGroup.TryAddSpawnIntervalDelta(1))
            {
                LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
            }
        }

        public void OnPress(bool isDoubleTap)
        {
        }

        public void OnRightClick()
        {
            if (_hatchGroup.TryAddSpawnIntervalDelta(1000)) // Add extreme value - will be clamped appropriately
            {
                LevelScreen.LevelControlPanel.OnSpawnIntervalChanged();
            }
        }
    }

    [InlineArray(NumberOfSpawnIntervalChars)]
    private struct SpawnIntervalCharBuffer
    {
        private char _0;
    }
}
