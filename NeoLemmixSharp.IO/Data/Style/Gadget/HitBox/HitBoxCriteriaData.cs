namespace NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;

public sealed class HitBoxCriteriaData
{
    public required uint[] AllowedLemmingActionIds { get; init; }
    public required uint[] AllowedLemmingStateIds { get; init; }
    public required byte AllowedLemmingTribeIds { get; init; }
    public required byte AllowedLemmingOrientationIds { get; init; }
    public required byte AllowedFacingDirectionId { get; init; }
}
