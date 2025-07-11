using NeoLemmixSharp.IO.FileFormats;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Data;

[DebuggerDisplay("{StyleIdentifier} - {FileFormatType}")]
public readonly struct StyleFormatPair(StyleIdentifier styleIdentifier, FileFormatType fileFormatType) : IEquatable<StyleFormatPair>
{
    public readonly StyleIdentifier StyleIdentifier = styleIdentifier;
    public readonly FileFormatType FileFormatType = fileFormatType;

    public bool Equals(StyleFormatPair other) => FileFormatType == other.FileFormatType &&
                                                 StyleIdentifier.Equals(other.StyleIdentifier);
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is StyleFormatPair other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(StyleIdentifier, FileFormatType);
    public static bool operator ==(StyleFormatPair left, StyleFormatPair right) => left.Equals(right);
    public static bool operator !=(StyleFormatPair left, StyleFormatPair right) => !left.Equals(right);
}
