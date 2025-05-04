using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default;

public sealed class LevelReadingException : Exception
{
    public LevelReadingException(string message)
        : base(message)
    {
    }

    public static void ReaderAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new LevelReadingException($"Assertion failure occurred when reading level file. Details: [{details}]");
    }

    public static void AssertBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount,
        string context)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new LevelReadingException(
            $"Wrong number of bytes read for {context}! " +
            $"Expected to read {expectedByteCount} bytes, actually read {currentPosition - initialPosition} bytes");
    }
}
