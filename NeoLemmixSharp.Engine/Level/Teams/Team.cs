using Microsoft.Xna.Framework;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Rendering.Viewport.Lemming;

namespace NeoLemmixSharp.Engine.Level.Teams;

public sealed class Team : IExtendedEnumType<Team>
{
	private static readonly Team[] Teams = GenerateTeamCollection();

	public static int NumberOfItems => Teams.Length;
	public static ReadOnlySpan<Team> AllItems => new(Teams);

	private static Team[] GenerateTeamCollection()
	{
		var teams = new Team[LevelConstants.NumberOfTeams];

		for (var i = 0; i < teams.Length; i++)
		{
			teams[i] = new Team(i);
		}

		// Probably irrelevant here since it's done programatically, but whatever
		IdEquatableItemHelperMethods.ValidateUniqueIds(new ReadOnlySpan<Team>(teams));

		return teams;
	}

	public int Id { get; }

	public LemmingSpriteBank SpriteBank { get; private set; }

	public Color HairColor { get; private set; }
	public Color PermanentSkillHairColor { get; private set; }
	public Color SkinColor { get; private set; }
	public Color ZombieSkinColor { get; private set; }
	public Color BodyColor { get; private set; }
	public Color PermanentSkillBodyColor { get; private set; }
	public Color NeutralBodyColor { get; private set; }

	private Team(int id)
	{
		Id = id;
	}

	public void SetColorData(TeamColorData colorData)
	{
		HairColor = colorData.HairColor;
		PermanentSkillHairColor = colorData.PermanentSkillHairColor;
		SkinColor = colorData.SkinColor;
		ZombieSkinColor = colorData.ZombieSkinColor;
		BodyColor = colorData.BodyColor;
		PermanentSkillBodyColor = colorData.PermanentSkillBodyColor;
		NeutralBodyColor = colorData.NeutralBodyColor;
	}

	public void SetSpriteBank(LemmingSpriteBank spriteBank)
	{
		SpriteBank = spriteBank;
	}

	public bool Equals(Team? other) => Id == other?.Id;
	public override bool Equals(object? obj) => obj is Team other && Id == other.Id;
	public override int GetHashCode() => Id;

	public static bool operator ==(Team left, Team right) => left.Id == right.Id;
	public static bool operator !=(Team left, Team right) => left.Id != right.Id;
}