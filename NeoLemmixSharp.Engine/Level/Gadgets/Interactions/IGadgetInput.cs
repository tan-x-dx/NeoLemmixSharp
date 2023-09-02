namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

public interface IGadgetInput
{
    string InputName { get; }
    void ReactToSignal(bool signal);
}