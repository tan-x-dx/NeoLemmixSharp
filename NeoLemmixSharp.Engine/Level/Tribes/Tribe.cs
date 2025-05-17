using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.Level.Tribes;

public sealed class Tribe : IIdEquatable<Tribe>
{
    public readonly int Id;
    public readonly Color HairColor;
    public readonly Color PermanentSkillHairColor;
    public readonly Color SkinColor;
    public readonly Color AcidLemmingFootColor;
    public readonly Color WaterLemmingFootColor;
    public readonly Color ZombieSkinColor;
    public readonly Color BodyColor;
    public readonly Color PermanentSkillBodyColor;
    public readonly Color NeutralBodyColor;
    public readonly Color PaintColor;
    public readonly LemmingSpriteBank SpriteBank;

    public Tribe(
        int id,
        LemmingSpriteBank spriteBank)
    {
        Id = id;

        var colorData = spriteBank.GetColorData(id);
        HairColor = colorData.HairColor;
        PermanentSkillHairColor = colorData.PermanentSkillHairColor;
        SkinColor = colorData.SkinColor;
        AcidLemmingFootColor = colorData.AcidLemmingFootColor;
        WaterLemmingFootColor = colorData.WaterLemmingFootColor;
        ZombieSkinColor = colorData.ZombieSkinColor;
        BodyColor = colorData.BodyColor;
        PermanentSkillBodyColor = colorData.PermanentSkillBodyColor;
        NeutralBodyColor = colorData.NeutralBodyColor;
        PaintColor = colorData.PaintColor;
        SpriteBank = spriteBank;
    }

    int IIdEquatable<Tribe>.Id => Id;
    [DebuggerStepThrough]
    public bool Equals(Tribe? other) => Id == (other?.Id ?? -1);
    [DebuggerStepThrough]
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Tribe other && Id == other.Id;
    [DebuggerStepThrough]
    public override int GetHashCode() => Id;
    [DebuggerStepThrough]
    public static bool operator ==(Tribe left, Tribe right) => left.Id == right.Id;
    [DebuggerStepThrough]
    public static bool operator !=(Tribe left, Tribe right) => left.Id != right.Id;
}