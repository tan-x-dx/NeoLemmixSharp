using System;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Rendering.Level.Ui;

public interface IControlPanelRenderer : IDisposable
{
    void RenderControlPanel(SpriteBatch spriteBatch);
}