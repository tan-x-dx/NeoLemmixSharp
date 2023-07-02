using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NeoLemmixSharp.Engine.Engine;
using NeoLemmixSharp.Engine.Rendering.Level.Ui;
using NeoLemmixSharp.Engine.Rendering.Level.Viewport;

namespace NeoLemmixSharp.Engine.LevelBuilding;

public sealed class ControlPanelSpriteBankBuilder : IDisposable
{
    private readonly ContentManager _contentManager;
    private readonly GraphicsDevice _graphicsDevice;

    public ControlPanelSpriteBankBuilder(
        GraphicsDevice graphicsDevice,
        ContentManager contentManager)
    {
        _graphicsDevice = graphicsDevice;
        _contentManager = contentManager;
    }

    public ControlPanelSpriteBank BuildControlPanelSpriteBank(LevelCursor levelCursor)
    {
        var textureLookup = new Dictionary<string, Texture2D>();

        CreateAnchorTexture(textureLookup);
        CreateWhitePixelTexture(textureLookup);
        LoadPanelTextures(textureLookup);

        var cursorSprite = LoadCursorSprites(levelCursor);

        return new ControlPanelSpriteBank(textureLookup, cursorSprite);
    }

    private void CreateAnchorTexture(IDictionary<string, Texture2D> textureLookup)
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

        textureLookup.Add("LemmingAnchorTexture", anchorTexture);
    }

    private void CreateWhitePixelTexture(IDictionary<string, Texture2D> textureLookup)
    {
        var whitePixelTexture = new Texture2D(_graphicsDevice, 1, 1);

        var x = new[] { Color.White.PackedValue };

        whitePixelTexture.SetData(x);
        textureLookup.Add("WhitePixel", whitePixelTexture);
    }

    private void LoadPanelTextures(IDictionary<string, Texture2D> textureLookup)
    {
        RegisterTexture("panel/empty_slot");
        RegisterTexture("panel/icon_cpm_and_replay");
        RegisterTexture("panel/icon_directional");
        RegisterTexture("panel/icon_ff");
        RegisterTexture("panel/icon_frameskip");
        RegisterTexture("panel/icon_nuke");
        RegisterTexture("panel/icon_pause");
        RegisterTexture("panel/icon_restart");
        RegisterTexture("panel/icon_rr_minus");
        RegisterTexture("panel/icon_rr_plus");
        RegisterTexture("panel/minimap_region");
        RegisterTexture("panel/panel_icons");
        RegisterTexture("panel/skill_count_erase");
        RegisterTexture("panel/skill_panels");
        RegisterTexture("panel/skill_selected");

        void RegisterTexture(string textureName)
        {
            var texture = _contentManager.Load<Texture2D>(textureName);
            textureLookup.Add(textureName, texture);
        }
    }
    private LevelCursorSprite LoadCursorSprites(LevelCursor levelCursor)
    {
        var standardCursorTexture = _contentManager.Load<Texture2D>("cursor/standard");
        var focusedCursorTexture = _contentManager.Load<Texture2D>("cursor/focused");

        return new LevelCursorSprite(levelCursor, standardCursorTexture, focusedCursorTexture);
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
        var originalPixelColourData = PixelColourData.GetPixelColourDataFromTexture(texture);

        var actionSpriteBundle = new LemmingActionSpriteBundle();
        LemmingAction.AllActions[stateName].ActionSpriteBundle = actionSpriteBundle;

        _actionSpriteBundleLookup.Add(stateName, actionSpriteBundle);

        ProcessLefts(_graphicsDevice, spriteData, originalPixelColourData, actionSpriteBundle);
        ProcessRights(_graphicsDevice, spriteData, originalPixelColourData, actionSpriteBundle);
    }

    private static void ProcessLefts(
        GraphicsDevice graphicsDevice,
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        LemmingActionSpriteBundle actionSpriteBundle)
    {
        CreateSprites(
            graphicsDevice,
            spriteData,
            originalPixelColourData,
            0,
            spriteData.LeftFootX,
            spriteData.LeftFootY,
            actionSpriteBundle,
            (o, b, a) => o.SetLeftActionSprite(b, a));
    }

    private static void ProcessRights(
        GraphicsDevice graphicsDevice,
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        LemmingActionSpriteBundle actionSpriteBundle)
    {
        CreateSprites(
            graphicsDevice,
            spriteData,
            originalPixelColourData,
            originalPixelColourData.Width / 2,
            spriteData.RightFootX,
            spriteData.RightFootY,
            actionSpriteBundle,
            (o, b, a) => o.SetRightActionSprite(b, a));
    }

    private static void CreateSprites(
        GraphicsDevice graphicsDevice,
        LemmingSpriteData spriteData,
        PixelColourData originalPixelColourData,
        int dx0,
        int footX,
        int footY,
        LemmingActionSpriteBundle actionSpriteBundle,
        Action<Orientation, LemmingActionSpriteBundle, ActionSprite> setSprite)
    {
        var spriteWidth = originalPixelColourData.Width / 2;
        var spriteHeight = originalPixelColourData.Height / spriteData.NumberOfFrames;

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
                    var pixel = originalPixelColourData.Get(x0 + dx0, y0 + f * spriteHeight);

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


    public void Dispose()
    {

    }
}