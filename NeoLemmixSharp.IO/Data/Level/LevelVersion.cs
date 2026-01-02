namespace NeoLemmixSharp.IO.Data.Level;

public readonly record struct LevelVersion(ulong Version)
{
    public LevelVersion Increment() => new(Version + 1);
}
