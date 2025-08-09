using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data;

[DebuggerDisplay("{StyleName}:{PieceName}")]
public readonly struct StylePiecePair(StyleIdentifier styleIdentifier, PieceIdentifier pieceIdentifier) : IEquatable<StylePiecePair>
{
    public readonly StyleIdentifier StyleIdentifier = styleIdentifier;
    public readonly PieceIdentifier PieceIdentifier = pieceIdentifier;

    public bool Equals(StylePiecePair other) => StyleIdentifier.Equals(other.StyleIdentifier) &&
                                                PieceIdentifier.Equals(other.PieceIdentifier);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is StylePiecePair other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StyleIdentifier, PieceIdentifier);
    public static bool operator ==(StylePiecePair left, StylePiecePair right) => left.Equals(right);
    public static bool operator !=(StylePiecePair left, StylePiecePair right) => !left.Equals(right);
}
