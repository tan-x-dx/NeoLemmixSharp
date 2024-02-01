namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.NeoLemmixCompat.LevelReading;

public sealed class CaseInvariantCharEqualityComparer : IEqualityComparer<char>
{
    public bool Equals(char x, char y)
    {
        return char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
    }

    public int GetHashCode(char obj)
    {
        return char.ToUpperInvariant(obj).GetHashCode();
    }
}