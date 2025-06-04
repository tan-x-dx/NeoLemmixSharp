using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.IO.Data.Level.Gadgets;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.Data.Style.Theme;
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

    internal GadgetStateArchetypeData ReadStateData(Size baseSpriteSize)
    {
        int stateNameId = _rawFileData.Read16BitUnsignedInteger();

        int offsetX = _rawFileData.Read16BitUnsignedInteger();
        int offsetY = _rawFileData.Read16BitUnsignedInteger();

        var hitBoxData = ReadHitBoxData();
        var regionData = ReadRegionData();

        var animationLayerData = ReadAnimationLayerData(baseSpriteSize);

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

        byte allowedLemmingOrientationIds = _rawFileData.Read8BitUnsignedInteger();
        byte allowedFacingDirectionId = _rawFileData.Read8BitUnsignedInteger();

        var result = new HitBoxData
        {
            SolidityType = solidityType,
            HitBoxBehaviour = hitBoxBehaviour,
            OnLemmingEnterActions = onLemmingEnterActions,
            OnLemmingPresentActions = onLemmingPresentActions,
            OnLemmingExitActions = onLemmingExitActions,
            AllowedLemmingActionIds = allowedLemmingActionIds,
            AllowedLemmingStateIds = allowedLemmingStateIds,
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

        FileReadingException.ReaderAssert(BitArrayHelpers.GetPopCount(result) > 0, "No bits set when reading bit sequence!");

        return result;
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

    private AnimationLayerArchetypeData[] ReadAnimationLayerData(Size baseSpriteSize)
    {
        int numberOfAnimationBehaviours = _rawFileData.Read8BitUnsignedInteger();

        FileReadingException.ReaderAssert(numberOfAnimationBehaviours > 0, "Zero animation data defined!");

        var result = new AnimationLayerArchetypeData[numberOfAnimationBehaviours];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = ReadAnimationBehaviourArchetypData(baseSpriteSize);
        }

        return result;
    }

    private AnimationLayerArchetypeData ReadAnimationBehaviourArchetypData(Size baseSpriteSize)
    {
        var animationLayerParameters = ReadAnimationLayerParameters();

        var nineSliceData = ReadNineSliceData(baseSpriteSize);

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

    private RectangularRegion ReadNineSliceData(Size baseSpriteSize)
    {
        int nineSliceLeft = _rawFileData.Read16BitUnsignedInteger();
        int nineSliceWidth = _rawFileData.Read16BitUnsignedInteger();

        int nineSliceTop = _rawFileData.Read16BitUnsignedInteger();
        int nineSliceHeight = _rawFileData.Read16BitUnsignedInteger();

        FileReadingException.ReaderAssert(nineSliceLeft >= 0, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceWidth >= 1, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceLeft + nineSliceWidth <= baseSpriteSize.W, "Invalid nine slice definition!");

        FileReadingException.ReaderAssert(nineSliceTop >= 0, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceHeight >= 1, "Invalid nine slice definition!");
        FileReadingException.ReaderAssert(nineSliceTop + nineSliceHeight <= baseSpriteSize.H, "Invalid nine slice definition!");

        var p = new Point(nineSliceLeft, nineSliceTop);
        var s = new Size(nineSliceWidth, nineSliceHeight);
        return new RectangularRegion(p, s);
    }
}
