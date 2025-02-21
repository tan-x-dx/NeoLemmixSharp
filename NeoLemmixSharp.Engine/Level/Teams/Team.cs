﻿using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Rendering.Viewport.LemmingRendering;

namespace NeoLemmixSharp.Engine.Level.Teams;

public sealed class Team : IIdEquatable<Team>
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
    public readonly LemmingSpriteBank SpriteBank;

    public Team(
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
        SpriteBank = spriteBank;
    }

    int IIdEquatable<Team>.Id => Id;
    public bool Equals(Team? other) => Id == (other?.Id ?? -1);
    public override bool Equals(object? obj) => obj is Team other && Id == other.Id;
    public override int GetHashCode() => Id;
    public static bool operator ==(Team left, Team right) => left.Id == right.Id;
    public static bool operator !=(Team left, Team right) => left.Id != right.Id;
}