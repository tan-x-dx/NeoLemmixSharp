using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Style;

public readonly struct StylePiecePair(StyleIdentifier styleName, PieceIdentifier pieceName) : IEquatable<StylePiecePair>
{
    public readonly StyleIdentifier StyleName = styleName;
    public readonly PieceIdentifier PieceName = pieceName;

    public override string ToString() => $"{StyleName}:{PieceName}";

    public bool Equals(StylePiecePair other) => StyleName.Equals(other.StyleName) &&
                                                PieceName.Equals(other.PieceName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is StylePiecePair other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StyleName, PieceName);
    public static bool operator ==(StylePiecePair left, StylePiecePair right) => left.Equals(right);
    public static bool operator !=(StylePiecePair left, StylePiecePair right) => !left.Equals(right);
}
