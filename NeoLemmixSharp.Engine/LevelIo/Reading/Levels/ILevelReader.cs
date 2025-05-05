using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelIo.Data;

namespace NeoLemmixSharp.Engine.LevelIo.Reading.Levels;

public interface ILevelReader : IDisposable
{
    LevelData ReadLevel(GraphicsDevice graphicsDevice);
}