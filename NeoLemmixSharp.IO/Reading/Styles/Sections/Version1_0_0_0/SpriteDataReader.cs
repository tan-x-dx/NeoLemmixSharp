using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0;

internal readonly ref struct SpriteDataReader
{
    public SpriteArchetypeData ReadSpriteData(RawStyleFileDataReader rawFileData, int expectedNumberOfGadgetStates)
    {
        int baseWidth = rawFileData.Read16BitUnsignedInteger();
        int baseHeight = rawFileData.Read16BitUnsignedInteger();

        int numberOfLayers = rawFileData.Read8BitUnsignedInteger();
        int numberOfFrames = rawFileData.Read8BitUnsignedInteger();

        int numberOfGadgetStates = rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfGadgetStates == expectedNumberOfGadgetStates, "Wrong number of states in animation data");

        var spriteArchetypeDataForStates = ReadGadgetStateSpriteArchetypeData(rawFileData, numberOfGadgetStates);

        return new SpriteArchetypeData
        {
            BaseSpriteSize = new Size(baseWidth, baseHeight),

            MaxNumberOfFrames = numberOfFrames,
            NumberOfLayers = numberOfLayers,

            SpriteArchetypeDataForStates = spriteArchetypeDataForStates
        };
    }

    private StateSpriteArchetypeData[] ReadGadgetStateSpriteArchetypeData(RawStyleFileDataReader rawFileData, int numberOfGadgetStates)
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

    private AnimationLayerArchetypeData[] ReadAnimationData(RawStyleFileDataReader rawFileData)
    {
        int numberOfAnimationBehaviours = rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfAnimationBehaviours > 0, "Zero animation data defined!");

        var result = new AnimationLayerArchetypeData[numberOfAnimationBehaviours];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadAnimationBehaviourArchetypData(rawFileData);
        }

        return result;
    }

    private AnimationLayerArchetypeData ReadAnimationBehaviourArchetypData(RawStyleFileDataReader rawFileData)
    {
        return default;

        /*   var animationLayerParameters = ReadAnimationLayerParameters(rawFileData);
           int rawColorChooser = rawFileData.Read8BitUnsignedInteger();
           var tribeColorChooser = TribeColorChooser.GetTribeColorChooser(rawColorChooser);

           int initialFrame = rawFileData.Read8BitUnsignedInteger();
           int nextGadgetState = rawFileData.Read8BitUnsignedInteger();

           return new AnimationLayerArchetypeData
           {
               AnimationLayerParameters = animationLayerParameters,

               InitialFrame = initialFrame,
               NextGadgetState = nextGadgetState - 1,

               ColorChooser = tribeColorChooser,

               NineSliceData = null!
           };*/
    }

    /* private AnimationLayerParameters ReadAnimationLayerParameters(RawStyleFileDataReader rawFileData)
     {
         int frameStart = rawFileData.Read8BitUnsignedInteger();
         int frameEnd = rawFileData.Read8BitUnsignedInteger();
         int frameDelta = rawFileData.Read8BitUnsignedInteger();
         int transitionToFrame = rawFileData.Read8BitUnsignedInteger();

         return new AnimationLayerParameters(frameStart, frameEnd, frameDelta, transitionToFrame);
     }*/
}
