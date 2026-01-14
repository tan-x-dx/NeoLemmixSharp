using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Reading;

internal sealed class FileReadingException(string message) : Exception(message)
{
    public static void ReaderAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        AssertionFailure(details);
    }

    [DoesNotReturn]
    private static void AssertionFailure(string details)
    {
        throw new FileReadingException($"Assertion failure occurred when reading file. Details: [{details}]");
    }
}
