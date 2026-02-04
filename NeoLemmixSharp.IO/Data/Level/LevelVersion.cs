using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Level;

[DebuggerStepThrough]
public readonly record struct LevelVersion(ulong Version)
{
    public LevelVersion Increment() => new(Version + 1);
}
