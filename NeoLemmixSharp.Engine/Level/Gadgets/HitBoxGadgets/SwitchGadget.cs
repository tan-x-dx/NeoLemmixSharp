using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class SwitchGadget : HitBoxGadget
{
    private HitBox _currentHitBox;
    private bool _facingRight;

    public override InteractiveGadgetType Type => SwitchGadgetType.Instance;
    public override Orientation Orientation => DownOrientation.Instance;

    public int AnimationFrame { get; private set; }
    public HitBox LeftHitBox { get; }
    public HitBox RightHitBox { get; }

    public GadgetOutput Output { get; } = new();

    public SwitchGadget(int id, RectangularLevelRegion gadgetBounds, bool faceRight)
        : base(id, gadgetBounds)
    {
        var p = gadgetBounds.TopLeft;
        var leftRect = new RectangularLevelRegion(p.X + 3, p.Y + 8, 5, 5);
        var leftHitBoxFilters = new ILemmingFilter[]
        {
            new LemmingActionFilter(null, Orientation, RightFacingDirection.Instance),
            new LemmingActionFilter(null, Orientation.GetOpposite(), LeftFacingDirection.Instance)
        };

        LeftHitBox = new HitBox(leftRect, leftHitBoxFilters);

        var rightRect = new RectangularLevelRegion(p.X + 10, p.Y + 8, 5, 5);
        var rightHitBoxFilters = new ILemmingFilter[]
        {
            new LemmingActionFilter(null, Orientation, LeftFacingDirection.Instance),
            new LemmingActionFilter(null, Orientation.GetOpposite(), RightFacingDirection.Instance)
        };

        RightHitBox = new HitBox(rightRect, rightHitBoxFilters);

        if (faceRight)
        {
            _facingRight = true;
            AnimationFrame = 6;
            _currentHitBox = RightHitBox;
        }
        else
        {
            _facingRight = false;
            AnimationFrame = 0;
            _currentHitBox = LeftHitBox;
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

    public override bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition)
    {
        return _currentHitBox.MatchesLemming(lemming) &&
               _currentHitBox.MatchesPosition(levelPosition);
    }

    public override bool MatchesPosition(LevelPosition levelPosition) => _currentHitBox.MatchesPosition(levelPosition);

    public override void OnLemmingMatch(Lemming lemming)
    {
        if (_facingRight)
        {
            _facingRight = false;
            _currentHitBox = LeftHitBox;
            Output.SetSignal(false);
        }
        else
        {
            _facingRight = true;
            _currentHitBox = RightHitBox;
            Output.SetSignal(true);
        }
    }
}