﻿using NeoLemmixSharp.Common.Util;
using NeoLemmixSharp.Engine.Level.Gadgets.HitBoxGadgets.StatefulGadgets;
using NeoLemmixSharp.Engine.Level.Gadgets.LevelRegion;

namespace NeoLemmixSharp.Engine.Level.Gadgets;

public interface IGadget : IRectangularBounds
{
    int Id { get; }
    RectangularLevelRegion GadgetBounds { get; }
}

public interface IControlledAnimationGadget : IGadget
{
    GadgetStateAnimationController AnimationController { get; }
}

public interface IResizeableGadget : IGadget
{
    void Resize(int dw, int dh);
    void SetSize(int w, int h);
}