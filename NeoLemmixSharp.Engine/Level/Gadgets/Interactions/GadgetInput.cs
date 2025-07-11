using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

public abstract class GadgetInput : IEquatable<GadgetInput>
{
    public GadgetInputName InputName { get; }

    protected GadgetInput(GadgetInputName inputName)
    {
        InputName = inputName;
    }

    public virtual void OnRegistered() { }
    public abstract void ReactToSignal(bool signal);

    public bool Equals(GadgetInput? other) => other is not null && string.Equals(InputName, other.InputName);
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetInput other && string.Equals(InputName, other.InputName);
    public sealed override int GetHashCode() => InputName.GetHashCode();
    public sealed override string ToString() => InputName.ToString();
}
