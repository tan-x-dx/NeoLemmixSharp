using NeoLemmixSharp.Engine.LemmingActions;
using NeoLemmixSharp.Rendering.LevelRendering;
using System.Collections.Generic;

namespace NeoLemmixSharp.Rendering.Foo;

public sealed class LemmingSpriteSet
{
    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup = new();

    public LemmingSpriteSet()
    {

    }

    public LemmingActionSpriteBundle GetActionSpriteBundle(LemmingAction lemmingAction)
    {
        return _actionSpriteBundleLookup[lemmingAction.LemmingActionName];
    }
}



public class FooBar
{
    void Abc()
    {
        var defaultSpriteData = new List<DataThing>
        {
            new DataThing(ClimberAction.Instance, "lemmingSprites/climber.png", 3, 8, 10, 3, 6),
        };
    }
}

public class DataThing
{
    public DataThing(
        LemmingAction lemmingAction,
        string resourceName,
        int spriteLayerCount,
        int spriteWidth,
        int spriteHeight,
        int anchorPointX,
        int anchorPointY)
    {
        var numberOfFrames = lemmingAction.NumberOfAnimationFrames;
    }
}