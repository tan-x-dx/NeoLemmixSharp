using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine;

public sealed class LemmingState
{
    public bool IsAthlete { get; set; }

    public Color HairColor => IsAthlete ? TeamAffiliation.BodyColor : TeamAffiliation.HairColor;
    public Color SkinColor { get; } = new(0xf0, 0xd0, 0xd0);
    public Color BodyColor => IsAthlete ? TeamAffiliation.HairColor : TeamAffiliation.BodyColor;


    public Team TeamAffiliation { get; set; } = Team.Team0;
}