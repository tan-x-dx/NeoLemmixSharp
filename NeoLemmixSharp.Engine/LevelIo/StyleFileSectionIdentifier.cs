using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.LevelIo.Writing;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.LevelIo;

public enum StyleFileSectionIdentifier
{
    ThemeDataSection,
    TerrainArchetypeDataSection,
    GadgetArchetypeDataSection,
}

public readonly struct StyleFileSectionIdentifierHasher :
    ISectionIdentifierHelper<StyleFileSectionIdentifier>
{
    private const int NumberOfEnumValues = 2;

    public int NumberOfItems => NumberOfEnumValues;

    [Pure]
    public int Hash(StyleFileSectionIdentifier item) => (int)item;
    [Pure]
    public StyleFileSectionIdentifier UnHash(int index) => (StyleFileSectionIdentifier)index;

    public void CreateBitBuffer(out BitBuffer32 buffer) => buffer = new();

    public static StyleFileSectionIdentifier GetEnumValue(int rawValue) => Helpers.GetEnumValue<StyleFileSectionIdentifier>(rawValue, NumberOfEnumValues);
}