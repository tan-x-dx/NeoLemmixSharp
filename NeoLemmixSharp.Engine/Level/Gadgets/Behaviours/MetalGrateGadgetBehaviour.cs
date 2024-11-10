using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class MetalGrateGadgetBehaviour : GadgetBehaviour
{
    public static readonly MetalGrateGadgetBehaviour Instance = new();

    private MetalGrateGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.MetalGrateGadgetBehaviourId;
    public override string GadgetBehaviourName => "metal grate";
}