using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Reading;

internal sealed class FileReadingException : Exception
{
    public FileReadingException(string message)
        : base(message)
    {
    }

    public static void ReaderAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new FileReadingException($"Assertion failure occurred when reading file. Details: [{details}]");
    }
}
