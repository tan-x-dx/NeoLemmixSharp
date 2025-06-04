using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal readonly ref struct GadgetStateReader
{
    private readonly RawStyleFileDataReader _rawFileData;
    private readonly StringIdLookup _stringIdLookup;

    internal GadgetStateReader(RawStyleFileDataReader rawFileData, StringIdLookup stringIdLookup)
    {
        _rawFileData = rawFileData;
        _stringIdLookup = stringIdLookup;
    }

    internal GadgetStateArchetypeData ReadStateData()
    {
        int stateNameId = _rawFileData.Read16BitUnsignedInteger();

        int offsetX = _rawFileData.Read16BitUnsignedInteger();
        int offsetY = _rawFileData.Read16BitUnsignedInteger();

        var hitBoxData = ReadHitBoxData();
        var regionData = ReadRegionData();

        var animationLayerData = ReadAnimationLayerArchetypeData();

        var result = new GadgetStateArchetypeData
        {
            StateName = _stringIdLookup[stateNameId],
            HitBoxOffset = new Point(offsetX, offsetY),
            HitBoxData = hitBoxData,
            RegionData = regionData,

            AnimationLayerData = animationLayerData,
        };

        return result;
    }

    private HitBoxData[] ReadHitBoxData()
    {
        int numberOfDefinedHitBoxes = _rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfDefinedHitBoxes <= EngineConstants.NumberOfOrientations, "Too many hit boxes defined!");

        var result = CollectionsHelper.GetArrayForSize<HitBoxData>(numberOfDefinedHitBoxes);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadHitBoxDatum();
        }

        return result;
    }

    private HitBoxData ReadHitBoxDatum()
    {
        uint rawSolidityType = _rawFileData.Read8BitUnsignedInteger();
        var solidityType = LemmingSolidityTypeHelpers.GetEnumValue(rawSolidityType);

        uint rawHitBoxBehaviour = _rawFileData.Read8BitUnsignedInteger();
        var hitBoxBehaviour = HitBoxBehaviourHelpers.GetEnumValue(rawHitBoxBehaviour);

        var onLemmingEnterActions = ReadGadgetActionData(0);
        var onLemmingPresentActions = ReadGadgetActionData(1);
        var onLemmingExitActions = ReadGadgetActionData(2);

        var allowedLemmingActionIds = ReadUintSequence();
        var allowedLemmingStateIds = ReadUintSequence();

        byte? allowedLemmingTribeId = ReadAllowedLemmingTribeId();
        byte? allowedLemmingOrientationIds = ReadAllowedLemmingOrientationIds();
        byte? allowedFacingDirectionId = ReadAllowedLemmingFacingDirectionId();

        var result = new HitBoxData
        {
            SolidityType = solidityType,
            HitBoxBehaviour = hitBoxBehaviour,
            OnLemmingEnterActions = onLemmingEnterActions,
            OnLemmingPresentActions = onLemmingPresentActions,
            OnLemmingExitActions = onLemmingExitActions,
            AllowedLemmingActionIds = allowedLemmingActionIds,
            AllowedLemmingStateIds = allowedLemmingStateIds,
            AllowedLemmingTribeIds = allowedLemmingTribeId,
            AllowedLemmingOrientationIds = allowedLemmingOrientationIds,
            AllowedFacingDirectionId = allowedFacingDirectionId
        };

        return result;
    }

    private GadgetActionData[] ReadGadgetActionData(int expectedMarkerValue)
    {
        int actualMarkerValue = _rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(expectedMarkerValue == actualMarkerValue, "Mismatch in Gadget Action Data reading!");

        int numberOfGadgetActions = _rawFileData.Read8BitUnsignedInteger();
        var result = CollectionsHelper.GetArrayForSize<GadgetActionData>(numberOfGadgetActions);

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadGadgetActionDatum();
        }

        return result;
    }

    private GadgetActionData ReadGadgetActionDatum()
    {
        uint rawGadgetActionType = _rawFileData.Read8BitUnsignedInteger();
        var gadgetActionType = GadgetActionTypeHelpers.GetEnumValue(rawGadgetActionType);
        int miscData = _rawFileData.Read32BitSignedInteger();

        return new GadgetActionData(gadgetActionType, miscData);
    }

    private uint[] ReadUintSequence()
    {
        int numberOfBytesToRead = _rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert((numberOfBytesToRead % sizeof(uint)) == 0, "Expected to read a multiple of 4 bytes!");

        var result = CollectionsHelper.GetArrayForSize<uint>(numberOfBytesToRead >> 2);

        var sourceBytes = _rawFileData.ReadBytes(numberOfBytesToRead);
        var destBytes = MemoryMarshal.Cast<uint, byte>(result);
        sourceBytes.CopyTo(destBytes);

        AssertNonZeroUintSequence(result);

        return result;
    }

    private static void AssertNonZeroUintSequence(uint[] bits)
    {
        foreach (var value in bits)
        {
            if (value != 0)
                return;
        }

        throw new FileReadingException("No bits set when reading bit sequence!");
    }

    private byte? ReadAllowedLemmingTribeId()
    {
        int rawValue = _rawFileData.Read8BitUnsignedInteger();

        var hasTribeData = ((rawValue >>> EngineConstants.MaxNumberOfTribes) & 1) != 0;

        const int TribeMask = (1 << EngineConstants.MaxNumberOfTribes) - 1;

        if (hasTribeData)
            return (byte)(rawValue & TribeMask);

        return null;
    }

    private byte? ReadAllowedLemmingOrientationIds()
    {
        int rawValue = _rawFileData.Read8BitUnsignedInteger();

        var hasOrientationData = ((rawValue >>> EngineConstants.NumberOfOrientations) & 1) != 0;

        const int OrientationMask = (1 << EngineConstants.NumberOfOrientations) - 1;

        if (hasOrientationData)
            return (byte)(rawValue & OrientationMask);

        return null;
    }

    private byte? ReadAllowedLemmingFacingDirectionId()
    {
        int rawValue = _rawFileData.Read8BitUnsignedInteger();

        var hasFacingDirectionData = ((rawValue >>> 1) & 1) != 0;

        if (hasFacingDirectionData)
            return (byte)(rawValue & 1);

        return null;
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
        int rotNum = _rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(rotNum == orientation.RotNum, "Hit box region data does not match expected orientation");

        uint rawHitBoxType = _rawFileData.Read8BitUnsignedInteger();
        var actualHitBoxType = HitBoxTypeHelpers.GetEnumValue(rawHitBoxType);

        int numberOfPoints = _rawFileData.Read16BitUnsignedInteger();
        var hitBoxPoints = CollectionsHelper.GetArrayForSize<Point>(numberOfPoints);

        for (var i = 0; i < hitBoxPoints.Length; i++)
        {
            int x = _rawFileData.Read8BitUnsignedInteger();
            int y = _rawFileData.Read8BitUnsignedInteger();
            hitBoxPoints[i] = new Point(x, y);
        }

        return new HitBoxRegionData
        {
            Orientation = orientation,
            HitBoxType = actualHitBoxType,
            HitBoxDefinitionData = hitBoxPoints
        };
    }

    private AnimationLayerArchetypeData[] ReadAnimationLayerArchetypeData()
    {
        int numberOfAnimationLayers = _rawFileData.Read8BitUnsignedInteger();

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
        int layer = _rawFileData.Read8BitUnsignedInteger();
        FileReadingException.ReaderAssert(layer < numberOfAnimationLayers, "Invalid layer definition!");

        var animationLayerParameters = ReadAnimationLayerParameters();

        int initialFrame = _rawFileData.Read8BitUnsignedInteger();
        int nextGadgetState = _rawFileData.Read8BitUnsignedInteger();

        return new AnimationLayerArchetypeData
        {
            Layer = layer,
            AnimationLayerParameters = animationLayerParameters,
            InitialFrame = initialFrame,

            // Need to offset by 1
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
}
