namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public readonly struct GadgetActionData(GadgetActionType gadgetActionType, int miscData)
{
    public readonly GadgetActionType GadgetActionType = gadgetActionType;
    public readonly int MiscData = miscData;
}
