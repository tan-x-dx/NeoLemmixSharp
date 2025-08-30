namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public interface IGadgetArchetypeData
{
    StyleIdentifier StyleIdentifier { get; }
    PieceIdentifier PieceIdentifier { get; }
    GadgetName GadgetName { get; }

    GadgetType GadgetType { get; }

    IGadgetStateArchetypeData[] GadgetStates { get; }
}
