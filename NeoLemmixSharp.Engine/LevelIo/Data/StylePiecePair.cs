using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.Data;

public readonly struct StylePiecePair(string styleName, string pieceName)
{
    public readonly string StyleName = styleName;
    public readonly string PieceName = pieceName;
}

public sealed class StylePiecePairEqualityComparer : IEqualityComparer<StylePiecePair>
{
    public bool Equals(StylePiecePair x, StylePiecePair y)
    {
        return string.Equals(x.StyleName, y.StyleName) &&
               string.Equals(x.PieceName, y.PieceName);
    }

    public int GetHashCode([DisallowNull] StylePiecePair obj)
    {
        return HashCode.Combine(obj.StyleName, obj.PieceName);
    }
}