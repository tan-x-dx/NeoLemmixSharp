using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible<int>
{
    private readonly Dictionary<string, IGadgetInput> _inputs = [];

    public int Id { get; }
    public abstract IGadgetRenderer Renderer { get; }

    protected GadgetBase(int id)
    {
        Id = id;
    }

    protected void RegisterInput(IGadgetInput gadgetInput) => _inputs.Add(gadgetInput.InputName, gadgetInput);

    public bool TryGetInputWithName(string inputName, [MaybeNullWhen(false)] out IGadgetInput gadgetInput)
    {
        return _inputs.TryGetValue(inputName, out gadgetInput);
    }

    public virtual void Tick() { }

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;

    public void ToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
    }
}
