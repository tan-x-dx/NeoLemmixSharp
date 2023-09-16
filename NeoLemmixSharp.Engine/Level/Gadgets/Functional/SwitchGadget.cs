using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Functional;

public sealed class SwitchGadget : GadgetBase
{
    private readonly LevelRegionHitBoxBehaviour _leftHitBox;
    private readonly LevelRegionHitBoxBehaviour _rightHitBox;

    private LevelRegionHitBoxBehaviour _currentHitBox;
    private bool _facingRight;

    public override GadgetType Type => GadgetType.Switch;
    public override Orientation Orientation => DownOrientation.Instance;

    public int AnimationFrame { get; private set; }
    public GadgetOutput Output { get; } = new();

    public SwitchGadget(int id, RectangularLevelRegion gadgetBounds, bool faceRight)
        : base(id, gadgetBounds)
    {
        var p = gadgetBounds.TopLeft;
        var leftRect = new RectangularLevelRegion(p.X + 3, p.Y + 8, 5, 5);
        _leftHitBox = new LevelRegionHitBoxBehaviour(leftRect, LemmingManager);
        _leftHitBox.ExcludeFacingDirection(LeftFacingDirection.Instance);

        var rightRect = new RectangularLevelRegion(p.X + 10, p.Y + 8, 5, 5);
        _rightHitBox = new LevelRegionHitBoxBehaviour(rightRect, LemmingManager);
        _rightHitBox.ExcludeFacingDirection(RightFacingDirection.Instance);

        if (faceRight)
        {
            _facingRight = true;
            AnimationFrame = 6;
            _currentHitBox = _rightHitBox;
        }
        else
        {
            _facingRight = false;
            AnimationFrame = 0;
            _currentHitBox = _leftHitBox;
        }
    }

    public override void Tick()
    {
        if (_facingRight)
        {
            if (AnimationFrame < 6)
            {
                AnimationFrame++;
            }
        }
        else
        {
            if (AnimationFrame > 0)
            {
                AnimationFrame--;
            }
        }
    }

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        return null;
    }

    public override bool CaresAboutLemmingInteraction => true;

    public override bool MatchesLemming(Lemming lemming) => _currentHitBox.MatchesLemming(lemming);
    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return _currentHitBox.MatchesLemmingData(lemming) &&
               _currentHitBox.MatchesPosition(levelPosition);
    }

    public override void OnLemmingMatch(Lemming lemming)
    {
        if (_facingRight)
        {
            _facingRight = false;
            _currentHitBox = _leftHitBox;
            Output.SetSignal(false);
        }
        else
        {
            _facingRight = true;
            _currentHitBox = _rightHitBox;
            Output.SetSignal(true);
        }
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => _currentHitBox.MatchesPosition(levelPosition);
}