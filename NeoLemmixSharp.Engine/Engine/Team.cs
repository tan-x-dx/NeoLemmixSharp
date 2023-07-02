using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Engine.Engine;

public sealed class Team : IEquatable<Team>
{
    public static Team Team0 { get; } = new(0, new Color(0x04,0xB0,0x00), new Color(0x40, 0x44, 0xDF)); // 0∘
    public static Team Team1 { get; } = new(1, new Color(0x00,0xB0,0xA9), new Color(0xD5, 0x3F, 0xDE)); // 60∘
    public static Team Team2 { get; } = new(2, new Color(0x00,0x04,0xB0), new Color(0xDE, 0x3F, 0x46)); // 120∘
    public static Team Team3 { get; } = new(3, new Color(0xAD,0x00,0xB0), new Color(0xDE, 0xD1, 0x3F)); // 180∘
    public static Team Team4 { get; } = new(4, new Color(0xB0,0x00,0x00), new Color(0x4A, 0xDE, 0x3F)); // 240∘
    public static Team Team5 { get; } = new(5, new Color(0xB0,0xA9,0x00), new Color(0x3F, 0xDE, 0xD5)); // 300∘

    public int Id { get; }
    public Color HairColor { get; }
    public Color BodyColor { get; }

    private Team(int id, Color hairColor, Color bodyColor)
    {
        Id = id;
        
        HairColor = hairColor;
        BodyColor = bodyColor;
    }

    public bool Equals(Team? other)
    {
        if (ReferenceEquals(null, other)) return false;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) => obj is Team other && Id == other.Id;
    public override int GetHashCode() => Id;

    public static bool operator ==(Team left, Team right) => left.Id == right.Id;
    public static bool operator !=(Team left, Team right) => left.Id != right.Id;
}