using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;
using NeoLemmixSharp.IO.Data.Style.Gadget.HitBox;
using NeoLemmixSharp.IO.FileFormats;
using System.Diagnostics;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0.Gadgets;

internal readonly ref struct GadgetStateWriter
{
    private readonly RawStyleFileDataWriter _rawFileData;
    private readonly StringIdLookup _stringIdLookup;

    internal GadgetStateWriter(RawStyleFileDataWriter rawFileData, StringIdLookup stringIdLookup)
    {
        _rawFileData = rawFileData;
        _stringIdLookup = stringIdLookup;
    }

    internal void WriteStateData(GadgetArchetypeData gadgetArchetypeData)
    {
        foreach (var gadgetStateData in gadgetArchetypeData.AllGadgetStateData)
        {
            WriteStateDatum(gadgetStateData);
        }
    }

    private void WriteStateDatum(GadgetStateArchetypeData gadgetStateData)
    {
        _rawFileData.Write(_stringIdLookup.GetStringId(gadgetStateData.StateName));

        var hitBoxOffsetData = ReadWriteHelpers.EncodePoint(gadgetStateData.HitBoxOffset);
        _rawFileData.Write(hitBoxOffsetData);

        WriteHitBoxData(gadgetStateData.HitBoxData);
        WriteRegionData(gadgetStateData.RegionData);

        WriteAnimationLayerArchetypeData(gadgetStateData.AnimationLayerData);
    }

    private void WriteHitBoxData(HitBoxData[] hitBoxData)
    {
        _rawFileData.Write((byte)hitBoxData.Length);

        foreach (var hitBoxDatum in hitBoxData)
        {
            WriteHitBoxDatum(hitBoxDatum);
        }
    }

    private void WriteHitBoxDatum(HitBoxData hitBoxDatum)
    {
        _rawFileData.Write((byte)hitBoxDatum.SolidityType);

        _rawFileData.Write((byte)hitBoxDatum.HitBoxBehaviour);

        WriteGadgetActionData(hitBoxDatum.OnLemmingEnterActions, 0);
        WriteGadgetActionData(hitBoxDatum.OnLemmingPresentActions, 1);
        WriteGadgetActionData(hitBoxDatum.OnLemmingExitActions, 2);

        WriteHitBoxCriteria(hitBoxDatum.HitBoxCriteria);
    }

    private void WriteGadgetActionData(GadgetActionData[] actionData, byte markerValue)
    {
        _rawFileData.Write(markerValue);

        _rawFileData.Write((byte)actionData.Length);

        foreach (var gadgetActionData in actionData)
        {
            WriteGadgetActionDatum(gadgetActionData);
        }
    }

    private void WriteGadgetActionDatum(GadgetActionData gadgetActionData)
    {
        _rawFileData.Write((byte)gadgetActionData.GadgetActionType);
        _rawFileData.Write(gadgetActionData.MiscData);
    }

    private void WriteHitBoxCriteria(HitBoxCriteriaData hitBoxCriteria)
    {
        new GadgetHitBoxCriteriaWriter<StyleFileSectionIdentifierHasher, StyleFileSectionIdentifier>(_rawFileData).WriteHitBoxCriteria(hitBoxCriteria);
    }

    private void WriteRegionData(HitBoxRegionData[] regionData)
    {
        Debug.Assert(regionData.Length == 4);

        WriteRegionDataForOrientation(regionData[EngineConstants.DownOrientationRotNum], Orientation.Down);
        WriteRegionDataForOrientation(regionData[EngineConstants.LeftOrientationRotNum], Orientation.Left);
        WriteRegionDataForOrientation(regionData[EngineConstants.UpOrientationRotNum], Orientation.Up);
        WriteRegionDataForOrientation(regionData[EngineConstants.RightOrientationRotNum], Orientation.Right);
    }

    private void WriteRegionDataForOrientation(HitBoxRegionData hitBoxRegionData, Orientation orientation)
    {
        FileWritingException.WriterAssert(hitBoxRegionData.Orientation == orientation, "HitBox region orientation mismatch!");

        _rawFileData.Write((byte)orientation.RotNum);

        _rawFileData.Write((byte)hitBoxRegionData.HitBoxType);

        _rawFileData.Write((ushort)hitBoxRegionData.HitBoxDefinitionData.Length);
        foreach (var point in hitBoxRegionData.HitBoxDefinitionData)
        {
            _rawFileData.Write((byte)point.X);
            _rawFileData.Write((byte)point.Y);
        }
    }

    private void WriteAnimationLayerArchetypeData(AnimationLayerArchetypeData[] animationLayerData)
    {
        _rawFileData.Write((byte)animationLayerData.Length);

        foreach (var animationLayerArchetypeDatum in animationLayerData)
        {
            WriteAnimationLayerArchetypeDatum(animationLayerArchetypeDatum);
        }
    }

    private void WriteAnimationLayerArchetypeDatum(AnimationLayerArchetypeData animationLayerArchetypeDatum)
    {
        _rawFileData.Write((byte)animationLayerArchetypeDatum.Layer);

        WriteAnimationLayerParameters(animationLayerArchetypeDatum);

        _rawFileData.Write((byte)animationLayerArchetypeDatum.InitialFrame);
        // Need to offset by 1
        _rawFileData.Write((byte)animationLayerArchetypeDatum.NextGadgetState + 1);
    }

    private void WriteAnimationLayerParameters(AnimationLayerArchetypeData animationLayerArchetypeDatum)
    {
        _rawFileData.Write((byte)animationLayerArchetypeDatum.AnimationLayerParameters.FrameStart);
        _rawFileData.Write((byte)animationLayerArchetypeDatum.AnimationLayerParameters.FrameEnd);
        _rawFileData.Write((byte)animationLayerArchetypeDatum.AnimationLayerParameters.FrameDelta);
        _rawFileData.Write((byte)animationLayerArchetypeDatum.AnimationLayerParameters.TransitionToFrame);
    }
}
