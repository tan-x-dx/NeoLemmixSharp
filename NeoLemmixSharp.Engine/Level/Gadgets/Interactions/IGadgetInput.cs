namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

public interface IGadgetInput
{
    string InputName { get; }
    void OnRegistered();
    void ReactToSignal(bool signal);
}