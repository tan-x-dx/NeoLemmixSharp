namespace NeoLemmixSharp.Engine.LevelIo;

public enum StylePieceType
{
    Terrain = StyleFileSectionIdentifier.TerrainArchetypeDataSection,
    Gadget = StyleFileSectionIdentifier.GadgetArchetypeDataSection
}

public static class StylePieceTypeHelpers
{
    public static StyleFileSectionIdentifier ToSectionIdentifier(this StylePieceType stylePieceType)
    {
        return (StyleFileSectionIdentifier)stylePieceType;
    }
}