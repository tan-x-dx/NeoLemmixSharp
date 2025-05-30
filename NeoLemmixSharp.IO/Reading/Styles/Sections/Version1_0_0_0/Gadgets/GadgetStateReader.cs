using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
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
        var spriteData = ReadSpriteData();

        var result = new GadgetStateArchetypeData
        {
            StateName = _stringIdLookup[stateNameId],
            HitBoxOffset = new Point(offsetX, offsetY),
            HitBoxData = hitBoxData,
            RegionData = regionData,
            SpriteData = spriteData,
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

    private SpriteArchetypeData ReadSpriteData()
    {
        throw new NotImplementedException();
    }
}
