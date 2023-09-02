using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Updates;

public interface IFrameUpdater
{
    UpdateState UpdateState { get; }

    void UpdateGadget(GadgetBase gadget);
    void UpdateLemming(Lemming lemming);
    bool Tick();
}