using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class FunctionalGadgetBehaviour : GadgetBehaviour
{
    public static readonly FunctionalGadgetBehaviour Instance = new();

    private FunctionalGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.FunctionalGadgetBehaviourId;
    public override string GadgetBehaviourName => "functional";
}