using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data;

[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public readonly record struct StylePiecePair(StyleIdentifier StyleIdentifier, PieceIdentifier PieceIdentifier);
