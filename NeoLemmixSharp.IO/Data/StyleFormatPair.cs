using NeoLemmixSharp.IO.FileFormats;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data;

[DebuggerStepThrough]
[DebuggerDisplay("{StyleIdentifier} - {FileFormatType}")]
public readonly record struct StyleFormatPair(StyleIdentifier StyleIdentifier, FileFormatType FileFormatType);
