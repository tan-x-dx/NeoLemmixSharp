namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class UpdraftGadgetSubType : GadgetSubType
{
    public static readonly UpdraftGadgetSubType Instance = new();

    private UpdraftGadgetSubType()
    {
    }

    public override int Id => LevelConstants.UpdraftGadgetTypeId;
    public override string GadgetTypeName => "updraft";
}