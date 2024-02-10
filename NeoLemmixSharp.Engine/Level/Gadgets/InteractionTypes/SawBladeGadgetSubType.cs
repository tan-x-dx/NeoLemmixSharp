namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class SawBladeGadgetSubType : GadgetSubType
{
    public static readonly SawBladeGadgetSubType Instance = new();

    private SawBladeGadgetSubType()
    {
    }

    public override int Id => LevelConstants.SawBladeGadgetTypeId;
    public override string GadgetTypeName => "saw blade";
}