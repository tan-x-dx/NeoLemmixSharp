using NeoLemmixSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeoLemmixSharp.Engine.Directions.Orientations;

public interface IOrientation : IEquatable<IOrientation>
{
    public static ReadOnlyCollection<IOrientation> AllOrientations { get; } = GenerateRotationCollection();

    private static ReadOnlyCollection<IOrientation> GenerateRotationCollection()
    {
        var list = new List<IOrientation>
        {
            DownOrientation.Instance,
            LeftOrientation.Instance,
            UpOrientation.Instance,
            RightOrientation.Instance
        };

        return new ReadOnlyCollection<IOrientation>(list);
    }

    protected static PixelManager Terrain => LevelScreen.CurrentLevel!.Terrain;

    int RotNum { get; }

    LevelPosition MoveRight(LevelPosition position, int step);
    LevelPosition MoveUp(LevelPosition position, int step);
    LevelPosition MoveLeft(LevelPosition position, int step);
    LevelPosition MoveDown(LevelPosition position, int step);

    /// <summary>
    /// Note for the relativeDirection parameter - Positive x -> right, positive y -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="relativeDirection"></param>
    /// <returns></returns>
    LevelPosition Move(LevelPosition position, LevelPosition relativeDirection);
    /// <summary>
    /// Note: Positive dx -> right, positive dy -> up
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    LevelPosition Move(LevelPosition position, int dx, int dy);

    ActionSprite GetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle);
    ActionSprite GetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle);
    void SetLeftActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite leftSprite);
    void SetRightActionSprite(LemmingActionSpriteBundle actionSpriteBundle, ActionSprite rightSprite);
}