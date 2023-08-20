using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Gadgets.HitBoxes;
using NeoLemmixSharp.Engine.Engine.Gadgets.States;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using NeoLemmixSharp.Engine.Engine.Orientations;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Logic;

public sealed class AndGateGadget : IGadget, IGadgetState
{
    public int Id { get; }

    public GadgetType Type => GadgetType.Logic;
    public Orientation Orientation => DownOrientation.Instance;
    public LevelPosition LevelPosition { get; }

    IGadgetState IGadget.CurrentState => this;

    public AndGateGadget(int id, LevelPosition levelPosition)
    {
        Id = id;
        LevelPosition = levelPosition;
    }

    public void Tick()
    {
    }

    public int AnimationFrame => 0;
    public IHitBox HitBox => EmptyHitBox.Instance;

    void IGadgetState.OnTransitionTo()
    {
    }

    void IGadgetState.OnTransitionFrom()
    {
    }

    void IGadgetState.OnLemmingInHitBox(Lemming lemming)
    {
    }
}