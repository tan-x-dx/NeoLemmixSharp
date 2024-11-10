using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class TinkerableGadgetBehaviour : GadgetBehaviour
{
    public static readonly TinkerableGadgetBehaviour Instance = new();

    private TinkerableGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.TinkerableGadgetBehaviourId;
    public override string GadgetBehaviourName => "tinkerable";
}