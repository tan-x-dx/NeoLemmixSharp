using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.MetalGrates;

public sealed partial class MetalGrateGadget
{
    private sealed class MetalGrateHitBox : IHitBox
    {
        private readonly RectangularLevelRegion _rectangle;

        public MetalGrateHitBox(RectangularLevelRegion rectangle)
        {
            _rectangle = rectangle;
        }

        public bool MatchesLemming(Lemming lemming, LevelPosition levelPosition)
        {
            return _rectangle.ContainsPoint(lemming.LevelPosition) ||
                   _rectangle.ContainsPoint(lemming.Orientation.MoveUp(lemming.LevelPosition, 1));
        }
    }

    private sealed class DeactivatedState : IGadgetState
    {
        private readonly MetalGrateHitBox _hitBox;

        public int AnimationFrame => 0;
        IHitBox IGadgetState.HitBox => _hitBox;

        public DeactivatedState(MetalGrateHitBox hitBox)
        {
            _hitBox = hitBox;
        }

        public void OnTransitionTo()
        {
        }

        public void Tick()
        {
        }

        public void OnTransitionFrom()
        {
        }

        public void OnLemmingInHitBox(Lemming lemming)
        {

        }
    }

    private sealed class TransitionToActiveState : IGadgetState
    {
        private readonly MetalGrateHitBox _hitBox;

        public int AnimationFrame { get; private set; }
        IHitBox IGadgetState.HitBox => _hitBox;

        public TransitionToActiveState(MetalGrateHitBox hitBox)
        {
            _hitBox = hitBox;
        }

        public void OnTransitionTo()
        {
        }

        public void Tick()
        {
        }

        public void OnTransitionFrom()
        {
        }

        public void OnLemmingInHitBox(Lemming lemming)
        {

        }
    }

    private sealed class ActivatedState : IGadgetState
    {
        private readonly MetalGrateHitBox _hitBox;

        public int AnimationFrame => 0;
        IHitBox IGadgetState.HitBox => _hitBox;

        public ActivatedState(MetalGrateHitBox hitBox)
        {
            _hitBox = hitBox;
        }

        public void OnTransitionTo()
        {
        }

        public void Tick()
        {
        }

        public void OnTransitionFrom()
        {
        }

        public void OnLemmingInHitBox(Lemming lemming)
        {

        }
    }

    private sealed class TransitionToDeactivatedState : IGadgetState
    {
        private readonly MetalGrateHitBox _hitBox;

        public int AnimationFrame { get; private set; }
        IHitBox IGadgetState.HitBox => _hitBox;

        public TransitionToDeactivatedState(MetalGrateHitBox hitBox)
        {
            _hitBox = hitBox;
        }

        public void OnTransitionTo()
        {
        }

        public void Tick()
        {
        }

        public void OnTransitionFrom()
        {
        }

        public void OnLemmingInHitBox(Lemming lemming)
        {

        }
    }
}