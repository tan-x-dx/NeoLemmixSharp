﻿global using ControlPanelParameterSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.ControlPanel.ControlPanelParameterHasher, NeoLemmixSharp.Common.Util.Collections.BitBuffer32, NeoLemmixSharp.Engine.Level.ControlPanel.ControlPanelParameters>;
global using GadgetEnumerable = NeoLemmixSharp.Common.Util.Collections.BitArrayEnumerable<NeoLemmixSharp.Engine.Level.Gadgets.GadgetManager, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxGadget>;
global using GadgetSpacialHashGrid = NeoLemmixSharp.Common.Util.PositionTracking.SpacialHashGrid<NeoLemmixSharp.Engine.Level.Gadgets.GadgetManager, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxGadget>;
global using LemmingActionSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.LemmingActions.LemmingAction.LemmingActionHasher, NeoLemmixSharp.Engine.Level.LemmingActions.LemmingAction.LemmingActionBitBuffer, NeoLemmixSharp.Engine.Level.LemmingActions.LemmingAction>;
global using LemmingEnumerable = NeoLemmixSharp.Common.Util.Collections.BitArrayEnumerable<NeoLemmixSharp.Engine.Level.Lemmings.LemmingManager, NeoLemmixSharp.Engine.Level.Lemmings.Lemming>;
global using LemmingSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.Lemmings.LemmingManager, NeoLemmixSharp.Common.Util.Collections.ArrayBitBuffer, NeoLemmixSharp.Engine.Level.Lemmings.Lemming>;
global using LemmingSkillSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.Skills.LemmingSkill.LemmingSkillHasher, NeoLemmixSharp.Engine.Level.Skills.LemmingSkill.LemmingSkillBitBuffer, NeoLemmixSharp.Engine.Level.Skills.LemmingSkill>;
global using LemmingSpacialHashGrid = NeoLemmixSharp.Common.Util.PositionTracking.SpacialHashGrid<NeoLemmixSharp.Engine.Level.Lemmings.LemmingManager, NeoLemmixSharp.Engine.Level.Lemmings.Lemming>;
global using LemmingTracker = NeoLemmixSharp.Common.Util.Collections.ItemTracker<NeoLemmixSharp.Engine.Level.Lemmings.LemmingManager, NeoLemmixSharp.Engine.Level.Lemmings.Lemming>;
global using LevelParameterSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.LevelParameterHasher, NeoLemmixSharp.Common.Util.Collections.BitBuffer32, NeoLemmixSharp.Engine.Level.LevelParameters>;
global using OrientationSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.Orientations.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitBuffer32, NeoLemmixSharp.Engine.Level.Orientations.Orientation>;
global using OrientationToHitBoxRegionLookup = NeoLemmixSharp.Common.Util.Collections.BitArrayDictionary<NeoLemmixSharp.Engine.Level.Orientations.Orientation.OrientationHasher, NeoLemmixSharp.Common.Util.Collections.BitBuffer32, NeoLemmixSharp.Engine.Level.Orientations.Orientation, NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes.IHitBoxRegion>;
global using SpriteSpacialHashGrid = NeoLemmixSharp.Common.Util.PositionTracking.SpacialHashGrid<NeoLemmixSharp.Engine.Rendering.LevelRenderer, NeoLemmixSharp.Engine.Rendering.Viewport.IViewportObjectRenderer>;
global using StateChangerSet = NeoLemmixSharp.Common.Util.Collections.BitArraySet<NeoLemmixSharp.Engine.Level.Skills.LemmingStateChangerHasher, NeoLemmixSharp.Common.Util.Collections.BitBuffer32, NeoLemmixSharp.Engine.Level.Skills.ILemmingStateChanger>;
