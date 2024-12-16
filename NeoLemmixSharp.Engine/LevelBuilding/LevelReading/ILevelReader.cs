using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading;

public interface ILevelReader : IDisposable
{
    LevelData ReadLevel(GraphicsDevice graphicsDevice);
}