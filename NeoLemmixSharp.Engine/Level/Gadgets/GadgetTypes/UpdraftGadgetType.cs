namespace NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;

public sealed class UpdraftGadgetType : GadgetType
{
    public static UpdraftGadgetType Instance { get; } = new();

    private UpdraftGadgetType()
    {
    }

    public override int Id => GameConstants.UpdraftGadgetTypeId;
    public override string GadgetTypeName => "updraft";
}