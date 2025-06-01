using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Theme;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal readonly ref struct GadgetSpriteDataReader
{
    private readonly RawStyleFileDataReader _rawFileData;

    public GadgetSpriteDataReader(RawStyleFileDataReader rawFileData)
    {
        _rawFileData = rawFileData;
    }

    internal SpriteArchetypeData ReadSpriteData(int expectedNumberOfGadgetStates)
    {
        int baseWidth = _rawFileData.Read16BitUnsignedInteger();
        int baseHeight = _rawFileData.Read16BitUnsignedInteger();

        int numberOfLayers = _rawFileData.Read8BitUnsignedInteger();
        int numberOfFrames = _rawFileData.Read8BitUnsignedInteger();

        int numberOfGadgetStates = _rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfGadgetStates == expectedNumberOfGadgetStates, "Wrong number of states in animation data");

        var spriteArchetypeDataForStates = ReadGadgetStateSpriteArchetypeData(numberOfGadgetStates);

        return new SpriteArchetypeData
        {
            BaseSpriteSize = new Size(baseWidth, baseHeight),

            MaxNumberOfFrames = numberOfFrames,
            NumberOfLayers = numberOfLayers,

            SpriteArchetypeDataForStates = spriteArchetypeDataForStates
        };
    }

    private StateSpriteArchetypeData[] ReadGadgetStateSpriteArchetypeData(int numberOfGadgetStates)
    {
        var result = new StateSpriteArchetypeData[numberOfGadgetStates];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = new StateSpriteArchetypeData
            {
                AnimationData = ReadAnimationData()
            };
        }

        return result;
    }

    private AnimationLayerArchetypeData[] ReadAnimationData()
    {
        int numberOfAnimationBehaviours = _rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfAnimationBehaviours > 0, "Zero animation data defined!");

        var result = new AnimationLayerArchetypeData[numberOfAnimationBehaviours];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadAnimationBehaviourArchetypData();
        }

        return result;
    }

    private AnimationLayerArchetypeData ReadAnimationBehaviourArchetypData()
    {
        var animationLayerParameters = ReadAnimationLayerParameters();

        var nineSliceData = ReadNineSliceData();

        uint rawColorType = _rawFileData.Read8BitUnsignedInteger();
        var colorType = TribeSpriteLayerColorTypeHelpers.GetEnumValue(rawColorType);

        int initialFrame = _rawFileData.Read8BitUnsignedInteger();
        int nextGadgetState = _rawFileData.Read8BitUnsignedInteger();

        return new AnimationLayerArchetypeData
        {
            AnimationLayerParameters = animationLayerParameters,
            NineSliceData = nineSliceData,
            ColorType = colorType,
            InitialFrame = initialFrame,
            NextGadgetState = nextGadgetState - 1
        };
    }

    private AnimationLayerParameters ReadAnimationLayerParameters()
    {
        int frameStart = _rawFileData.Read8BitUnsignedInteger();
        int frameEnd = _rawFileData.Read8BitUnsignedInteger();
        int frameDelta = _rawFileData.Read8BitUnsignedInteger();
        int transitionToFrame = _rawFileData.Read8BitUnsignedInteger();

        return new AnimationLayerParameters(frameStart, frameEnd, frameDelta, transitionToFrame);
    }

    private NineSliceData ReadNineSliceData()
    {
        int nineSliceDown = _rawFileData.Read8BitUnsignedInteger();
        int nineSliceLeft = _rawFileData.Read8BitUnsignedInteger();
        int nineSliceUp = _rawFileData.Read8BitUnsignedInteger();
        int nineSliceRight = _rawFileData.Read8BitUnsignedInteger();

        return new NineSliceData(nineSliceDown, nineSliceLeft, nineSliceUp, nineSliceRight);
    }
}
