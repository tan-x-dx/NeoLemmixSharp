using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class SplatGadgetType : InteractiveGadgetType
{
    public static SplatGadgetType Instance { get; } = new();

    private SplatGadgetType()
    {
    }

    public override int Id => GameConstants.SplatGadgetTypeId;
    public override string GadgetTypeName => "splat";

    public override void InteractWithLemming(Lemming lemming)
    {

    }
}