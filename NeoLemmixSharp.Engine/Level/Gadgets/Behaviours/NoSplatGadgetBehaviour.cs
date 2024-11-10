using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class NoSplatGadgetBehaviour : GadgetBehaviour
{
    public static readonly NoSplatGadgetBehaviour Instance = new();

    private NoSplatGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.NoSplatGadgetBehaviourId;
    public override string GadgetBehaviourName => "no splat";
}