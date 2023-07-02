using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Level.Ui;

public interface IControlPanelRenderer : IDisposable
{
    void RenderControlPanel(SpriteBatch spriteBatch);
}