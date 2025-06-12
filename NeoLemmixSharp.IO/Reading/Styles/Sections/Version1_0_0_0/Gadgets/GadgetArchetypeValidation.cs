using NeoLemmixSharp.Common;
using NeoLemmixSharp.IO.Data.Style.Gadget;

namespace NeoLemmixSharp.IO.Reading.Styles.Sections.Version1_0_0_0.Gadgets;

internal static class GadgetArchetypeValidation
{
    internal static void AssertGadgetStateDataMakesSense(
        GadgetType gadgetType,
        GadgetStateArchetypeData[] gadgetStates)
    {
        var baseGadgetType = gadgetType.GetBaseGadgetType();

        switch (baseGadgetType)
        {
            case BaseGadgetType.HitBox:
                return;

            case BaseGadgetType.Hatch:
                AssertHatchGadgetStateDataMakesSense();
                return;

            case BaseGadgetType.Functional:
                AssertFunctionalGadgetStateDataMakesSense();
                return;
        }

        return;

        void AssertHatchGadgetStateDataMakesSense()
        {
            FileReadingException.ReaderAssert(gadgetStates.Length == 2, "Expected exactly 2 states for Hatch gadget!");
        }

        void AssertFunctionalGadgetStateDataMakesSense()
        {
            FileReadingException.ReaderAssert(gadgetStates.Length == EngineConstants.NumberOfAllowedStatesForFunctionalGadgets, "Expected exactly 2 states for Functional gadget!");
        }
    }

    internal static void AssertGadgetArchetypeDataHasRequiredMiscData(GadgetArchetypeData gadgetArchetypeData)
    {
        var baseGadgetType = gadgetArchetypeData.GadgetType.GetBaseGadgetType();

        switch (baseGadgetType)
        {
            case BaseGadgetType.HitBox:
                return;

            case BaseGadgetType.Hatch:
                AssertHatchGadgetArchetypeDataHasRequiredMiscData(gadgetArchetypeData);
                return;

            case BaseGadgetType.Functional:
                AssertFunctionalGadgetArchetypeDataHasRequiredMiscData(gadgetArchetypeData);
                return;
        }

        return;
    }

    private static void AssertHatchGadgetArchetypeDataHasRequiredMiscData(GadgetArchetypeData hatchGadgetArchetypeData)
    {
        FileReadingException.ReaderAssert(hatchGadgetArchetypeData.HasMiscData(GadgetArchetypeMiscDataType.SpawnPointOffset), "Missing spawn point offset for Hatch gadget!");
    }

    private static void AssertFunctionalGadgetArchetypeDataHasRequiredMiscData(GadgetArchetypeData functionalGadgetArchetypeData)
    {
    }
}
