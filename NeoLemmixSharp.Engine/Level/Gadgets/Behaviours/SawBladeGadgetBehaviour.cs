using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class SawBladeGadgetBehaviour : GadgetBehaviour
{
    public static readonly SawBladeGadgetBehaviour Instance = new();

    private SawBladeGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.SawBladeGadgetBehaviourId;
    public override string GadgetBehaviourName => "saw blade";
}