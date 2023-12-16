using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NeoLemmixSharp.Engine.Rendering.Ui;

public sealed class ControlPanelSpriteBankBuilder
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly Dictionary<ControlPanelTexture, Texture2D> _textureLookup = new();

    public ControlPanelSpriteBankBuilder(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _contentManager = contentManager;
    }

    public ControlPanelSpriteBank BuildControlPanelSpriteBank()
    {
        CreateAnchorTexture();
        CreateWhitePixelTexture();
        LoadPanelTextures();

        return new ControlPanelSpriteBank(_textureLookup);
    }

    private void CreateAnchorTexture()
    {
        var anchorTexture = new Texture2D(_graphicsDevice, 3, 3);

        var red = new Color(200, 0, 0, 255).PackedValue;
        var yellow = new Color(200, 200, 0, 255).PackedValue;

        var x = new uint[9];
        x[1] = red;
        x[3] = red;
        x[4] = yellow;
        x[5] = red;
        x[7] = red;
        anchorTexture.SetData(x);

        _textureLookup.Add(ControlPanelTexture.LemmingAnchorTexture, anchorTexture);
    }

    private void CreateWhitePixelTexture()
    {
        var whitePixelTexture = new Texture2D(_graphicsDevice, 1, 256);

        var whiteColors = Enumerable
            .Range(0, 256)
            .Select(alpha => new Color(0xff, 0xff, 0xff, 255 - alpha))
            .ToArray();

        whitePixelTexture.SetData(whiteColors);
        _textureLookup.Add(ControlPanelTexture.WhitePixel, whitePixelTexture);
    }

    private void LoadPanelTextures()
    {
        RegisterTexture(ControlPanelTexture.PanelEmptySlot);
        RegisterTexture(ControlPanelTexture.PanelIconCpmAndReplay);
        RegisterTexture(ControlPanelTexture.PanelIconDirectional);
        RegisterTexture(ControlPanelTexture.PanelIconFastForward);
        RegisterTexture(ControlPanelTexture.PanelIconFrameskip);
        RegisterTexture(ControlPanelTexture.PanelIconNuke);
        RegisterTexture(ControlPanelTexture.PanelIconPause);
        RegisterTexture(ControlPanelTexture.PanelIconRestart);
        RegisterTexture(ControlPanelTexture.PanelIconReleaseRateMinus);
        RegisterTexture(ControlPanelTexture.PanelIconReleaseRatePlus);
        RegisterTexture(ControlPanelTexture.PanelMinimapRegion);
        RegisterTexture(ControlPanelTexture.PanelPanelIcons);
        RegisterTexture(ControlPanelTexture.PanelSkillCountErase);
        RegisterTexture(ControlPanelTexture.PanelSkillPanels);
        RegisterTexture(ControlPanelTexture.PanelSkillSelected);
    }
    private void LoadCursorSprites()
    {
        RegisterTexture(ControlPanelTexture.CursorStandard);
        RegisterTexture(ControlPanelTexture.CursorFocused);
    }

    private void RegisterTexture(ControlPanelTexture textureName)
    {
        var texture = _contentManager.Load<Texture2D>(textureName.GetString());
        _textureLookup.Add(textureName, texture);
    }

    /*
     *
    private readonly Dictionary<string, LemmingActionSpriteBundle> _actionSpriteBundleLookup = new();
    private readonly Dictionary<string, ISprite> _spriteLookup = new();

    public SpriteBankBuilder(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
    }

    public SpriteBank BuildSpriteBank(
        ContentManager content,
        ThemeData themeData,
        TerrainSprite terrainSprite,
        ICollection<GadgetData> allGadgetData)
    {
        _spriteLookup.Add(SpriteBankTextureNames.LevelCursor, cursorSprite);

        LoadLemmingSprites(themeData);
        LoadGadgetSprites(allGadgetData);
        LoadOtherTextures(content);

        return new SpriteBank(
            _actionSpriteBundleLookup,
            _textureLookup,
            _spriteLookup,
            terrainSprite);
    }

    private void LoadLemmingSprites(ThemeData themeData)
    {
        foreach (var lemmingState in LemmingAction.AllLemmingActions.Where(la => la.Id >= 0))
        {
            var pngFilePath = Path.Combine(themeData.LemmingSpritesFilePath, $"{lemmingState.LemmingActionName}.png");

            var spriteIdentifier = GetSpriteIdentifier(lemmingState.LemmingActionName);
            var spriteData = themeData.LemmingSpriteDataLookup[spriteIdentifier];

            var texture = Texture2D.FromFile(_graphicsDevice, pngFilePath);

            ProcessLemmingSpriteTexture(lemmingState.LemmingActionName, spriteData, texture);
        }
    }

    private static string GetSpriteIdentifier(string lemmingStateName)
    {
        return $"${lemmingStateName.ToUpperInvariant()}";
    }

    private void ProcessLemmingSpriteTexture(string stateName, LemmingSpriteData spriteData, Texture2D texture)
    {
        var originalPixelColorData = PixelColorData.GetPixelColorDataFromTexture(texture);

        var actionSpriteBundle = new LemmingActionSpriteBundle();
        LemmingAction.AllActions[stateName].ActionSpriteBundle = actionSpriteBundle;

        _actionSpriteBundleLookup.Add(stateName, actionSpriteBundle);

        ProcessLefts(_graphicsDevice, spriteData, originalPixelColorData, actionSpriteBundle);
        ProcessRights(_graphicsDevice, spriteData, originalPixelColorData, actionSpriteBundle);
    }

    private static void ProcessLefts(
        GraphicsDevice graphicsDevice,
        LemmingSpriteData spriteData,
        PixelColorData originalPixelColorData,
        LemmingActionSpriteBundle actionSpriteBundle)
    {
        CreateSprites(
            graphicsDevice,
            spriteData,
            originalPixelColorData,
            0,
            spriteData.LeftFootX,
            spriteData.LeftFootY,
            actionSpriteBundle,
            (o, b, a) => o.SetLeftActionSprite(b, a));
    }

    private static void ProcessRights(
        GraphicsDevice graphicsDevice,
        LemmingSpriteData spriteData,
        PixelColorData originalPixelColorData,
        LemmingActionSpriteBundle actionSpriteBundle)
    {
        CreateSprites(
            graphicsDevice,
            spriteData,
            originalPixelColorData,
            originalPixelColorData.Width / 2,
            spriteData.RightFootX,
            spriteData.RightFootY,
            actionSpriteBundle,
            (o, b, a) => o.SetRightActionSprite(b, a));
    }

    private static void CreateSprites(
        GraphicsDevice graphicsDevice,
        LemmingSpriteData spriteData,
        PixelColorData originalPixelColorData,
        int dx0,
        int footX,
        int footY,
        LemmingActionSpriteBundle actionSpriteBundle,
        Action<Orientation, LemmingActionSpriteBundle, ActionSprite> setSprite)
    {
        var spriteWidth = originalPixelColorData.Width / 2;
        var spriteHeight = originalPixelColorData.Height / spriteData.NumberOfFrames;

        var spriteDrawingDatas = Orientation
            .AllOrientations
            .Select(o => new SpriteDrawingData(o, spriteWidth, spriteHeight, spriteData.NumberOfFrames))
            .ToArray();

        for (var f = 0; f < spriteData.NumberOfFrames; f++)
        {
            for (var x0 = 0; x0 < spriteWidth; x0++)
            {
                for (var y0 = 0; y0 < spriteHeight; y0++)
                {
                    var pixel = originalPixelColorData.Get(x0 + dx0, y0 + f * spriteHeight);

                    for (var i = 0; i < spriteDrawingDatas.Length; i++)
                    {
                        spriteDrawingDatas[i].Set(pixel, x0, y0, f);
                    }
                }
            }
        }

        foreach (var spriteDrawingData in spriteDrawingDatas)
        {
            var texture = spriteDrawingData.ToTexture(graphicsDevice);

            spriteDrawingData.DihedralTransformation.Transform(footX,
                footY,
                spriteWidth - 1,
                spriteHeight - 1,
                out var footX1,
                out var footY1);

            var actionSprite = new ActionSprite(
                texture,
                spriteDrawingData.ThisSpriteWidth,
                spriteDrawingData.ThisSpriteHeight,
                spriteData.NumberOfFrames,
                new LevelPosition(footX1, footY1));

            setSprite(spriteDrawingData.Orientation, actionSpriteBundle, actionSprite);
        }
    }

    private void LoadGadgetSprites(ICollection<GadgetData> allGadgetData)
    {
    }

     */

}