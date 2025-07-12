using NeoLemmixSharp.IO.Data.Style.Gadget;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Interactions;

public abstract class GadgetLinkInput : IEquatable<GadgetLinkInput>
{
    public GadgetInputName InputName { get; }

    protected GadgetLinkInput(GadgetInputName inputName)
    {
        InputName = inputName;
    }

    public virtual void OnRegistered() { }
    public abstract void ReactToSignal(bool signal);

    public bool Equals(GadgetLinkInput? other) => other is not null && InputName.Equals(other.InputName);
    public sealed override bool Equals([NotNullWhen(true)] object? obj) => obj is GadgetLinkInput other && InputName.Equals(other.InputName);
    public sealed override int GetHashCode() => InputName.GetHashCode();
    public sealed override string ToString() => InputName.ToString();
}
