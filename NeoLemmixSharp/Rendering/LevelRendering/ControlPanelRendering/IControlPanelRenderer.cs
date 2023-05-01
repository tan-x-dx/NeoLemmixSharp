using Microsoft.Xna.Framework.Graphics;
using System;

namespace NeoLemmixSharp.Rendering.LevelRendering.ControlPanelRendering;

public interface IControlPanelRenderer : IDisposable
{
    void RenderControlPanel(SpriteBatch spriteBatch);
}