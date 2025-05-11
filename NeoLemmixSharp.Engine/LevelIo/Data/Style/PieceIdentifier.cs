using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Data.Style;

public readonly struct PieceIdentifier : IEquatable<PieceIdentifier>
{
    private readonly string _pieceName;

    public PieceIdentifier(string? pieceName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pieceName);

        _pieceName = pieceName;
    }

    public override string ToString() => _pieceName;

    public bool Equals(PieceIdentifier other) => string.Equals(_pieceName, other._pieceName);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is PieceIdentifier other && Equals(other);
    public override int GetHashCode() => _pieceName.GetHashCode();
    public static bool operator ==(PieceIdentifier left, PieceIdentifier right) => left.Equals(right);
    public static bool operator !=(PieceIdentifier left, PieceIdentifier right) => !left.Equals(right);
}
