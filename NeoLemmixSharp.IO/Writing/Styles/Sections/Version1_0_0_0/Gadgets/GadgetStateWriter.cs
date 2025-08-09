using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Writing.Styles.Sections.Version1_0_0_0.Gadgets;

internal readonly ref struct GadgetStateWriter
{
    private readonly RawStyleFileDataWriter _reader;
    private readonly FileWriterStringIdLookup _stringIdLookup;

    internal GadgetStateWriter(RawStyleFileDataWriter reader, FileWriterStringIdLookup stringIdLookup)
    {
        _reader = reader;
        _stringIdLookup = stringIdLookup;
    }

    internal void WriteStateData(IGadgetArchetypeData gadgetArchetypeData)
    {
        /*   foreach (var gadgetStateData in gadgetArchetypeData.AllGadgetStateData)
           {
               WriteStateDatum(gadgetStateData);
           }*/
    }

    /*  private void WriteStateDatum(GadgetStateArchetypeData gadgetStateData)
      {
          _reader.Write16BitUnsignedInteger(_stringIdLookup.GetStringId(gadgetStateData.StateName));

          var hitBoxOffsetData = ReadWriteHelpers.EncodePoint(gadgetStateData.HitBoxOffset);
          _reader.Write32BitSignedInteger(hitBoxOffsetData);

          WriteHitBoxData(gadgetStateData.HitBoxData);
          WriteRegionData(gadgetStateData.RegionData);

          WriteAnimationLayerArchetypeData(gadgetStateData.AnimationLayerData);
      }

      private void WriteHitBoxData(HitBoxData[] hitBoxData)
      {
          _reader.Write8BitUnsignedInteger((byte)hitBoxData.Length);

          foreach (var hitBoxDatum in hitBoxData)
          {
              WriteHitBoxDatum(hitBoxDatum);
          }
      }

      private void WriteHitBoxDatum(HitBoxData hitBoxDatum)
      {
          _reader.Write8BitUnsignedInteger((byte)hitBoxDatum.SolidityType);

          _reader.Write8BitUnsignedInteger((byte)hitBoxDatum.HitBoxBehaviour);

          WriteGadgetActionData(hitBoxDatum.InnateOnLemmingEnterActions, 0);
          WriteGadgetActionData(hitBoxDatum.InnateOnLemmingPresentActions, 1);
          WriteGadgetActionData(hitBoxDatum.InnateOnLemmingExitActions, 2);

          WriteHitBoxCriteria(hitBoxDatum.InnateHitBoxCriteria);
      }

      private void WriteGadgetActionData(GadgetActionData[] actionData, byte markerValue)
      {
          _reader.Write8BitUnsignedInteger(markerValue);

          _reader.Write8BitUnsignedInteger((byte)actionData.Length);

          foreach (var gadgetActionData in actionData)
          {
              WriteGadgetActionDatum(gadgetActionData);
          }
      }

      private void WriteGadgetActionDatum(GadgetActionData gadgetActionData)
      {
          _reader.Write8BitUnsignedInteger((byte)gadgetActionData.GadgetActionType);
          _reader.Write32BitSignedInteger(gadgetActionData.MiscData);
      }

      private void WriteHitBoxCriteria(HitBoxCriteriaData hitBoxCriteria)
      {
          new GadgetHitBoxCriteriaWriter<RawStyleFileDataWriter>(_reader).WriteHitBoxCriteria(hitBoxCriteria);
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

          _reader.Write8BitUnsignedInteger((byte)orientation.RotNum);

          _reader.Write8BitUnsignedInteger((byte)hitBoxRegionData.HitBoxType);

          _reader.Write16BitUnsignedInteger((ushort)hitBoxRegionData.HitBoxDefinitionData.Length);
          foreach (var point in hitBoxRegionData.HitBoxDefinitionData)
          {
              _reader.Write8BitUnsignedInteger((byte)point.X);
              _reader.Write8BitUnsignedInteger((byte)point.Y);
          }
      }

      private void WriteAnimationLayerArchetypeData(AnimationLayerArchetypeData[] animationLayerData)
      {
          _reader.Write8BitUnsignedInteger((byte)animationLayerData.Length);

          foreach (var animationLayerArchetypeDatum in animationLayerData)
          {
              WriteAnimationLayerArchetypeDatum(animationLayerArchetypeDatum);
          }
      }

      private void WriteAnimationLayerArchetypeDatum(AnimationLayerArchetypeData animationLayerArchetypeDatum)
      {
          _reader.Write8BitUnsignedInteger((byte)animationLayerArchetypeDatum.Layer);

          WriteAnimationLayerParameters(animationLayerArchetypeDatum);

          _reader.Write8BitUnsignedInteger((byte)animationLayerArchetypeDatum.InitialFrame);
          // Need to offset by 1
          _reader.Write32BitSignedInteger((byte)animationLayerArchetypeDatum.NextGadgetState + 1);
      }

      private void WriteAnimationLayerParameters(AnimationLayerArchetypeData animationLayerArchetypeDatum)
      {
          _reader.Write8BitUnsignedInteger((byte)animationLayerArchetypeDatum.AnimationLayerParameters.FrameStart);
          _reader.Write8BitUnsignedInteger((byte)animationLayerArchetypeDatum.AnimationLayerParameters.FrameEnd);
          _reader.Write8BitUnsignedInteger((byte)animationLayerArchetypeDatum.AnimationLayerParameters.FrameDelta);
          _reader.Write8BitUnsignedInteger((byte)animationLayerArchetypeDatum.AnimationLayerParameters.TransitionToFrame);
      }*/
}
