using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetSubTypes;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.LemmingActions;

public sealed class OhNoerAction : LemmingAction
{
	public static readonly OhNoerAction Instance = new();

	private static SimpleSet<LemmingAction> _airborneActions = null!;

	private OhNoerAction()
	{
	}

	public static void Initialise()
	{
		_airborneActions = ExtendedEnumTypeComparer<LemmingAction>.CreateSimpleSet();

		_airborneActions.Add(VaporiserAction.Instance);
		_airborneActions.Add(DrownerAction.Instance);
		_airborneActions.Add(FloaterAction.Instance);
		_airborneActions.Add(GliderAction.Instance);
		_airborneActions.Add(FallerAction.Instance);
		_airborneActions.Add(SwimmerAction.Instance);
		_airborneActions.Add(ReacherAction.Instance);
		_airborneActions.Add(ShimmierAction.Instance);
		_airborneActions.Add(JumperAction.Instance);
	}

	public override int Id => LevelConstants.OhNoerActionId;
	public override string LemmingActionName => "ohnoer";
	public override int NumberOfAnimationFrames => LevelConstants.OhNoerAnimationFrames;
	public override bool IsOneTimeAction => true;
	public override int CursorSelectionPriorityValue => LevelConstants.NonWalkerMovementPriority;

	public override bool UpdateLemming(Lemming lemming)
	{
		ref var lemmingPosition = ref lemming.LevelPosition;

		if (lemming.EndOfAnimation)
		{
			LevelScreen.LemmingManager.DeregisterBlocker(lemming);
			var nextAction = lemming.CountDownAction;
			nextAction.TransitionLemmingToAction(lemming, false);
			lemming.ClearCountDownAction();
			return !nextAction.IsOneTimeAction;
		}

		if (LevelScreen.TerrainManager.PixelIsSolidToLemming(lemming, lemmingPosition))
			return true;

		LevelScreen.LemmingManager.DeregisterBlocker(lemming);

		var fallDelta = GetFallDelta(lemming);

		var lemmingOrientation = lemming.Orientation;
		lemmingPosition = lemmingOrientation.MoveDown(lemmingPosition, fallDelta);

		return true;
	}

	private static int GetFallDelta(Lemming lemming)
	{
		var lemmingOrientation = lemming.Orientation;

		var hasUpdraft = false;
		var hasDowndraft = false;

		var gadgetManager = LevelScreen.GadgetManager;
		var gadgetsNearPosition = gadgetManager.GetAllGadgetsAtLemmingPosition(lemming);

		if (gadgetsNearPosition.Count > 0)
		{
			foreach (var gadget in gadgetsNearPosition)
			{
				if (gadget.SubType != UpdraftGadgetType.Instance || !gadget.MatchesLemming(lemming))
					continue;

				var gadgetOrientation = gadget.Orientation;

				if (gadgetOrientation == lemmingOrientation)
				{
					hasDowndraft = true;
				}
				if (gadgetOrientation == Orientation.GetOpposite(lemmingOrientation))
				{
					hasUpdraft = true;
				}
			}
		}

		if (hasUpdraft && hasDowndraft)
			return LevelConstants.DefaultFallStep;

		if (hasUpdraft)
			return LevelConstants.UpdraftFallStep;

		if (hasDowndraft)
			return LevelConstants.DownDraftFallStep;

		return LevelConstants.DefaultFallStep;
	}

	protected override int TopLeftBoundsDeltaX(int animationFrame) => -3;
	protected override int TopLeftBoundsDeltaY(int animationFrame) => animationFrame < 7 ? 10 : 9;

	protected override int BottomRightBoundsDeltaX(int animationFrame) => 3;

	public static void HandleCountDownTransition(Lemming lemming)
	{
		var currentAction = lemming.CurrentAction;

		if (currentAction == NoneAction.Instance)
			return;

		if (_airborneActions.Contains(currentAction))
		{
			// If in the air, do the action immediately
			lemming.CountDownAction.TransitionLemmingToAction(lemming, false);
			lemming.ClearCountDownAction();
			return;
		}

		Instance.TransitionLemmingToAction(lemming, false); // Otherwise start oh-noing!
	}
}