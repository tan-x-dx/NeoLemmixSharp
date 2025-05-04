using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Common.Util.Collections.BitArrays;
using NeoLemmixSharp.Engine.Level.Gadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.Level.LemmingActions;
using NeoLemmixSharp.Engine.Level.Skills;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets;
using NeoLemmixSharp.Engine.LevelIo.Data.Gadgets.ArchetypeData;
using System.Runtime.InteropServices;

namespace NeoLemmixSharp.Engine.LevelIo.LevelReading.Default.Styles.Gadgets;

public static class HitBoxGadgetReader
{
    public static GadgetArchetypeData ReadGadgetArchetypeData(
        string styleName,
        string pieceName,
        GadgetType gadgetType,
        RawStyleFileDataReader rawFileData)
    {
        int resizeTypeByte = rawFileData.Read8BitUnsignedInteger();
        var resizeType = (ResizeType)(resizeTypeByte & 3);

        var gadgetStateData = ReadStateData(rawFileData);

        var spriteData = new SpriteDataReader().ReadSpriteData(rawFileData, gadgetStateData.Length);

        var result = new GadgetArchetypeData
        {
            StyleName = styleName,
            PieceName = pieceName,

            GadgetType = gadgetType,
            ResizeType = resizeType,

            AllGadgetStateData = gadgetStateData,
            SpriteData = spriteData
        };

        return result;
    }

    private static GadgetStateArchetypeData[] ReadStateData(RawStyleFileDataReader rawFileData)
    {
        int numberOfGadgetStates = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(numberOfGadgetStates > 0, "Zero state data defined!");

        var result = new GadgetStateArchetypeData[numberOfGadgetStates];
        var i = 0;

        while (i < result.Length)
        {
            int offsetX = rawFileData.Read8BitUnsignedInteger();
            int offsetY = rawFileData.Read8BitUnsignedInteger();

            var hitBoxData = ReadHitBoxData(rawFileData);
            var regionData = ReadHitBoxRegionData(rawFileData);

            result[i++] = new GadgetStateArchetypeData
            {
                HitBoxOffset = new Point(offsetX, offsetY),
                HitBoxData = hitBoxData,
                RegionData = regionData
            };
        }

        return result;
    }

    private static HitBoxData[] ReadHitBoxData(RawStyleFileDataReader rawFileData)
    {
        int numberOfHitBoxData = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(numberOfHitBoxData > 0, "Zero hit box data defined!");

        var result = new HitBoxData[numberOfHitBoxData];

        var i = 0;
        while (i < result.Length)
        {
            result[i++] = ReadHitBoxDatum(rawFileData);
        }

        return result;
    }

    private static HitBoxData ReadHitBoxDatum(RawStyleFileDataReader rawFileData)
    {
        int rawLemmingSolidityType = rawFileData.Read8BitUnsignedInteger();
        int rawHitBoxBehaviour = rawFileData.Read8BitUnsignedInteger();

        var actualLemmingSolidityType = LemmingSolidityTypeHelpers.GetEnumValue(rawLemmingSolidityType);
        var actualHitBoxBehaviour = HitBoxBehaviourHelpers.GetEnumValue(rawHitBoxBehaviour);

        GadgetActionReader.ReadGadgetActions(rawFileData, out var onLemmingEnterActions, out var onLemmingPresentActions, out var onLemmingExitActions);

        ReadFilterData(rawFileData, out var allowedActions, out var allowedStates, out var allowedOrientations, out var allowedFacingDirection);

        return new HitBoxData
        {
            SolidityType = actualLemmingSolidityType,
            HitBoxBehaviour = actualHitBoxBehaviour,

            OnLemmingEnterActions = onLemmingEnterActions,
            OnLemmingPresentActions = onLemmingPresentActions,
            OnLemmingExitActions = onLemmingExitActions,

            AllowedActions = allowedActions,
            AllowedStates = allowedStates,
            AllowedOrientations = allowedOrientations,
            AllowedFacingDirection = allowedFacingDirection
        };
    }

    private static HitBoxRegionData[] ReadHitBoxRegionData(RawStyleFileDataReader rawFileData)
    {
        var regionData = new HitBoxRegionData[EngineConstants.NumberOfOrientations];
        regionData[EngineConstants.DownOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Down);
        regionData[EngineConstants.LeftOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Left);
        regionData[EngineConstants.UpOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Up);
        regionData[EngineConstants.RightOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Right);

        return regionData;
    }

    private static HitBoxRegionData ReadHitBoxRegionDataForOrientation(RawStyleFileDataReader rawFileData, Orientation orientation)
    {
        int rotNum = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(rotNum == orientation.RotNum, "Hit box region data does not match expected orientation");

        int rawHitBoxType = rawFileData.Read8BitUnsignedInteger();
        var actualHitBoxType = HitBoxTypeHelpers.GetEnumValue(rawHitBoxType);

        int numberOfPoints = rawFileData.Read16BitUnsignedInteger();
        var hitBoxPoints = CollectionsHelper.GetArrayForSize<Point>(numberOfPoints);

        var i = 0;
        while (i < hitBoxPoints.Length)
        {
            int x = rawFileData.Read8BitUnsignedInteger();
            int y = rawFileData.Read8BitUnsignedInteger();
            hitBoxPoints[i++] = new Point(x, y);
        }

        return new HitBoxRegionData
        {
            Orientation = orientation,
            HitBoxType = actualHitBoxType,
            HitBoxDefinitionData = hitBoxPoints
        };
    }

    private static void ReadFilterData(
        RawStyleFileDataReader rawFileData,
        out LemmingActionSet? allowedActions,
        out StateChangerSet? allowedStates,
        out OrientationSet? allowedOrientations,
        out FacingDirection? allowedFacingDirection)
    {
        uint hitBoxFilterByte = rawFileData.Read8BitUnsignedInteger();

        var filterData = LevelReadWriteHelpers.DecodeGadgetArchetypeDataFilterByte(hitBoxFilterByte);

        if (filterData.HasAllowedActions)
        {
            allowedActions = ReadBitArraySet<LemmingAction.LemmingActionHasher, LemmingAction.LemmingActionBitBuffer, LemmingAction>(rawFileData);
        }
        else
        {
            allowedActions = null;
        }

        if (filterData.HasAllowedStates)
        {
            allowedStates = ReadBitArraySet<ILemmingStateChanger.LemmingStateChangerHasher, BitBuffer32, ILemmingStateChanger>(rawFileData);
        }
        else
        {
            allowedStates = null;
        }

        if (filterData.HasAllowedOrientations)
        {
            allowedOrientations = ReadBitArraySet<Orientation.OrientationHasher, BitBuffer32, Orientation>(rawFileData);
        }
        else
        {
            allowedOrientations = null;
        }

        if (filterData.HasAllowedFacingDirection)
        {
            allowedFacingDirection = new FacingDirection(rawFileData.Read8BitUnsignedInteger());
        }
        else
        {
            allowedFacingDirection = null;
        }
    }

    private static BitArraySet<THasher, TBuffer, T> ReadBitArraySet<THasher, TBuffer, T>(RawStyleFileDataReader rawFileData)
        where THasher : struct, IPerfectHasher<T>, IBitBufferCreator<TBuffer>
        where TBuffer : struct, IBitBuffer
        where T : notnull
    {
        var hasher = new THasher();

        var expectedNumberOfItems = hasher.NumberOfItems;
        var bufferLength = BitArrayHelpers.CalculateBitArrayBufferLength(expectedNumberOfItems);

        var result = new BitArraySet<THasher, TBuffer, T>(hasher, false);

        var bytes = rawFileData.ReadBytes(bufferLength * sizeof(uint));
        var uints = MemoryMarshal.Cast<byte, uint>(bytes);
        result.ReadFrom(uints);

        return result;
    }
}
