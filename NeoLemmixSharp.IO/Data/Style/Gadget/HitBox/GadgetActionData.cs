namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public readonly struct GadgetActionData(LemmingBehaviourType gadgetActionType, int miscData)
{
    public readonly LemmingBehaviourType GadgetActionType = gadgetActionType;
    public readonly int MiscData = miscData;
}
