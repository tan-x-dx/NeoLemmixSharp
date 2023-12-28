namespace NeoLemmixSharp.Engine.Level.ControlPanel.Buttons;

public enum ButtonType
{
	Padding,
	SkillAssign,
	SkillScrollLeft,
	SkillScrollRight,
	SpawnIntervalDecrease,
	SpawnIntervalDisplay,
	SpawnIntervalIncrease,
	Pause,
	Nuke,
	FastForward,
	Restart,
	NudgeFrameBack,
	NudgeFrameForward,
	DirectionalSelectLeft,
	DirectionalSelectRight,
	ClearPhysics,
	Replay
}

public enum ButtonTypeSizePosition
{
	Normal,
	TopHalf,
	BottomHalf
}

public static class ButtonTypeHelpers
{
	public static ButtonTypeSizePosition GetButtonTypeSizePosition(this ButtonType type) => type switch
	{
		ButtonType.Padding => ButtonTypeSizePosition.Normal,
		ButtonType.SkillAssign => ButtonTypeSizePosition.Normal,
		ButtonType.SkillScrollLeft => ButtonTypeSizePosition.Normal,
		ButtonType.SkillScrollRight => ButtonTypeSizePosition.Normal,
		ButtonType.SpawnIntervalDecrease => ButtonTypeSizePosition.Normal,
		ButtonType.SpawnIntervalDisplay => ButtonTypeSizePosition.Normal,
		ButtonType.SpawnIntervalIncrease => ButtonTypeSizePosition.Normal,
		ButtonType.Pause => ButtonTypeSizePosition.Normal,
		ButtonType.Nuke => ButtonTypeSizePosition.Normal,
		ButtonType.FastForward => ButtonTypeSizePosition.Normal,
		ButtonType.Restart => ButtonTypeSizePosition.Normal,
		ButtonType.NudgeFrameBack => ButtonTypeSizePosition.TopHalf,
		ButtonType.NudgeFrameForward => ButtonTypeSizePosition.BottomHalf,
		ButtonType.DirectionalSelectLeft => ButtonTypeSizePosition.TopHalf,
		ButtonType.DirectionalSelectRight => ButtonTypeSizePosition.BottomHalf,
		ButtonType.ClearPhysics => ButtonTypeSizePosition.TopHalf,
		ButtonType.Replay => ButtonTypeSizePosition.BottomHalf,

		_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
	};
}