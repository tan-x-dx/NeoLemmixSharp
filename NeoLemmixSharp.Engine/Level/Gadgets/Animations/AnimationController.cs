using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

namespace NeoLemmixSharp.Engine.Level.Gadgets.Animations;

public sealed class AnimationController
{
    private readonly AnimationLayer[] _layers;

    private readonly GadgetBounds _currentGadgetBounds;

    public AnimationController(
        AnimationLayer[] layers,
        GadgetBounds currentGadgetBounds)
    {
        if (layers.Length < 1)
            throw new ArgumentException("Requires at least ONE animation behaviour");

        _layers = layers;
        _currentGadgetBounds = currentGadgetBounds;
    }

    public RectangularRegion CurrentBounds => _currentGadgetBounds.CurrentBounds;

    public int GetNextStateIndex()
    {
        var primaryLayer = _layers[0];
        return primaryLayer.IsEndOfAnimation
            ? primaryLayer.NextGadgetState
            : -1;
    }

    public void OnTransitionTo()
    {
        for (var i = 0; i < _layers.Length; i++)
        {
            _layers[i].OnTransitionTo();
        }
    }

    public void Tick()
    {
        for (var i = 0; i < _layers.Length; i++)
        {
            _layers[i].Tick();
        }
    }

    public void RenderLayers(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRectangle,
        Rectangle destinationRectangle)
    {
        for (var i = 0; i < _layers.Length; i++)
        {
            _layers[i].RenderLayer(spriteBatch, texture, sourceRectangle, destinationRectangle);
        }
    }
}
