using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible<int>
{
    private readonly SimpleList<GadgetInput> _inputs;
    protected readonly GadgetBounds _currentGadgetBounds;
    protected readonly GadgetBounds _previousGadgetBounds;

    public int Id { get; }
    public Orientation Orientation { get; }
    public abstract IGadgetRenderer Renderer { get; }

    public LevelPosition Position => _currentGadgetBounds.Position;
    public LevelSize Size => _currentGadgetBounds.Size;

    protected GadgetBase(
        int id,
        Orientation orientation,
        GadgetBounds gadgetBounds,
        int expectedNumberOfInputs)
    {
        Id = id;
        Orientation = orientation;
        _currentGadgetBounds = gadgetBounds;
        _previousGadgetBounds = new GadgetBounds();
        _previousGadgetBounds.SetFrom(gadgetBounds);
        _inputs = new SimpleList<GadgetInput>(expectedNumberOfInputs);
    }

    protected void RegisterInput(GadgetInput gadgetInput)
    {
        _inputs.Add(gadgetInput);
    }

    public bool TryGetInputWithName(string inputName, [MaybeNullWhen(false)] out GadgetInput gadgetInput)
    {
        var span = _inputs.AsReadOnlySpan();
        for (var i = 0; i < _inputs.Count; i++)
        {
            var input = span[i];
            if (string.Equals(input.InputName, inputName))
            {
                gadgetInput = input;
                return true;
            }
        }

        gadgetInput = null;
        return false;
    }

    public abstract void Tick();

    public bool Equals(GadgetBase? other) => Id == (other?.Id ?? -1);
    public sealed override bool Equals(object? obj) => obj is GadgetBase other && Id == other.Id;
    public sealed override int GetHashCode() => Id;

    public static bool operator ==(GadgetBase left, GadgetBase right) => left.Id == right.Id;
    public static bool operator !=(GadgetBase left, GadgetBase right) => left.Id != right.Id;

    public void WriteToSnapshotData(out int snapshotData)
    {
        snapshotData = 0;
    }

    public void SetFromSnapshotData(in int snapshotData)
    {
    }
}
