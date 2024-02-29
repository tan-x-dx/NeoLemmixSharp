using NeoLemmixSharp.Engine.Level.Gadgets.Actions;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class GadgetStateData
{
    public required IGadgetAction[] OnLemmingEnterActions { get; init; }
    public required IGadgetAction[] OnLemmingPresentActions { get; init; }
    public required IGadgetAction[] OnLemmingExitActions { get; init; }

    public required int NumberOfFrames { get; init; }
    public required int TriggerX { get; init; }
    public required int TriggerY { get; init; }
    public required int TriggerWidth { get; init; }
    public required int TriggerHeight { get; init; }
}