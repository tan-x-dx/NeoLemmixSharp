namespace NeoLemmixSharp.Engine.Level.Gadgets.Interfaces;

public interface IResizeableGadget
{
    ResizeType ResizeType { get; }
    void Resize(int dw, int dh);
    void SetSize(int w, int h);
}
