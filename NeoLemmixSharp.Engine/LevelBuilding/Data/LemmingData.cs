using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.Level.FacingDirections;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data;

public sealed class LemmingData
{
    public int X { get; set; }
    public int Y { get; set; }
    public uint State { get; set; }

    public Orientation Orientation { get; set; } = DownOrientation.Instance;
    public FacingDirection FacingDirection { get; set; } = FacingDirection.RightInstance;
    public Team Team { get; set; } = Team.AllItems[LevelConstants.ClassicTeamId];
    public LemmingAction InitialLemmingAction { get; set; } = WalkerAction.Instance;
}