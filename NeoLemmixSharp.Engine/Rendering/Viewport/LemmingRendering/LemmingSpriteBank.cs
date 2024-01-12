using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

public sealed class LemmingSpriteBank : IDisposable
{
	private readonly ActionSprite[] _actionSprites;
	private readonly TeamColorData[] _teamColorData;

	public LemmingSpriteBank(ActionSprite[] actionSprites, TeamColorData[] teamColorData)
	{
		_actionSprites = actionSprites;
		_teamColorData = teamColorData;
	}

	public ActionSprite GetActionSprite(
		LemmingAction lemmingAction,
		Orientation orientation,
		FacingDirection facingDirection)
	{
		if (lemmingAction == NoneAction.Instance)
			return EmptyActionSprite.Instance;

		var key = GetKey(lemmingAction, orientation, facingDirection);

		return _actionSprites[key];
	}

	public static int GetKey(
		LemmingAction lemmingAction,
		Orientation orientation,
		FacingDirection facingDirection)
	{
		if (lemmingAction == NoneAction.Instance)
			throw new InvalidOperationException("Cannot render \"None\" action");

		var lowerBits = GetKey(orientation, facingDirection);

		return (lemmingAction.Id << 3) | lowerBits;
	}

	public static int GetKey(
		Orientation orientation,
		FacingDirection facingDirection)
	{
		return (orientation.RotNum << 1) | facingDirection.Id;
	}

	public void Dispose()
	{
		DisposableHelperMethods.DisposeOfAll(new ReadOnlySpan<ActionSprite>(_actionSprites));
	}

	public void SetTeamColors(Team team)
	{
		team.SetColorData(_teamColorData[team.Id]);
	}
}