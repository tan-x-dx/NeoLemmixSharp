namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public readonly struct GadgetActionData(GadgetActionType gadgetActionType, int miscData)
{
    public readonly GadgetActionType GadgetActionType = gadgetActionType;
    public readonly int MiscData = miscData;
}
