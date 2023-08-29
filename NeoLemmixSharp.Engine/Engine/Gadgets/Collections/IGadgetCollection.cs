using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Engine.Lemmings;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace NeoLemmixSharp.Engine.Engine.Gadgets.Collections;

public interface IGadgetCollection
{
    [Pure]
    bool TryGetGadgetThatMatchesTypeAndOrientation(Lemming lemming, LevelPosition levelPosition, [NotNullWhen(true)] out Gadget? gadget);
}