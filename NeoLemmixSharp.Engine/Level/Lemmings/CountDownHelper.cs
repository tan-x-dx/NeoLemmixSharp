using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Rendering.Text;

namespace NeoLemmixSharp.Engine.Level.Lemmings;

public static class CountDownHelper
{
    public static void UpdateCountDownTimer(Lemming lemming)
    {
        var lemmingRenderer = lemming.Renderer;

        var textSpan = lemmingRenderer.CountDownCharsSpan;
        var countDownValue = GetCountDownValue(lemming);

        TextRenderingHelpers.WriteDigits(textSpan, countDownValue);
    }

    private static uint GetCountDownValue(Lemming lemming)
    {
        var countDownTimer = lemming.CountDownTimer;

        return lemming.IsFastForward
            ? (countDownTimer + EngineConstants.EngineTicksPerSecond - 1) / EngineConstants.EngineTicksPerSecond
            : (countDownTimer + EngineConstants.GameplayTicksPerSecond - 1) / EngineConstants.GameplayTicksPerSecond;
    }
}