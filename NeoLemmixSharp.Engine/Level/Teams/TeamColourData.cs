using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Level.Teams;

public sealed class TeamColorData
{
    public required Color HairColor { get; init; }
    public required Color SkinColor { get; init; }
    public required Color ZombieSkinColor { get; init; }
    public required Color BodyColor { get; init; }
    public required Color NeutralBodyColor { get; init; }
}