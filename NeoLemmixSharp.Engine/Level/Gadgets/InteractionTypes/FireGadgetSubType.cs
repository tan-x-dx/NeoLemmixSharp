namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class FireGadgetSubType : GadgetSubType
{
    public static readonly FireGadgetSubType Instance = new();

    private FireGadgetSubType()
    {
    }

    public override int Id => LevelConstants.FireGadgetTypeId;
    public override string GadgetTypeName => "fire";
}