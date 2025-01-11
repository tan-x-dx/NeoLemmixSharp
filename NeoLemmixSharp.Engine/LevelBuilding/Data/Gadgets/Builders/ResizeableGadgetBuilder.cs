﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.LemmingFiltering;
using NeoLemmixSharp.Engine.Level.Lemmings;
using NeoLemmixSharp.Engine.Level.Orientations;
using NeoLemmixSharp.Engine.Level.Teams;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Sprites;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;

public sealed class ResizeableGadgetBuilder : IGadgetBuilder
{
    public required int GadgetBuilderId { get; init; }
    public required GadgetStateArchetypeData ArchetypeData { get; init; }

    public required SpriteData SpriteData { get; init; }

    public GadgetBase BuildGadget(
        GadgetSpriteBuilder gadgetSpriteBuilder,
        GadgetData gadgetData,
        LemmingManager lemmingHasher)
    {
        var gadgetWidth = gadgetData.GetProperty(GadgetProperty.Width);
        var gadgetHeight = gadgetData.GetProperty(GadgetProperty.Height);

        var gadgetBounds = new RectangularHitBoxRegion(
            gadgetData.X,
            gadgetData.Y,
            gadgetWidth,
            gadgetHeight);

        var lemmingTracker = new LemmingTracker(lemmingHasher);

        var gadgetRenderer = gadgetSpriteBuilder.BuildResizeableGadgetRenderer(this, gadgetData);

        var hitBox = CreateHitBox(gadgetData, gadgetBounds);

        var result = new HitBoxGadget(
            gadgetData.Id,
            gadgetData.Orientation,
            lemmingTracker,
            []);

        gadgetRenderer?.SetGadget(result);

        return result;
    }

    private HitBox CreateHitBox(
        GadgetData gadgetData,
        RectangularHitBoxRegion gadgetBounds)
    {
        var triggerData = ArchetypeData.TriggerData;
        if (triggerData.Length == 0)
            return HitBox.CreateEmptyHitBox(new LevelPosition(gadgetData.X, gadgetData.Y));

        var hitBoxRegion = CreateRectangularHitBoxLevelRegion(gadgetData, triggerData, gadgetBounds);

        var lemmingFilters = new List<ILemmingCriterion>();

        if (ArchetypeData.AllowedActions is not null)
        {
            var actionFilter = new LemmingActionCriterion();
            actionFilter.RegisterActions(ArchetypeData.AllowedActions);
            lemmingFilters.Add(actionFilter);
        }

        if (ArchetypeData.AllowedStates is not null)
        {
            var states = ArchetypeData.AllowedStates.ToArray();
            var stateFilter = new LemmingStateCriterion(states);
            lemmingFilters.Add(stateFilter);
        }

        if (ArchetypeData.AllowedOrientations is not null)
        {
            var orientationFilter = new LemmingOrientationFilter();
            var gadgetOrientationId = gadgetData.Orientation.RotNum;

            foreach (var orientation in ArchetypeData.AllowedOrientations)
            {
                var rotatedOrientationId = (orientation.RotNum + gadgetOrientationId) & 3;
                orientationFilter.RegisterOrientation(Orientation.AllItems[rotatedOrientationId]);
            }

            lemmingFilters.Add(orientationFilter);
        }

        if (ArchetypeData.AllowedFacingDirection is not null)
        {
            var facingDirectionFilter = new LemmingFacingDirectionCriterion(ArchetypeData.AllowedFacingDirection);
            lemmingFilters.Add(facingDirectionFilter);
        }

        if (gadgetData.TryGetProperty(GadgetProperty.TeamId, out var teamId))
        {
            var team = Team.AllItems[teamId];
            var teamFilter = new LemmingTeamCriterion(team);
            lemmingFilters.Add(teamFilter);
        }

        var hitBox = new HitBox(
            hitBoxRegion,
            lemmingFilters.ToArray());

        return hitBox;
    }

    private RectangularHitBoxRegion CreateRectangularHitBoxLevelRegion(
        GadgetData gadgetData,
        ReadOnlySpan<LevelPosition> triggerData,
        RectangularHitBoxRegion gadgetBounds)
    {
        if (triggerData.Length != 2)
            throw new InvalidOperationException("Expected exactly two points of data");

        gadgetData.GetDihedralTransformation(out var dihedralTransformation);

        var p0 = triggerData[0];
        var p1 = triggerData[1];

        dihedralTransformation.Transform(
            p0.X,
            p0.Y,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            out var tX,
            out var tY);

        p0 = new LevelPosition(tX, tY);

        dihedralTransformation.Transform(
            p1.X,
            p1.Y,
            SpriteData.SpriteWidth,
            SpriteData.SpriteHeight,
            out tX,
            out tY);

        p1 = new LevelPosition(tX, tY);

        return new RectangularHitBoxRegion(p0.X, p0.Y, p1.X, p1.Y);
    }
}