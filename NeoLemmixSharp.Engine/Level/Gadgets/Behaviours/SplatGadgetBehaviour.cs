using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class SplatGadgetBehaviour : GadgetBehaviour
{
    public static readonly SplatGadgetBehaviour Instance = new();

    private SplatGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.SplatGadgetBehaviourId;
    public override string GadgetBehaviourName => "splat";
}