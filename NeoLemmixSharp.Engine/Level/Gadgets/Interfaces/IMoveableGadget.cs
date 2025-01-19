namespace NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;

public interface IMoveableGadget
{
    void Move(int dx, int dy);
    void SetPosition(int x, int y);
}
