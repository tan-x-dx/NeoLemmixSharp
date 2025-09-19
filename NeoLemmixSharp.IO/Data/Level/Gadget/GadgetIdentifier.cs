using System.Diagnostics;

namespace NeoLemmixSharp.IO.Data.Level.Gadget;

[DebuggerDisplay("GadgetId: {GadgetId}")]
public readonly record struct GadgetIdentifier(int GadgetId);
