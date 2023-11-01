using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;

namespace NeoLemmixSharp.Menu.Rendering.Pages;

public interface IPageRenderer : IDisposable
{
    void SetWindowDimensions(int windowWidth, int windowHeight);
    void SetRootWidget(Desktop desktop);
    void RenderPage(SpriteBatch spriteBatch);
}