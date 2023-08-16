﻿using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;

namespace NeoLemmixSharp.Engine.Engine.Teams;

public sealed class Team : IUniqueIdItem<Team>
{
    private static readonly Team[] Teams = GenerateTeamCollection();

    public static int NumberOfItems => Teams.Length;
    public static ReadOnlySpan<Team> AllItems => new(Teams);

    private static Team[] GenerateTeamCollection()
    {
        var teams = new Team[GameConstants.NumberOfTeams];

        for (var i = 0; i < teams.Length; i++)
        {
            teams[i] = new Team(i);
        }

        // Probably irrelevant here since it's done programatically, but whatever
        teams.ValidateUniqueIds();

        return teams;
    }

    public int Id { get; }
    public Color HairColor { get; private set; }
    public Color SkinColor { get; private set; }
    public Color BodyColor { get; private set; }

    private Team(int id)
    {
        Id = id;
    }

    public void SetColorData(TeamColorData colorData)
    {
        HairColor = colorData.HairColor;
        SkinColor = colorData.SkinColor;
        BodyColor = colorData.BodyColor;
    }

    public bool Equals(Team? other) => Id == other?.Id;
    public override bool Equals(object? obj) => obj is Team other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Team left, Team right) => left.Id == right.Id;
    public static bool operator !=(Team left, Team right) => left.Id != right.Id;
}