namespace NeoLemmixSharp.Common.Util.GameInput;

public sealed class NonRepeatingActionPerformer
{
    private bool _canPerformAction = true;

    public bool Evaluate(bool condition)
    {
        if (!condition)
        {
            _canPerformAction = true;
            return false;
        }

        if (!_canPerformAction)
            return false;

        _canPerformAction = false;
        return true;
    }
}