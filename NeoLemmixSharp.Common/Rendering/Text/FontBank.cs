﻿using Microsoft.Xna.Framework.Content;

namespace NeoLemmixSharp.Common.Rendering.Text;

public static class FontBank
{
    public static void Initialise(ContentManager contentManager)
    {
        MenuFont = new MenuFont(contentManager);
        PanelFont = new PanelFont(contentManager);
        SkillCountDigitFont = new SkillCountDigitFont(contentManager);
    }

    public static MenuFont MenuFont { get; private set; } = null!;
    public static PanelFont PanelFont { get; private set; } = null!;
    public static SkillCountDigitFont SkillCountDigitFont { get; private set; } = null!;
}