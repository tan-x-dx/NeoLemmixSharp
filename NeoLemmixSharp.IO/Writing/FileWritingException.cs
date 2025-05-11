using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.IO.Writing;

public sealed class FileWritingException : Exception
{
    public FileWritingException(string message)
        : base(message)
    {
    }

    public static void WriterAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new FileWritingException($"Error occurred when writing level file. Details: [{details}]");
    }
}
