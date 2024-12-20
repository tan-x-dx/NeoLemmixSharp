using Microsoft.Xna.Framework;

namespace NeoLemmixSharp.Common.Util;

public static class Helpers
{
    public static LevelPosition TopLeftLevelPosition(this Rectangle rectangle) => new(rectangle.X, rectangle.Y);
    public static LevelPosition BottomLeftLevelPosition(this Rectangle rectangle) => new(rectangle.X, rectangle.Y + rectangle.Height - 1);
    public static LevelPosition TopRightLevelPosition(this Rectangle rectangle) => new(rectangle.X + rectangle.Width - 1, rectangle.Y);
    public static LevelPosition BottomRightLevelPosition(this Rectangle rectangle) => new(rectangle.X + rectangle.Width - 1, rectangle.Y + rectangle.Height - 1);

    public static IEqualityComparer<char> CaseInvariantCharEqualityComparer { get; } = new CharEqualityComparer();

    private sealed class CharEqualityComparer : IEqualityComparer<char>
    {
        bool IEqualityComparer<char>.Equals(char x, char y)
        {
            return char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
        }

        int IEqualityComparer<char>.GetHashCode(char obj)
        {
            return char.ToUpperInvariant(obj).GetHashCode();
        }
    }
}