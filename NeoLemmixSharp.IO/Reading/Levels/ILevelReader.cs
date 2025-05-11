using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.IO.Data.Level;

namespace NeoLemmixSharp.IO.Reading.Levels;

public interface ILevelReader : IDisposable
{
    LevelData ReadLevel(GraphicsDevice graphicsDevice);
}