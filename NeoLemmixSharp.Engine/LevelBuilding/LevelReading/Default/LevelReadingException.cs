namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class LevelReadingException : Exception
{
    public LevelReadingException(string message)
        : base(message)
    {
    }
}