using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.IO.Data.Style.Theme;

public sealed class TribeColorData
{
    public required Color HairColor { get; init; }
    public required Color PermanentSkillHairColor { get; init; }

    public required Color SkinColor { get; init; }
    public required Color ZombieSkinColor { get; init; }

    public required Color BodyColor { get; init; }
    public required Color PermanentSkillBodyColor { get; init; }
    public required Color NeutralBodyColor { get; init; }

    public required Color AcidLemmingFootColor { get; init; }
    public required Color WaterLemmingFootColor { get; init; }

    public required Color PaintColor { get; init; }
}