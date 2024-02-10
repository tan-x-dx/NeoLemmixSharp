namespace NeoLemmixSharp.Engine.Level.Gadgets.InteractionTypes;

public sealed class WaterGadgetSubType : GadgetSubType
{
    public static readonly WaterGadgetSubType Instance = new();

    private WaterGadgetSubType()
    {
    }

    public override int Id => LevelConstants.WaterGadgetTypeId;
    public override string GadgetTypeName => "water";
}