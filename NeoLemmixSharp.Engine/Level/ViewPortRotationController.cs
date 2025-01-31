using NeoLemmixSharp.Engine.Level.Orientations;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Level;

public sealed class ViewPortRotationController
{
    private const float Pi = MathF.PI;
    private const float PiByTwo = MathF.PI / 2.0f;
    private const float NegativePiByTwo = MathF.PI / -2.0f;
    private const float DegreesToRadiansConversionFactor = MathF.PI / 180f;

    private int _t;
    private int _deltaT;
    private float _theta;

    private Orientation _currentOrientation = Orientation.Down;
    private Orientation? _transitionOrientation;

    public float Theta => _theta;

    public void SetTransitionTo(Orientation orientation)
    {
        if (_transitionOrientation is null)
        {
            _transitionOrientation = orientation;
        }
        else if (_transitionOrientation != orientation)
        {
            _currentOrientation = _transitionOrientation.Value;
            _transitionOrientation = orientation;
        }

        UpdateTValues();
    }

    private void UpdateTValues()
    {
        var currentOrientationRotNum = _currentOrientation.RotNum;
        var transitionOrientationRotNum = _transitionOrientation!.Value.RotNum;


    }

    public void Tick()
    {
        if (_transitionOrientation is null)
            return;

        _t += _deltaT;
        _theta = GetTheta(out var is90DegreeAngle);

        if (is90DegreeAngle)
        {
            _transitionOrientation = null;
        }
    }

    [Pure]
    private float GetTheta(out bool is90DegreeAngle)
    {
        is90DegreeAngle = true;
        // For these specific 90∘ angles, use the precomputed constants
        if (_t == 0)
            return 0f;

        if (_t == 90)
            return PiByTwo;

        if (_t == 180)
            return Pi;

        if (_t == -90)
            return NegativePiByTwo;

        is90DegreeAngle = false;
        return _t * DegreesToRadiansConversionFactor;
    }
}