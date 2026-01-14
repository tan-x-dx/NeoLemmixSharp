using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Writing;

internal sealed class FileWritingException(string message) : Exception(message)
{
    public static void WriterAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        AssertionFailure(details);
    }

    [DoesNotReturn]
    private static void AssertionFailure(string details)
    {
        throw new FileWritingException($"Assertion failure occurred when writing file. Details: [{details}]");
    }
}
