using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public interface IControlPanelRenderer : IDisposable
{
    void SetScreenDimensions(int screenWidth, int screenHeight);
    void RenderControlPanel(SpriteBatch spriteBatch);
}