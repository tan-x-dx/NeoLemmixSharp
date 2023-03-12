using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NeoLemmixSharp.Rendering;
using NeoLemmixSharp.Screen;

namespace NeoLemmixSharp.Engine;

public sealed class LevelScreen : BaseScreen
{
    public static LevelScreen? CurrentLevel { get; private set; }

    private readonly ITickable[] _levelObjects;
    private readonly IRenderable[] _levelSprites;

    public NeoLemmixViewPort Viewport { get; }

    public LevelScreen()
    {
        CurrentLevel = this;
        Viewport = new NeoLemmixViewPort();

        _levelObjects = GetLevelObjects();
        _levelSprites = GetLevelSprites();
    }

    private ITickable[] GetLevelObjects()
    {
        var result = new ITickable[1];

        return result;
    }

    private IRenderable[] GetLevelSprites()
    {
        var result = new IRenderable[1];

        return result;
    }

    public override void Tick(MouseState mouseState)
    {
        Viewport.Tick(mouseState);

        for (var i = 0; i < _levelObjects.Length; i++)
        {
            if (_levelObjects[i].ShouldTick)
            {
                _levelObjects[i].Tick(mouseState);
            }
        }
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < _levelSprites.Length; i++)
        {
            if (_levelSprites[i].ShouldRender)
            {
                _levelSprites[i].Render(spriteBatch);
            }
        }
    }

    public override void Dispose()
    {
    }
}