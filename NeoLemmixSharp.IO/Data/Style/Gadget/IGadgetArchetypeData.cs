namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetArchetypeData
{
    StyleIdentifier StyleIdentifier { get; }
    PieceIdentifier PieceIdentifier { get; }
    string GadgetName { get; }

    GadgetType GadgetType { get; }

    IGadgetStateArchetypeData[] GadgetStates { get; }
}
