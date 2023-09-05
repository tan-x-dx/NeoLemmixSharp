using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Updates;

public interface IFrameUpdater
{
    UpdateState UpdateState { get; }

    bool UpdateGadget(GadgetBase gadget);
    bool UpdateLemming(Lemming lemming);
    bool Tick();
}