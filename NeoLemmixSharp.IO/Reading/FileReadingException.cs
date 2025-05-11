using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Reading;

public sealed class FileReadingException : Exception
{
    public FileReadingException(string message)
        : base(message)
    {
    }

    public static void ReaderAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new FileReadingException($"Assertion failure occurred when reading level file. Details: [{details}]");
    }

    public static void AssertBytesMakeSense(
        int currentPosition,
        int initialPosition,
        int expectedByteCount,
        string context)
    {
        if (currentPosition - initialPosition == expectedByteCount)
            return;

        throw new FileReadingException(
            $"Wrong number of bytes read for {context}! " +
            $"Expected to read {expectedByteCount} bytes, actually read {currentPosition - initialPosition} bytes");
    }
}
