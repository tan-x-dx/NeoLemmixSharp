﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.States;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

public sealed class SwitchGadget : HitBoxGadget
{
    private HitBox _currentHitBox;
    private bool _facingRight;

    public override GadgetType Type => GadgetType.Switch;
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
        LeftHitBox = new HitBox(leftRect, LemmingManager);
        LeftHitBox.ExcludeFacingDirection(LeftFacingDirection.Instance);

        var rightRect = new RectangularLevelRegion(p.X + 10, p.Y + 8, 5, 5);
        RightHitBox = new HitBox(rightRect, LemmingManager);
        RightHitBox.ExcludeFacingDirection(RightFacingDirection.Instance);

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

    public override IGadgetInput? GetInputWithName(string inputName)
    {
        return null;
    }

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

    public override bool MatchesPosition(LevelPosition levelPosition) => _currentHitBox.MatchesPosition(levelPosition);
}