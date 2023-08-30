namespace NeoLemmixSharp.Engine.Engine.Gadgets.Behaviours.Movement;

public interface IMovementBehaviour
{
    void Move(int dx, int dy);
    void SetPosition(int x, int y);
}