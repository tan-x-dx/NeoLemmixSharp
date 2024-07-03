using System.Text;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default;

public sealed class DefaultLevelReader : ILevelReader
{
    public LevelData ReadLevel(string levelFilePath, GraphicsDevice graphicsDevice)
    {
        using var dataReaderList = new DataReaderList(levelFilePath);

        return dataReaderList.ReadFile();
    }

    public string ScrapeLevelTitle(string levelFilePath)
    {
        var fileStream = new FileStream(levelFilePath, FileMode.Open);
        var reader = new BinaryReader(fileStream);

        fileStream.Position = 0x11;

        var stringLength = reader.ReadUInt16();

        Span<byte> byteBuffer = stringLength > 80
            ? new byte[stringLength]
            : stackalloc byte[stringLength];

        _ = reader.Read(byteBuffer);

        var encoding = Encoding.UTF8;

        return encoding.GetString(byteBuffer);
    }

    public void Dispose()
    {
    }
}