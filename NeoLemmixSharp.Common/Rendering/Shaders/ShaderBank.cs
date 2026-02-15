using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Common.Rendering.Shaders;

public static class ShaderBank
{
    public static void Initialise(ContentManager contentManager)
    {
        if (TintShader is not null)
            throw new InvalidOperationException($"Cannot initialise {nameof(ShaderBank)} more than once!");

        TintShader = contentManager.Load<Effect>("shaders/TintShader");
        GreyScaleTintShader = contentManager.Load<Effect>("shaders/GreyScaleTintShader");
    }

    public static Effect TintShader { get; private set; } = null!;
    public static Effect GreyScaleTintShader { get; private set; } = null!;
}
