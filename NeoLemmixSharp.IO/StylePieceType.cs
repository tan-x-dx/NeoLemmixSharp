namespace NeoLemmixSharp.IO;

internal enum StylePieceType
{
    Terrain = StyleFileSectionIdentifier.TerrainArchetypeDataSection,
    Gadget = StyleFileSectionIdentifier.GadgetArchetypeDataSection
}

internal static class StylePieceTypeHelpers
{
    public static StyleFileSectionIdentifier ToSectionIdentifier(this StylePieceType stylePieceType)
    {
        return (StyleFileSectionIdentifier)stylePieceType;
    }
}