using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Writing;

internal sealed class FileWritingException : Exception
{
    public FileWritingException(string message)
        : base(message)
    {
    }

    public static void WriterAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new FileWritingException($"Assertion failure occurred when writing file. Details: [{details}]");
    }
}
