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

        throw new LevelReadingException($"Error occurred when reading level file. Details: [{details}]");
    }
}