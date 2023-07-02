using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public interface IControlPanelRenderer : IDisposable
{
    void RenderControlPanel(SpriteBatch spriteBatch);
}