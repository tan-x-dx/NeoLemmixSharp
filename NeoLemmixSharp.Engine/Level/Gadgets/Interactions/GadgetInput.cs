namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

public abstract class GadgetInput : IEquatable<GadgetInput>
{
    public string InputName { get; }

    protected GadgetInput(string inputName)
    {
        InputName = inputName;
    }

    public virtual void OnRegistered() { }
    public abstract void ReactToSignal(bool signal);

    public bool Equals(GadgetInput? other) => other is not null && string.Equals(InputName, other.InputName);
    public sealed override bool Equals(object? obj) => obj is GadgetInput other && string.Equals(InputName, other.InputName);
    public sealed override int GetHashCode() => InputName.GetHashCode();
    public sealed override string ToString() => InputName;
}