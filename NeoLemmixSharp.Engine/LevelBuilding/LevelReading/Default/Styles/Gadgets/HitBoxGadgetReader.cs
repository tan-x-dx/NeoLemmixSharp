using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util.Collections;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.HitBoxes;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders;
using NeoLemmixSharp.Engine.LevelBuilding.Data.Gadgets.Builders.ArchetypeData;

namespace NeoLemmixSharp.Engine.LevelBuilding.LevelReading.Default.Styles.Gadgets;

public static class HitBoxGadgetReader
{
    public static HitBoxGadgetArchetypeBuilder ReadGadget(string styleName, string pieceName, RawFileData rawFileData)
    {
        var gadgetStateData = ReadStateData(rawFileData);

        var spriteData = new SpriteDataReader().ReadSpriteData(rawFileData);

        var result = new HitBoxGadgetArchetypeBuilder
        {
            StyleName = styleName,
            PieceName = pieceName,

            ResizeType = ResizeType.None,

            AllGadgetStateData = gadgetStateData,
            SpriteData = spriteData
        };

        return result;
    }

    private static GadgetStateArchetypeData[] ReadStateData(RawFileData rawFileData)
    {
        int numberOfItemsInSection = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(numberOfItemsInSection > 0, "Zero state data defined!");

        var result = new GadgetStateArchetypeData[numberOfItemsInSection];
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

    private static HitBoxData[] ReadHitBoxData(RawFileData rawFileData)
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

    private static HitBoxData ReadHitBoxDatum(RawFileData rawFileData)
    {
        int rawLemmingSolidityType = rawFileData.Read8BitUnsignedInteger();
        int rawHitBoxBehaviour = rawFileData.Read8BitUnsignedInteger();

        var actualLemmingSolidityType = LemmingSolidityTypeHelpers.GetLemmingSolidityType(rawLemmingSolidityType);
        var actualHitBoxBehaviour = HitBoxBehaviourHelpers.GetGadgetHitBoxBehaviour(rawHitBoxBehaviour);

        GadgetActionReader.ReadGadgetActions(rawFileData, out var onLemmingEnterActions, out var onLemmingPresentActions, out var onLemmingExitActions);

        return new HitBoxData
        {
            SolidityType = actualLemmingSolidityType,
            HitBoxBehaviour = actualHitBoxBehaviour,

            OnLemmingEnterActions = onLemmingEnterActions,
            OnLemmingPresentActions = onLemmingPresentActions,
            OnLemmingExitActions = onLemmingExitActions,

            AllowedActions = null,
            AllowedStates = null,
            AllowedOrientations = null,
            AllowedFacingDirection = null
        };
    }

    private static HitBoxRegionData[] ReadHitBoxRegionData(RawFileData rawFileData)
    {
        var regionData = new HitBoxRegionData[EngineConstants.NumberOfOrientations];
        regionData[EngineConstants.DownOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Down);
        regionData[EngineConstants.LeftOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Left);
        regionData[EngineConstants.UpOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Up);
        regionData[EngineConstants.RightOrientationRotNum] = ReadHitBoxRegionDataForOrientation(rawFileData, Orientation.Right);

        return regionData;
    }

    private static HitBoxRegionData ReadHitBoxRegionDataForOrientation(RawFileData rawFileData, Orientation orientation)
    {
        int rotNum = rawFileData.Read8BitUnsignedInteger();

        LevelReadingException.ReaderAssert(rotNum == orientation.RotNum, "Hit box region data does not match expected orientation");

        int rawHitBoxType = rawFileData.Read8BitUnsignedInteger();
        var actualHitBoxType = HitBoxTypeHelpers.GetGadgetHitBoxType(rawHitBoxType);

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
}
