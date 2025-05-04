using System.Diagnostics.CodeAnalysis;

namespace NeoLemmixSharp.Engine.LevelIo.LevelWriting;

public sealed class LevelWritingException : Exception
{
    public LevelWritingException(string message)
        : base(message)
    {
    }

    public static void WriterAssert([DoesNotReturnIf(false)] bool condition, string details)
    {
        if (condition)
            return;

        throw new LevelWritingException($"Error occurred when writing level file. Details: [{details}]");
    }
}
