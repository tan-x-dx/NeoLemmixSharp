using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Rewind.SnapshotData;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public abstract class GadgetBase : IIdEquatable<GadgetBase>, ISnapshotDataConvertible<int>
{
    private readonly SimpleList<GadgetInput> _inputs;
    private AnimationController _currentAnimationController;

    public required GadgetBounds CurrentGadgetBounds { protected get; init; }
    public required GadgetBounds PreviousGadgetBounds { protected get; init; }

    public required int Id { get; init; }
    public required Orientation Orientation { get; init; }
    public required bool IsFastForward { get; init; }

    public AnimationController CurrentAnimationController
    {
        get => _currentAnimationController;
        protected set
        {
            if (_currentAnimationController == value)
                return;

            _currentAnimationController = value;
            value.OnTransitionTo();
        }
    }

    public LevelPosition Position => CurrentGadgetBounds.Position;
    public LevelSize Size => CurrentGadgetBounds.Size;

    public GadgetRenderer Renderer { get; internal set; }

    protected GadgetBase(int expectedNumberOfInputs)
    {
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
