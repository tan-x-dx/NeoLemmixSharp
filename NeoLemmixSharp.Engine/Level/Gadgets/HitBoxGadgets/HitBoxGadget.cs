using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Identity;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Gadgets.GadgetTypes;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;

#pragma warning disable CS0660, CS0661, CA1067
public abstract class HitBoxGadget : GadgetBase, IIdEquatable<HitBoxGadget>, IRectangularBounds
#pragma warning restore CS0660, CS0661, CA1067
{
    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    public abstract override InteractiveGadgetType Type { get; }

    protected HitBoxGadget(int id, RectangularLevelRegion gadgetBounds)
        : base(id, gadgetBounds)
    {
        var topLeft = GadgetBounds.TopLeft;
        var bottomRight = GadgetBounds.BottomRight;

        TopLeftPixel = topLeft;
        BottomRightPixel = bottomRight;

        PreviousTopLeftPixel = topLeft;
        PreviousBottomRightPixel = bottomRight;
    }

    protected void UpdatePosition(LevelPosition position)
    {
        var terrainManager = Global.TerrainManager;

        PreviousTopLeftPixel = terrainManager.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = terrainManager.NormalisePosition(BottomRightPixel);

        position = terrainManager.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = terrainManager.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = terrainManager.NormalisePosition(GadgetBounds.BottomRight);

        Global.GadgetManager.UpdateGadgetPosition(this);
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

    public abstract void OnLemmingMatch(Lemming lemming);

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}