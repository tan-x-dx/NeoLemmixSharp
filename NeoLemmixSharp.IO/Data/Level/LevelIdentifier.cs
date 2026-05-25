using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Level;

[DebuggerStepThrough]
public readonly record struct LevelIdentifier(ulong LevelId)
{
    public static unsafe LevelIdentifier GenerateRandomLevelIdentifier()
    {
        ulong val = 0;
        var span = new Span<byte>(&val, 8);

        Random.Shared.NextBytes(span);

        return new LevelIdentifier(val);
    }
}
