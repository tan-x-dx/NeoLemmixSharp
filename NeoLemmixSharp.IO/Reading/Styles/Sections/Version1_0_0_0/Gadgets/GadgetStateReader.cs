using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal readonly ref struct GadgetStateReader
{
    private readonly RawStyleFileDataReader _reader;
    private readonly FileReaderStringIdLookup _stringIdLookup;

    internal GadgetStateReader(RawStyleFileDataReader reader, FileReaderStringIdLookup stringIdLookup)
    {
        _reader = reader;
        _stringIdLookup = stringIdLookup;
    }

    internal GadgetStateArchetypeData ReadStateData()
    {
        int stateNameId = _reader.Read16BitUnsignedInteger();

        var rawPointData = _reader.Read32BitSignedInteger();
        var hitBoxOffset = ReadWriteHelpers.DecodePoint(rawPointData);

        //var hitBoxData = ReadHitBoxData();
        var regionData = ReadRegionData();

        var animationLayerData = ReadAnimationLayerArchetypeData();

        var result = new GadgetStateArchetypeData
        {
            StateName = new GadgetStateName(_stringIdLookup[stateNameId]),
            // HitBoxOffset = hitBoxOffset,
            //  HitBoxData = hitBoxData,
            RegionData = regionData,

            AnimationLayerData = animationLayerData,
        };

        return result;
    }
    /*
    private HitBoxData[] ReadHitBoxData()
    {
        int numberOfDefinedHitBoxes = _reader.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfDefinedHitBoxes <= EngineConstants.NumberOfOrientations, "Too many hit boxes defined!");

        var result = Helpers.GetArrayForSize<HitBoxData>(numberOfDefinedHitBoxes);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadHitBoxDatum();
        }

        return result;
    }
    
    private HitBoxData ReadHitBoxDatum()
    {
        uint rawSolidityType = _reader.Read8BitUnsignedInteger();
        var solidityType = LemmingSolidityTypeHelpers.GetEnumValue(rawSolidityType);

        uint rawHitBoxBehaviour = _reader.Read8BitUnsignedInteger();
        var hitBoxBehaviour = HitBoxInteractionTypeHelpers.GetEnumValue(rawHitBoxBehaviour);

        var onLemmingEnterActions = ReadGadgetActionData(0);
        var onLemmingPresentActions = ReadGadgetActionData(1);
        var onLemmingExitActions = ReadGadgetActionData(2);

        var hitBoxCriteria = ReadHitBoxCriteria();

        var result = new HitBoxData
        {
            SolidityType = solidityType,
            HitBoxBehaviour = hitBoxBehaviour,
            InnateOnLemmingEnterActions = onLemmingEnterActions,
            InnateOnLemmingPresentActions = onLemmingPresentActions,
            InnateOnLemmingExitActions = onLemmingExitActions,

            InnateHitBoxCriteria = hitBoxCriteria
        };

        return result;
    }*/
    /*
    private LemmingBehaviourData[] ReadGadgetActionData(int expectedMarkerValue)
    {
        int actualMarkerValue = _reader.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(expectedMarkerValue == actualMarkerValue, "Mismatch in Gadget Action Data reading!");

        int numberOfGadgetActions = _reader.Read8BitUnsignedInteger();
        var result = Helpers.GetArrayForSize<LemmingBehaviourData>(numberOfGadgetActions);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadGadgetActionDatum();
        }

        return result;
    }

    private LemmingBehaviourData ReadGadgetActionDatum()
    {
        uint rawGadgetActionType = _reader.Read8BitUnsignedInteger();
        var gadgetActionType = LemmingBehaviourTypeHelpers.GetEnumValue(rawGadgetActionType);
        int miscData = _reader.Read32BitSignedInteger();

        return new LemmingBehaviourData(gadgetActionType, miscData);
    }
    */
    private HitBoxCriteriaData ReadHitBoxCriteria()
    {
        return new GadgetHitBoxCriteriaReader<RawStyleFileDataReader>(_reader).ReadHitBoxCriteria();
    }

    private HitBoxRegionData[] ReadRegionData()
    {
        var regionData = new HitBoxRegionData[EngineConstants.NumberOfOrientations];
        regionData[EngineConstants.DownOrientationRotNum] = ReadRegionDataForOrientation(Orientation.Down);
        regionData[EngineConstants.LeftOrientationRotNum] = ReadRegionDataForOrientation(Orientation.Left);
        regionData[EngineConstants.UpOrientationRotNum] = ReadRegionDataForOrientation(Orientation.Up);
        regionData[EngineConstants.RightOrientationRotNum] = ReadRegionDataForOrientation(Orientation.Right);

        return regionData;
    }

    private HitBoxRegionData ReadRegionDataForOrientation(Orientation orientation)
    {
        int rotNum = _reader.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(rotNum == orientation.RotNum, "HitBox region orientation mismatch!");

        uint rawHitBoxType = _reader.Read8BitUnsignedInteger();
        var actualHitBoxType = HitBoxTypeHelpers.GetEnumValue(rawHitBoxType);

        int numberOfPoints = _reader.Read16BitUnsignedInteger();
        var hitBoxPoints = Helpers.GetArrayForSize<Point>(numberOfPoints);

        for (var i = 0; i < hitBoxPoints.Length; i++)
        {
            int x = _reader.Read8BitUnsignedInteger();
            int y = _reader.Read8BitUnsignedInteger();
            hitBoxPoints[i] = new Point(x, y);
        }

        return new HitBoxRegionData
        {
            HitBoxOffset = default,
            Orientation = orientation,
            HitBoxType = actualHitBoxType,
            HitBoxDefinitionData = hitBoxPoints
        };
    }

    private AnimationLayerArchetypeData[] ReadAnimationLayerArchetypeData()
    {
        int numberOfAnimationLayers = _reader.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfAnimationLayers > 0, "Zero animation layers defined!");

        var result = new AnimationLayerArchetypeData[numberOfAnimationLayers];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadAnimationLayerArchetypeDatum(numberOfAnimationLayers);
        }

        return result;
    }

    private AnimationLayerArchetypeData ReadAnimationLayerArchetypeDatum(int numberOfAnimationLayers)
    {
        int layer = _reader.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(layer < numberOfAnimationLayers, "Invalid layer definition!");

        var animationLayerParameters = ReadAnimationLayerParameters();

        int initialFrame = _reader.Read8BitUnsignedInteger();
        // Need to offset by 1
        int nextGadgetState = _reader.Read8BitUnsignedInteger() - 1;

        return new AnimationLayerArchetypeData
        {
            Layer = layer,
            AnimationLayerParameters = animationLayerParameters,
            InitialFrame = initialFrame,

            NextGadgetState = nextGadgetState
        };
    }

    private AnimationLayerParameters ReadAnimationLayerParameters()
    {
        int frameStart = _reader.Read8BitUnsignedInteger();
        int frameEnd = _reader.Read8BitUnsignedInteger();
        int frameDelta = _reader.Read8BitUnsignedInteger();
        int transitionToFrame = _reader.Read8BitUnsignedInteger();

        return new AnimationLayerParameters(frameStart, frameEnd, frameDelta, transitionToFrame);
    }
}
