using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data;

[DebuggerStepThrough]
[DebuggerDisplay("{StyleIdentifier}:{PieceIdentifier}")]
public readonly record struct StylePiecePair(StyleIdentifier StyleIdentifier, PieceIdentifier PieceIdentifier);
