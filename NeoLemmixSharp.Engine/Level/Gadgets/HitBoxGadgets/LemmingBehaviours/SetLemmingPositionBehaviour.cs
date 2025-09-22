using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingBehaviours;

public sealed class SetLemmingPositionBehaviour : LemmingBehaviour
{
    private readonly Point _desiredPosition;
    private readonly RelativePositioningType _relativePositioningType;

    public SetLemmingPositionBehaviour(Point desiredPosition, RelativePositioningType relativePositioningType)
        : base(LemmingBehaviourType.SetLemmingPosition)
    {
        _desiredPosition = desiredPosition;
        _relativePositioningType = relativePositioningType;
    }

    protected override void PerformInternalBehaviour(int triggerData)
    {
        var lemming = GetLemming(triggerData);
        SetLemmingPosition(ref lemming.AnchorPosition);

        LevelScreen.LemmingManager.UpdateLemmingPosition(lemming);
    }

    private void SetLemmingPosition(ref Point lemmingPosition)
    {
        if (_relativePositioningType == RelativePositioningType.RelativeToParentGadget)
        {
            var parentGadget = ParentGadget;
            var desiredPosition = parentGadget.Position + _desiredPosition;
            lemmingPosition = desiredPosition;
        }
        else
        {
            lemmingPosition = _desiredPosition;
        }
    }
}
