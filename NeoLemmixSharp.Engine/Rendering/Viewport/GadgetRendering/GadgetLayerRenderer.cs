using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Rendering;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

public sealed class GadgetLayerRenderer
{
    private readonly Texture2D _texture;
    private readonly GadgetStateAnimationBehaviour _stateAnimationBehaviour;
    private readonly Color _renderColor;

    public GadgetLayerRenderer(
        Texture2D texture,
        GadgetStateAnimationBehaviour stateAnimationBehaviour,
        Color renderColor)
    {
        _texture = texture;
        _stateAnimationBehaviour = stateAnimationBehaviour;
        _renderColor = renderColor;
    }

    public void RenderLayer(
        SpriteBatch spriteBatch,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        _stateAnimationBehaviour.GetFrameData(out var spriteFrameAndLayerData);

        sourceRectangle.X += spriteFrameAndLayerData.SourceDx;
        sourceRectangle.Y += spriteFrameAndLayerData.SourceDy;

        spriteBatch.Draw(
            _texture,
            destinationRectangle,
            sourceRectangle,
            _renderColor,
            1.0f);
    }
}