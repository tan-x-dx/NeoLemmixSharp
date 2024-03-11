using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class StatefulGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }
    public required GadgetBehaviour GadgetBehaviour { get; init; }
    public required GadgetStateData[] AllGadgetStateData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher)
    {
        var bounds = new RectangularLevelRegion(gadgetData.X, gadgetData.Y, SpriteData.SpriteWidth, SpriteData.SpriteHeight);
        var gadgetStates = CreateStates(gadgetData);
        var gadgetRenderer = gadgetSpriteBuilder.BuildGadgetRenderer(this, gadgetData);

        return new StatefulGadget(
            gadgetData.Id,
            GadgetBehaviour,
            gadgetData.Orientation,
            bounds,
            gadgetRenderer,
            gadgetStates,
            new ItemTracker<Lemming>(lemmingHasher));
    }

    private GadgetState[] CreateStates(GadgetData gadgetData)
    {
        var result = new GadgetState[AllGadgetStateData.Length];

        for (var i = AllGadgetStateData.Length - 1; i >= 0; i--)
        {
            result[i] = CreateGadgetState(gadgetData, AllGadgetStateData[i]);
        }

        return result;
    }

    private GadgetState CreateGadgetState(GadgetData gadgetData, GadgetStateData gadgetStateData)
    {
        var hitBoxRegion = CreateHitBoxLevelRegion(gadgetData, gadgetStateData);

        var lemmingFilters = Array.Empty<ILemmingFilter>();

        var hitBox = new HitBox(
            hitBoxRegion,
            lemmingFilters);

        return new GadgetState(
            gadgetStateData.NumberOfFrames,
            gadgetStateData.OnLemmingEnterActions,
            gadgetStateData.OnLemmingPresentActions,
            gadgetStateData.OnLemmingExitActions,
            hitBox,
            0);
    }

    private ILevelRegion CreateHitBoxLevelRegion(GadgetData gadgetData, GadgetStateData gadgetStateData)
    {
        var nullableRectangularTriggerData = gadgetStateData.TriggerData;
        if (!nullableRectangularTriggerData.HasValue)
            return EmptyLevelRegion.Instance;

        return CreateRectangularHitBoxLevelRegion(nullableRectangularTriggerData.Value);

        RectangularLevelRegion CreateRectangularHitBoxLevelRegion(RectangularTriggerData triggerData)
        {
            gadgetData.GetDihedralTransformation(out var dihedralTransformation);

            dihedralTransformation.Transform(
                triggerData.TriggerX,
                triggerData.TriggerY,
                SpriteData.SpriteWidth,
                SpriteData.SpriteHeight,
                out var tX,
                out var tY);

            var p0 = new LevelPosition(tX, tY);

            dihedralTransformation.Transform(
                triggerData.TriggerX + triggerData.TriggerWidth - 1,
                triggerData.TriggerY + triggerData.TriggerHeight - 1,
                SpriteData.SpriteWidth,
                SpriteData.SpriteHeight,
                out tX,
                out tY);

            var p1 = new LevelPosition(tX, tY);

            return new RectangularLevelRegion(p0, p1);
        }
    }
}