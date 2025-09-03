using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonBehaviours.Movement;

public interface IMoveableGadget
{
    Point Position { get; }
    void Move(Point delta);
    void SetPosition(Point position);
}
