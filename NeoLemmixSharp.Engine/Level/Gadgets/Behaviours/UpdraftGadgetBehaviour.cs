using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;

public sealed class UpdraftGadgetBehaviour : GadgetBehaviour
{
    public static readonly UpdraftGadgetBehaviour Instance = new();

    private UpdraftGadgetBehaviour()
    {
    }

    public override int Id => EngineConstants.UpdraftGadgetBehaviourId;
    public override string GadgetBehaviourName => "updraft";
}