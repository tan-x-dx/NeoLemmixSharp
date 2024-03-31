namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public readonly struct RectangularTriggerData
{
    public readonly int TriggerX;
    public readonly int TriggerY;
    public readonly int TriggerWidth;
    public readonly int TriggerHeight;

    public RectangularTriggerData(int triggerX, int triggerY, int triggerWidth, int triggerHeight)
    {
        TriggerX = triggerX;
        TriggerY = triggerY;
        TriggerWidth = triggerWidth;
        TriggerHeight = triggerHeight;
    }
}