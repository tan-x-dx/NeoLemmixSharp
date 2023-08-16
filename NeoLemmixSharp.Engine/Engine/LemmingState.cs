using Microsoft.Xna.Framework;
using NeoLemmixSharp.Engine.Engine.Teams;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class LemmingState
{
    public bool IsAthlete { get; set; }

    public Color HairColor => IsAthlete ? TeamAffiliation.BodyColor : TeamAffiliation.HairColor;
    public Color SkinColor => TeamAffiliation.SkinColor;
    public Color BodyColor => IsAthlete ? TeamAffiliation.HairColor : TeamAffiliation.BodyColor;

    public Team TeamAffiliation { get; set; } = Team.AllItems[0];
}