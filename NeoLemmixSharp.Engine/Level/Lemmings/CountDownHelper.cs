using NeoLemmixSharp.Common;
using NeoLemmixSharp.Common.Util;

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

    private static int GetCountDownValue(Lemming lemming)
    {
        var countDownTimer = lemming.CountDownTimer;

        return lemming.IsFastForward
            ? (countDownTimer + EngineConstants.TicksPerSecond - 1) / EngineConstants.TicksPerSecond
            : (countDownTimer + EngineConstants.StandardTicksPerSecond - 1) / EngineConstants.StandardTicksPerSecond;
    }
}