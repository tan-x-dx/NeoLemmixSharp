using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels;

public interface ILevelReader<TReaderType> : IDisposable
    where TReaderType : ILevelReader<TReaderType>, allows ref struct
{
    static abstract TReaderType Create(string filePath);

    LevelData ReadLevel();
}
