﻿using NeoLemmixSharp.Engine.Level;
using NeoLemmixSharp.Engine.LevelBuilding.Data;

namespace NeoLemmixSharp.Menu.Pages;

public sealed class LevelStartPage : PageBase
{
    public LevelStartPage(
        MenuInputController inputController,
        LevelScreen levelScreen,
        LevelData levelData)
        : base(inputController)
    {
    }

    protected override void OnInitialise()
    {
        throw new NotImplementedException();
    }

    protected override void OnWindowDimensionsChanged(int windowWidth, int windowHeight)
    {
        throw new NotImplementedException();
    }

    public override void Tick()
    {
        throw new NotImplementedException();
    }

    protected override void OnDispose()
    {
        throw new NotImplementedException();
    }
}