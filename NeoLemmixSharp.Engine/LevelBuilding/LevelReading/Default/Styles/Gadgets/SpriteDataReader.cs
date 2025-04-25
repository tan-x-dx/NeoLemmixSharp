using NeoLemmixSharp.Common;
using NeoLemmixSharp.Engine.Level.Gadgets.Animations;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;

public readonly ref struct SpriteDataReader
{
    public SpriteArchetypeData ReadSpriteData(RawFileData rawFileData, int expectedNumberOfGadgetStates)
    {
        int baseWidth = rawFileData.Read16BitUnsignedInteger();
        int baseHeight = rawFileData.Read16BitUnsignedInteger();

        int numberOfLayers = rawFileData.Read8BitUnsignedInteger();
        int numberOfFrames = rawFileData.Read8BitUnsignedInteger();

        int numberOfGadgetStates = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(numberOfGadgetStates == expectedNumberOfGadgetStates, "Wrong number of states in animation data");

        var spriteArchetypeDataForStates = ReadGadgetStateSpriteArchetypeData(rawFileData, numberOfGadgetStates);

        return new SpriteArchetypeData
        {
            BaseSpriteSize = new Size(baseWidth, baseHeight),

            MaxNumberOfFrames = numberOfFrames,
            NumberOfLayers = numberOfLayers,

            SpriteArchetypeDataForStates = spriteArchetypeDataForStates
        };
    }

    private StateSpriteArchetypeData[] ReadGadgetStateSpriteArchetypeData(RawFileData rawFileData, int numberOfGadgetStates)
    {
        var result = new StateSpriteArchetypeData[numberOfGadgetStates];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = new StateSpriteArchetypeData
            {
                AnimationData = ReadAnimationData(rawFileData)
            };
        }

        return result;
    }

    private AnimationLayerArchetypeData[] ReadAnimationData(RawFileData rawFileData)
    {
        int numberOfAnimationBehaviours = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(numberOfAnimationBehaviours > 0, "Zero animation data defined!");

        var result = new AnimationLayerArchetypeData[numberOfAnimationBehaviours];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadAnimationBehaviourArchetypData(rawFileData);
        }

        return result;
    }

    private AnimationLayerArchetypeData ReadAnimationBehaviourArchetypData(RawFileData rawFileData)
    {
        var animationLayerParameters = ReadAnimationLayerParameters(rawFileData);
        int rawColorChooser = rawFileData.Read8BitUnsignedInteger();
        var teamColorChooser = TeamColors.GetTeamColorChooser(rawColorChooser);

        int initialFrame = rawFileData.Read8BitUnsignedInteger();
        int nextGadgetState = rawFileData.Read8BitUnsignedInteger();

        return new AnimationLayerArchetypeData
        {
            AnimationLayerParameters = animationLayerParameters,

            InitialFrame = initialFrame,
            NextGadgetState = nextGadgetState - 1,

            ColorChooser = teamColorChooser,

            NineSliceData = null!
        };
    }

    private AnimationLayerParameters ReadAnimationLayerParameters(RawFileData rawFileData)
    {
        int frameStart = rawFileData.Read8BitUnsignedInteger();
        int frameEnd = rawFileData.Read8BitUnsignedInteger();
        int frameDelta = rawFileData.Read8BitUnsignedInteger();
        int transitionToFrame = rawFileData.Read8BitUnsignedInteger();

        return new AnimationLayerParameters(frameStart, frameEnd, frameDelta, transitionToFrame);
    }
}
