using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.Behaviours;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Gadgets.Interactions;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;
using NeoLemmixSharp.Engine.Level.Lemmings;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class StatefulGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }
    public required GadgetBehaviour GadgetBehaviour { get; init; }
    public required GadgetStateData[] AllGadgetStateData { get; init; }

    public required Texture2D Sprite { get; init; }

    public required int GadgetWidth { get; init; }
    public required int GadgetHeight { get; init; }

    public GadgetBase BuildGadget(
        GadgetData gadgetData,
        IPerfectHasher<Lemming> lemmingHasher)
    {
        var bounds = new RectangularLevelRegion(gadgetData.X, gadgetData.Y, GadgetWidth, GadgetHeight);
        var gadgetStates = CreateStates(gadgetData);

        return new StatefulGadget(
            gadgetData.Id,
            GadgetBehaviour,
            gadgetData.Orientation,
            bounds,
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
        var hitBoxRegion = CreateHitBoxRectangularLevelRegion(gadgetData, gadgetStateData);

        var lemmingFilters = Array.Empty<ILemmingFilter>();

        var hitBox = new HitBox(
            hitBoxRegion,
            lemmingFilters);

        var output = new GadgetOutput();

        return new GadgetState(
            gadgetStateData.NumberOfFrames,
            gadgetStateData.OnLemmingEnterActions,
            gadgetStateData.OnLemmingPresentActions,
            gadgetStateData.OnLemmingExitActions,
            hitBox,
            output,
            0);
    }

    private RectangularLevelRegion CreateHitBoxRectangularLevelRegion(GadgetData gadgetData, GadgetStateData gadgetStateData)
    {
        gadgetData.GetDihedralTransformation(out var dihedralTransformation);

        dihedralTransformation.Transform(
            gadgetStateData.TriggerX,
            gadgetStateData.TriggerY,
            GadgetWidth,
            GadgetHeight,
            out var tX,
            out var tY);

        var p0 = new LevelPosition(tX, tY);

        dihedralTransformation.Transform(
            gadgetStateData.TriggerX + gadgetStateData.TriggerWidth - 1,
            gadgetStateData.TriggerY + gadgetStateData.TriggerHeight - 1,
            GadgetWidth,
            GadgetHeight,
            out tX,
            out tY);

        var p1 = new LevelPosition(tX, tY);

        var hitBoxRegion = new RectangularLevelRegion(p0, p1);
        return hitBoxRegion;
    }
}