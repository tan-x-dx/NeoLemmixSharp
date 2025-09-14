using NeoLemmixSharp.Common.Enums;

namespace NeoLemmixSharp.Engine.Level.Gadgets.CommonTriggers;

public sealed class GadgetPositionTrigger : GadgetTrigger
{
    private readonly int _x;
    private readonly int _y;

    private readonly ComparisonType _comparisonX;
    private readonly ComparisonType _comparisonY;

    private readonly bool _requireX;
    private readonly bool _requireY;

    public GadgetPositionTrigger(
        int x,
        int y,
        ComparisonType comparisonX,
        ComparisonType comparisonY,
        bool requireX,
        bool requireY)
        : base(GadgetTriggerType.GadgetPositionTrigger)
    {
        _x = x;
        _y = y;
        _comparisonX = comparisonX;
        _comparisonY = comparisonY;
        _requireX = requireX;
        _requireY = requireY;
    }

    public override void DetectTrigger(GadgetBase parentGadget)
    {
        var xMatches = !_requireX || _comparisonX.ComparisonMatches(parentGadget.CurrentGadgetBounds.X, _x);
        var yMatches = !_requireY || _comparisonY.ComparisonMatches(parentGadget.CurrentGadgetBounds.Y, _y);

        var triggered = xMatches && yMatches;

        DetermineTrigger(triggered);
        if (triggered)
            TriggerBehaviours();
        MarkAsEvaluated();
    }
}
