using NeoLemmixSharp.Common;

namespace NeoLemmixSharp.IO.Data.Style.Gadget;

public sealed class GadgetArchetypeData : IArchetypeData
{
    TextureType IArchetypeData.TextureType => TextureType.GadgetSprite;

    public required StyleIdentifier StyleIdentifier { get; init; }
    public required PieceIdentifier PieceIdentifier { get; init; }
    public required GadgetName GadgetName { get; init; }

    public required Size BaseSpriteSize { get; init; }

    public required IGadgetArchetypeSpecificationData SpecificationData { get; init; }
    string IArchetypeData.Name => GadgetName.ToString();
    public required string TextureFilePath { get; init; }
    RectangularRegion IArchetypeData.NineSliceData => throw new NotImplementedException();
    ResizeType IArchetypeData.ResizeType => throw new NotImplementedException();
    Size IArchetypeData.DefaultSize => BaseSpriteSize;
}
