using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class MetalGrateGadgetType : InteractiveGadgetType
{
    public static MetalGrateGadgetType Instance { get; } = new();

    private MetalGrateGadgetType()
    {
    }

    public override int Id => GameConstants.MetalGrateGadgetTypeId;
    public override string GadgetTypeName => "metal grate";

    public override void InteractWithLemming(Lemming lemming)
    {
    }
}