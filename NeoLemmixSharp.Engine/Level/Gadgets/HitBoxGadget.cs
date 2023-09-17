using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Common.Util.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

#pragma warning disable CS0660, CS0661, CA1067
public abstract class HitBoxGadget : GadgetBase, IIdEquatable<HitBoxGadget>, IRectangularBounds
#pragma warning restore CS0660, CS0661, CA1067
{
    public LevelPosition TopLeftPixel { get; private set; }
    public LevelPosition BottomRightPixel { get; private set; }
    public LevelPosition PreviousTopLeftPixel { get; private set; }
    public LevelPosition PreviousBottomRightPixel { get; private set; }

    protected HitBoxGadget(int id, RectangularLevelRegion gadgetBounds)
        : base(id, gadgetBounds)
    {
        TopLeftPixel = GadgetBounds.TopLeft;
        BottomRightPixel = GadgetBounds.BottomRight;

        PreviousTopLeftPixel = TopLeftPixel;
        PreviousBottomRightPixel = BottomRightPixel;
    }

    protected void UpdatePosition(LevelPosition position)
    {
        PreviousTopLeftPixel = TerrainManager.NormalisePosition(TopLeftPixel);
        PreviousBottomRightPixel = TerrainManager.NormalisePosition(BottomRightPixel);

        position = TerrainManager.NormalisePosition(position);

        GadgetBounds.X = position.X;
        GadgetBounds.Y = position.Y;

        TopLeftPixel = TerrainManager.NormalisePosition(GadgetBounds.TopLeft);
        BottomRightPixel = TerrainManager.NormalisePosition(GadgetBounds.BottomRight);

        GadgetManager.UpdateGadgetPosition(this);
    }

    public abstract bool MatchesLemming(Lemming lemming);
    public abstract bool MatchesLemmingAtPosition(Lemming lemming, LevelPosition levelPosition);
    public abstract bool MatchesPosition(LevelPosition levelPosition);

    public abstract void OnLemmingMatch(Lemming lemming);

    public bool Equals(HitBoxGadget? other) => Id == (other?.Id ?? -1);

    public static bool operator ==(HitBoxGadget left, HitBoxGadget right) => left.Id == right.Id;
    public static bool operator !=(HitBoxGadget left, HitBoxGadget right) => left.Id != right.Id;
}