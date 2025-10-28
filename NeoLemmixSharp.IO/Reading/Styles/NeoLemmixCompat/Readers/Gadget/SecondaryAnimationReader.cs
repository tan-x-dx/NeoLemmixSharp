using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Data;
using NeoLemmixSharp.IO.Reading.Levels.NeoLemmixCompat.Readers;
using NeoLemmixSharp.IO.Util;

namespace NeoLemmixSharp.IO.Reading.Styles.NeoLemmixCompat.Readers.Gadget;

internal sealed class SecondaryAnimationReader : NeoLemmixDataReader
{
    private readonly NeoLemmixGadgetArchetypeData _gadgetArchetypeData;
    private readonly UniqueStringSet _uniqueStringSet;

    //  private AnimationData? _secondaryAnimationData;
    //  private AnimationTriggerReader? _triggerReader;

    internal SecondaryAnimationReader(
        NeoLemmixGadgetArchetypeData gadgetArchetypeData,
        UniqueStringSet uniqueStringSet)
        : base("$ANIMATION")
    {
        _gadgetArchetypeData = gadgetArchetypeData;
        _uniqueStringSet = uniqueStringSet;

        RegisterTokenAction("INITIAL_FRAME", SetInitialFrame);
        RegisterTokenAction("FRAMES", SetFrameCount);
        RegisterTokenAction("NAME", SetName);
        RegisterTokenAction("HIDE", SetHidden);
        RegisterTokenAction("OFFSET_X", SetOffsetX);
        RegisterTokenAction("OFFSET_Y", SetOffsetY);
        RegisterTokenAction("COLOR", SetColor);
        RegisterTokenAction("$TRIGGER", SetTriggerData);
        RegisterTokenAction("$END", OnEnd);
    }

    public override bool BeginReading(ReadOnlySpan<char> line)
    {
        FinishedReading = false;

        //    _secondaryAnimationData = new AnimationData();
        return false;
    }

    /* public override bool ReadNextLine(ReadOnlySpan<char> line)
     {
         if (_triggerReader != null)
         {
             var result = _triggerReader.ReadNextLine(line);

             if (_triggerReader.FinishedReading)
             {
                 _triggerReader = null;
             }

             return result;
         }

         NxlvReadingHelpers.GetTokenPair(line, out var firstToken, out var secondToken, out var secondTokenIndex);

         var alternateLookup = _tokenActions.GetAlternateLookup<ReadOnlySpan<char>>();

         if (alternateLookup.TryGetValue(firstToken, out var tokenAction))
         {
             tokenAction(line, secondToken, secondTokenIndex);
         }
         else
         {
             NxlvReadingHelpers.ThrowUnknownTokenException(IdentifierToken, firstToken, line);
         }

         return false;
     }*/

    private void SetInitialFrame(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        var initialFrame = TokensMatch(secondToken, "RANDOM")
            ? 0
            : int.Parse(secondToken);

        // _secondaryAnimationData!.InitialFrame = initialFrame;
    }

    private void SetFrameCount(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _secondaryAnimationData!.NumberOfFrames = int.Parse(secondToken);
    }

    private void SetName(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _secondaryAnimationData!.Name = _uniqueStringSet.GetUniqueStringInstance(secondToken);
    }

    private void SetHidden(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _secondaryAnimationData!.Hide = true;
    }

    private void SetOffsetX(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _secondaryAnimationData!.OffsetX = int.Parse(secondToken);
    }

    private void SetOffsetY(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _secondaryAnimationData!.OffsetY = int.Parse(secondToken);
    }

    private void SetColor(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
    }

    private void SetTriggerData(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _triggerReader = new AnimationTriggerReader(_secondaryAnimationData!.TriggerData);
        // _triggerReader.BeginReading("$TRIGGER");
    }

    private void OnEnd(ReadOnlySpan<char> line, ReadOnlySpan<char> secondToken, int secondTokenIndex)
    {
        // _secondaryAnimationData = null;
        // _gadgetArchetypeData.AnimationData.Add(_secondaryAnimationData!);
        FinishedReading = true;
    }
}
