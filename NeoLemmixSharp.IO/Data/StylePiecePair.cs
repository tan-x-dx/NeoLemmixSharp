using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data;

[DebuggerDisplay("{StyleName}:{PieceName}")]
public readonly record struct StylePiecePair(StyleIdentifier StyleIdentifier, PieceIdentifier PieceIdentifier);
