﻿global using GadgetSet = NeoLemmixSharp.Common.Util.Collections.SimpleSetEnumerable<NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxGadget>;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Rendering;
using NeoLemmixSharp.Engine.Rendering.Viewport.GadgetRendering;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

#pragma warning disable CS0660, CS0661, CA1067
public abstract class HitBoxGadget : GadgetBase, IIdEquatable<HitBoxGadget>
#pragma warning restore CS0660, CS0661, CA1067
{
    protected readonly ItemTracker<Lemming> LemmingTracker;

    protected HitBoxGadget(
        int id,
        RectangularLevelRegion gadgetBounds,
        IGadgetRenderer? renderer,
        ItemTracker<Lemming> lemmingTracker)
        : base(id, gadgetBounds, renderer)
    {
        LemmingTracker = lemmingTracker;

        var topLeft = GadgetBounds.TopLeft;
        var bottomRight = GadgetBounds.BottomRight;
        TopLeftPixel = topLeft;
        BottomRightPixel = bottomRight;
        PreviousTopLeftPixel = topLeft;
        PreviousBottomRightPixel = bottomRight;
    }

    protected void UpdatePosition(LevelPosition position)
    {
        var terrainManager = LevelScreen.TerrainManager;

        PreviousTopLeftPixel = terrainManager.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = terrainManager.NormalisePosition(BottomRightPixel);

        position = terrainManager.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = terrainManager.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = terrainManager.NormalisePosition(GadgetBounds.BottomRight);

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);

        if (Renderer is not null)
        {
            LevelScreenRenderer.Instance.LevelRenderer.UpdateSpritePosition(Renderer);
        }
    }

    protected void UpdateSize(LevelPosition size)
    {
        var terrainManager = LevelScreen.TerrainManager;

        PreviousBottomRightPixel = terrainManager.NormalisePosition(BottomRightPixel);

        GadgetBounds.W = size.X;
        GadgetBounds.H = size.Y;

        BottomRightPixel = terrainManager.NormalisePosition(GadgetBounds.BottomRight);

        LevelScreen.GadgetManager.UpdateGadgetPosition(this);
    }

    public bool MatchesLemming(Lemming lemming)
    {
        var anchorPosition = lemming.LevelPosition;
        var footPosition = lemming.FootPosition;

        return MatchesLemmingAtPosition(lemming, anchorPosition) ||
               MatchesLemmingAtPosition(lemming, footPosition);
    }

    public abstract bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition);
    public abstract bool MatchesPosition(LevelPosition levelPosition);
    public abstract bool IsSolidToLemmingAtPosition(Lemming lemming, LevelPosition levelPosition);
    public abstract bool IsSteelToLemmingAtPosition(Lemming lemming, LevelPosition levelPosition);

    public abstract void OnLemmingMatch(Lemming lemming);

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}