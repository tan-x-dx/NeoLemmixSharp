using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class UpdraftGadgetType : InteractiveGadgetType
{
    public static UpdraftGadgetType Instance { get; } = new();

    private UpdraftGadgetType()
    {
    }

    public override int Id => Global.UpdraftGadgetTypeId;
    public override string GadgetTypeName => "updraft";

    public override void InteractWithLemming(Lemming lemming)
    {
    }
}