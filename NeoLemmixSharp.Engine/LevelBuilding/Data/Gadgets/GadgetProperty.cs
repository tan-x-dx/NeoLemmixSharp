﻿using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitBuffers;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets;

public enum GadgetProperty
{
    HatchGroupId,
    TeamId,
    SkillId,
    Width,
    Height,
    RawLemmingState,
    Count,
    InitialAnimationFrame,
    LogicGateType
}

public readonly struct GadgetPropertyHasher : IPerfectHasher<GadgetProperty>
{
    public int NumberOfItems => 9;

    [Pure]
    public int Hash(GadgetProperty item) => (int)item;
    [Pure]
    public GadgetProperty UnHash(int index) => (GadgetProperty)index;

    [Pure]
    public static BitArraySet<GadgetPropertyHasher, BitBuffer32, GadgetProperty> CreateBitArraySet(bool fullSet = false) => new(new GadgetPropertyHasher(), new BitBuffer32(), fullSet);
    [Pure]
    public static BitArrayDictionary<GadgetPropertyHasher, BitBuffer32, GadgetProperty, TValue> CreateBitArrayDictionary<TValue>() => new(new GadgetPropertyHasher(), new BitBuffer32());
}