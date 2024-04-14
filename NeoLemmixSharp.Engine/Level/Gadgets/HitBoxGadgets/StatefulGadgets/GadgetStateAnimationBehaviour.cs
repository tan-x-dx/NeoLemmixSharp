using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;

public sealed class GadgetStateAnimationBehaviour
{
    private readonly int _sourceDx;
    private readonly int _spriteHeight;
    public GadgetSecondaryAnimationAction SecondaryAnimationAction { get; }
    public int MinFrame { get; }
    public int MaxFrame { get; }

    public int CurrentFrame { get; set; }

    public Color Color { get; set; } = Color.White;

    public GadgetStateAnimationBehaviour(
         int spriteWidth,
         int spriteHeight,
         int layer,
         int initialFrame,
         int minFrame,
         int maxFrame,
         GadgetSecondaryAnimationAction secondaryAnimationAction)
    {
        _sourceDx = spriteWidth * layer;
        _spriteHeight = spriteHeight;
        MinFrame = minFrame;
        MaxFrame = maxFrame;
        SecondaryAnimationAction = secondaryAnimationAction;

        CurrentFrame = initialFrame;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        sourceRectangle.X += _sourceDx;
        sourceRectangle.Y += CurrentFrame * _spriteHeight;

        spriteBatch.Draw(
            texture,
            destinationRectangle,
            sourceRectangle,
            Color,
            1.0f);
    }
}